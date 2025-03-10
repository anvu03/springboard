using MediatR;
using SpringBoard.Application.Abstractions.Persistence;
using SpringBoard.Domain.Exceptions;

namespace SpringBoard.Application.Features.Auth.Commands;

/// <summary>
/// Command for revoking a specific refresh token.
/// </summary>
public record RevokeTokenCommand : IRequest
{
    /// <summary>
    /// Gets or sets the token to revoke.
    /// </summary>
    public required string Token { get; init; }
}

/// <summary>
/// Handler for processing the <see cref="RevokeTokenCommand"/>.
/// </summary>
public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeTokenCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public RevokeTokenCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the revocation of a refresh token.
    /// </summary>
    /// <param name="request">The revoke token command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            throw new InvalidEntityStateException("Token", "Token cannot be empty");
        }

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Find the token
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.Token);
            
            if (refreshToken == null)
            {
                throw new EntityNotFoundException("Refresh token not found");
            }

            // Revoke the token
            await _unitOfWork.RefreshTokens.RevokeAsync(request.Token, DateTime.UtcNow);
            
            // Save changes and commit the transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            // Rollback transaction if an error occurs
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
