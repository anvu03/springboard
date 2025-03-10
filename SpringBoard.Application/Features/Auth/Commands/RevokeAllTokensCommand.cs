using MediatR;
using SpringBoard.Application.Abstractions.Persistence;

namespace SpringBoard.Application.Features.Auth.Commands;

/// <summary>
/// Command for revoking all refresh tokens for a user.
/// </summary>
public record RevokeAllTokensCommand : IRequest
{
    /// <summary>
    /// Gets or sets the user ID for which to revoke all tokens.
    /// </summary>
    public required Guid UserId { get; init; }
}

/// <summary>
/// Handler for processing the <see cref="RevokeAllTokensCommand"/>.
/// </summary>
public class RevokeAllTokensCommandHandler : IRequestHandler<RevokeAllTokensCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeAllTokensCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public RevokeAllTokensCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the revocation of all refresh tokens for a user.
    /// </summary>
    /// <param name="request">The revoke all tokens command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task Handle(RevokeAllTokensCommand request, CancellationToken cancellationToken)
    {
        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Revoke all tokens for the user
            await _unitOfWork.RefreshTokens.RevokeAllForUserAsync(request.UserId, DateTime.UtcNow);
            
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
