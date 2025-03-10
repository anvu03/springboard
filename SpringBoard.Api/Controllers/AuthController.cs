using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpringBoard.Api.Models.Auth;
using SpringBoard.Application.Abstractions.Services;
using SpringBoard.Application.Features.Auth.Commands;
using SpringBoard.Application.Features.Auth.Models;

namespace SpringBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>The ID of the newly created user</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), new { id = userId }, userId);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and refresh token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand
        {
            UsernameOrEmail = request.UsernameOrEmail,
            Password = request.Password
        };
        
        var authResponse = await _mediator.Send(command);
        
        var response = new AuthResponseModel
        {
            UserId = authResponse.UserId,
            IdToken = authResponse.IdToken,
            RefreshToken = authResponse.RefreshToken
        };
        
        return Ok(response);
    }

    /// <summary>
    /// Refreshes an expired JWT token using a valid refresh token
    /// </summary>
    /// <param name="request">Refresh token details</param>
    /// <returns>New JWT token and refresh token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        };
        
        var authResponse = await _mediator.Send(command);
        
        var response = new AuthResponseModel
        {
            UserId = authResponse.UserId,
            IdToken = authResponse.IdToken,
            RefreshToken = authResponse.RefreshToken
        };
        
        return Ok(response);
    }

    /// <summary>
    /// Revokes the specified refresh token
    /// </summary>
    /// <param name="request">Token to revoke</param>
    /// <returns>Success status</returns>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        var command = new RevokeTokenCommand
        {
            Token = request.Token
        };
        
        await _mediator.Send(command);
        return Ok();
    }

    /// <summary>
    /// Revokes all refresh tokens for the current user
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPost("revoke-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeAllTokens()
    {
        try
        {
            var userId = _currentUserService.GetRequiredUserId();
            
            // Create a command to revoke all tokens for the user
            await _mediator.Send(new RevokeAllTokensCommand { UserId = userId });
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}