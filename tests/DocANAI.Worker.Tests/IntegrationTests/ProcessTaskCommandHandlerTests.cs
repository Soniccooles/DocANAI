using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DocANAI.Persistence;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Features.ProcessTask;
using DocANAI.Worker.Infrastructure.Chunking;
using DocANAI.Worker.Infrastructure.Llm;
using DocANAI.Worker.Infrastructure.Parsing;
using DocANAI.Worker.Infrastructure.Reports;
using DocANAI.Worker.Infrastructure.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using TaskStatusEnum = DocANAI.Persistence.Entities.ProcessingTasks.TaskStatus;

namespace DocANAI.Worker.Tests.IntegrationTests;

public class ProcessTaskCommandHandlerTests : PostgresTestBase
{
    private IContainer BuildContainer(IObjectStorageService storage, IAnswerGenerator answerGenerator)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Options.Create(new ChunkSettings()));
        services.AddSingleton(storage);
        services.AddSingleton(answerGenerator);
        services.AddSingleton<IDocumentTextExtractor, DocumentTextExtractor>();
        services.AddSingleton<IExcelReportBuilder, ExcelReportBuilder>();
        services.AddSingleton<IDocumentChunker, DocumentChunker>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessTaskCommand).Assembly));

        var builder = new ContainerBuilder();
        builder.Populate(services);
        builder.RegisterModule(new PersistenceInfrastructureModule { ConnectionString = ConnectionString });

        return builder.Build();
    }

    private async Task<IdOf<ProcessingTask>> SeedTaskAsync()
    {
        await using var db = CreateDbContext();

        var user = User.Create(IdOf<User>.New(), $"integration-{Guid.NewGuid():N}", "not-a-real-hash");
        var task = ProcessingTask.Create(IdOf<ProcessingTask>.New(), user.Id, SeededModelId, SeededPriorityId);
        task.Start();

        var document = SourceDocument.Create(
            IdOf<SourceDocument>.New(), user.Id, task.Id, ".txt", "article.txt", "documents/article.txt", 1);
        var question = Question.Create(
            IdOf<Question>.New(), task.Id, questionFileId: null, questionNumber: 1, "О чём документ?");

        db.AddRange(user, task, document, question);
        await db.SaveChangesAsync();

        return task.Id;
    }

    private static async Task ProcessAsync(IContainer container, IdOf<ProcessingTask> taskId)
    {
        await using var scope = container.BeginLifetimeScope();
        var mediator = scope.Resolve<IMediator>();

        await mediator.Send(new ProcessTaskCommand(taskId));
    }

    private async Task RestartTaskAsync(IdOf<ProcessingTask> taskId)
    {
        await using var db = CreateDbContext();

        var task = await db.ProcessingTasks.SingleAsync(t => t.Id == taskId);
        task.Start();
        db.Update(task);
        await db.SaveChangesAsync();
    }

    private async Task<int> CountChunksAsync(IdOf<ProcessingTask> taskId)
    {
        await using var db = CreateDbContext();

        return await db.DocumentChunks
            .CountAsync(c => db.SourceDocuments.Any(d => d.Id == c.SourceDocumentId && d.TaskId == taskId));
    }
    
    [Test]
    public async Task process_task_retry_after_failure_replaces_chunks()
    {
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TestData", "russian_article.txt"));
        var expectedChunks = new DocumentChunker(Options.Create(new ChunkSettings())).Split(text).Count;
        var taskId = await SeedTaskAsync();
        
        var storage = Substitute.For<IObjectStorageService>();
        storage.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_ => new MemoryStream(Encoding.UTF8.GetBytes(text)));
        
        var answerGenerator = Substitute.For<IAnswerGenerator>();
        answerGenerator.GenerateAnswerAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Question>(), Arg.Any<CancellationToken>())
            .Returns(
                _ => throw new InvalidOperationException("LLM is unavailable"),
                _ => "Тестовый ответ");
        await using var container = BuildContainer(storage, answerGenerator);
        Assert.ThrowsAsync<InvalidOperationException>(() => ProcessAsync(container, taskId));
        Assert.That(await CountChunksAsync(taskId), Is.EqualTo(expectedChunks));
        await RestartTaskAsync(taskId);
        
        await ProcessAsync(container, taskId);
        
        await using var db = CreateDbContext();
        var task = await db.ProcessingTasks.SingleAsync(t => t.Id == taskId);
        var indexes = await db.DocumentChunks
            .Where(c => db.SourceDocuments.Any(d => d.Id == c.SourceDocumentId && d.TaskId == taskId))
            .OrderBy(c => c.ChunkIndex)
            .Select(c => c.ChunkIndex)
            .ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(task.Status, Is.EqualTo(TaskStatusEnum.Done));
            Assert.That(indexes, Is.EqualTo(Enumerable.Range(0, expectedChunks)));
        });
    }
}