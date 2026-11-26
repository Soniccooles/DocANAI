using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.Seeder;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DocANAI.Worker.Tests.IntegrationTests;

public class DataIntegrityTests : PostgresTestBase
{
    private async Task<(IdOf<AIModel>, IdOf<ProcessingTask>)> SeedModelWithTaskAsync()
    {
        await using var db = CreateDbContext();

        var user = User.Create(IdOf<User>.New(), $"integration-{Guid.NewGuid():N}", "not-a-real-hash");
        var model = AIModel.Create(IdOf<AIModel>.New(), $"test-model-{Guid.NewGuid():N}", "1.7b");
        var task = ProcessingTask.Create(IdOf<ProcessingTask>.New(), user.Id, model.Id, SeededPriorityId);

        db.AddRange(user, model, task);
        await db.SaveChangesAsync();

        return (model.Id, task.Id);
    }
    
    [Test]
    public async Task delete_model_with_dependent_task_throws_exception()
    {
        var (modelId, taskId) = await SeedModelWithTaskAsync();

        await using (var db = CreateDbContext())
        {
            var model = await db.AIModels.SingleAsync(m => m.Id == modelId);
            db.AIModels.Remove(model);
            
            Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }

        await using var check = CreateDbContext();
        Assert.That(await check.AIModels.AnyAsync(m => m.Id == modelId), Is.True);
        Assert.That(await check.ProcessingTasks.AnyAsync(t => t.Id == taskId), Is.True);
    }

    [Test]
    public async Task delete_priority_with_dependent_task_throws_exception()
    {
        var (_, taskId) = await SeedModelWithTaskAsync();
        await using (var db = CreateDbContext())
        {
            var priority = await db.Priorities.SingleAsync(p => p.Id == SeededPriorityId);
            db.Priorities.Remove(priority);
            
            Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
        
        await using var check = CreateDbContext();
        Assert.That(await check.Priorities.AnyAsync(p => p.Id == SeededPriorityId), Is.True);
        Assert.That(await check.ProcessingTasks.AnyAsync(t => t.Id == taskId), Is.True);
    }

    [Test]
    public async Task seeder_adds_missing_model_while_existing_records_and_tasks_remain()
    {
        var (modelId, taskId) = await SeedModelWithTaskAsync();

        await using (var db = CreateDbContext())
        {
            await db.AIModels.Where(m => m.Id == SeededModelId).ExecuteDeleteAsync();
        }

        await SeedAsync();

        await using (var db = CreateDbContext())
        {
            Assert.That(await db.AIModels.AnyAsync(m => m.Id == SeededModelId), Is.True);
            Assert.That(await db.AIModels.AnyAsync(m => m.Id == modelId), Is.True);
            Assert.That(await db.ProcessingTasks.AnyAsync(t => t.Id == taskId), Is.True);
        }

        var modelsBefore = await CountAsync(db => db.AIModels.CountAsync());
        var tasksBefore = await CountAsync(db => db.ProcessingTasks.CountAsync());

        await SeedAsync();
        
        Assert.That(await CountAsync(db => db.AIModels.CountAsync()), Is.EqualTo(modelsBefore));
        Assert.That(await CountAsync(db => db.ProcessingTasks.CountAsync()), Is.EqualTo(tasksBefore));
    }

    private async Task SeedAsync()
    {
        await using var db = CreateDbContext();
        await new DatabaseSeeder(db, NullLogger<DatabaseSeeder>.Instance).SeedAsync("Development");
    }

    private async Task<int> CountAsync(Func<Persistence.Context.PostgreSqlDbContext, Task<int>> query)
    {
        await using var db = CreateDbContext();
        return await query(db);
    }
}