using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Worker.Features.BuildReport;

public sealed record BuildExcelReportCommand(IdOf<ProcessingTask> TaskId) 
    : IRequest;