using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.Auth;
using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Auth.Me;

public sealed record MeCommand(IdOf<User> UserId)
    : IRequest<Result<MeResponse, string>>;