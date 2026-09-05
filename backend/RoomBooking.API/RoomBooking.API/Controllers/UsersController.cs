using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomBooking.API.Contracts.UserContracts;
using RoomBooking.API.FailureHandlers;
using RoomBooking.Application.DTOs.User;
using RoomBooking.Core.Abstractions.Services;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFailureHandler _failureHandler;
    public UsersController(IUserService userService, IFailureHandler failureHandler)
    {
        _userService = userService;
        _failureHandler = failureHandler;
    }

    [Authorize]
    [HttpGet]
    [EnableRateLimiting("token-by-ip")] //for lists, burst is allowed
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllUsers(cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulUsers = result.Value!;
        
        var response = successfulUsers.Select(u => new UserResponse(
            u.Id,
            u.Name,
            u.Email,
            u.Department));

        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [EnableRateLimiting("fixed-by-ip")] //for single requests strict limit
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        
        var result = await _userService.GetUserById(id, cancellationToken);

        if (result.IsFailure)
        {
            //needs test
            return _failureHandler.HandleFailure(result, HttpContext);
        }
        
        var successfulUser = result.Value!;
        
        var response = new UserResponse(
            successfulUser.Id,
            successfulUser.Name,
            successfulUser.Email,
            successfulUser.Department);
        
        return Ok(response);
    }

    [HttpPost("create-user")]
    [EnableRateLimiting("fixed-by-ip")] // For creation | spam protection
    public async Task<ActionResult<Guid>> CreateUser([FromBody] UserCreationRequest userCreationRequest,
        CancellationToken cancellationToken)
        {
        var result = await _userService.CreateUser(
            userCreationRequest.Name,
            userCreationRequest.Email,
            userCreationRequest.Department,
            userCreationRequest.Password,
            cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulUser = result.Value;

        return Ok(successfulUser);
    }

    [HttpPost("login")]
    [EnableRateLimiting("fixed-by-ip")]
    public async Task<ActionResult<string>> Login([FromBody] UserLoginRequest userLoginRequest,
        CancellationToken cancellationToken)
    {
        var token = await _userService.Login(userLoginRequest.Email, userLoginRequest.Password, cancellationToken);

        if (token.IsFailure)
        {
            return _failureHandler.HandleFailure(token, HttpContext);
        }
        
        //saving a token to cookies 
        HttpContext.Response.Cookies.Append("my-cookies", token.Value!, 
            new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        
        return Ok(token.Value);
    }

    // [Authorize]
    [HttpPost("logout")]
    // [EnableRateLimiting("fixed-by-ip")]
    public async Task<ActionResult> LogOut(CancellationToken cancellationToken)
    {
        HttpContext.Response.Cookies.Delete("my-cookies");

        return Ok();
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [EnableRateLimiting("fixed-by-ip")] //for updates
    public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UserUpdateDto userUpdateDto, CancellationToken cancellationToken)
    {
        
        var result = await _userService.UpdateUser(
            id,
            userUpdateDto.Name,
            userUpdateDto.Email,
            userUpdateDto.Department,
            cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulUser = result.Value!;

        //Not returning id
        var rowsAffected = Enumerable
            .Range(0, successfulUser.Length)
            .Where(i => successfulUser[i]?.GetType() != typeof(Guid))
            .Select(i => successfulUser[i]?.ToString())
            .ToList();
        
        return Ok(rowsAffected);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("fixed-by-ip")]
    public async Task<ActionResult<Guid>> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUser(id, cancellationToken);
        
        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }
        
        var successfulUser = result.Value;
        
        return Ok(successfulUser);
    }

    [Authorize]
    [HttpPost("add-user-address")]
    [EnableRateLimiting("concurrency-by-ip")] //for db operations | Protects db from too many connections
    public async Task<ActionResult<Guid>> AddUserAddressInfo(Guid id, [FromBody] UserAddressInfoRequest addressInfoRequest,
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserById(id, cancellationToken);
        
        //fabric method
        var addressInfo = user.Value?.AddAddressInfo( // making user value nullable to avoid NRF | NEEDS REFACTOR
            addressInfoRequest.street,
            addressInfoRequest.city,
            addressInfoRequest.state,
            addressInfoRequest.postalCode,
            addressInfoRequest.country) !;
        
        var result = await _userService.AddAddressInfo(id, addressInfo, cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulUser = result.Value!;

        var affectedRows = Enumerable
            .Range(0, successfulUser.Length)
            .Where(i => successfulUser[i]?.GetType() != typeof(Guid))
            .Select(i => successfulUser[i]?.ToString())
            .ToList();
        
        return Ok(affectedRows);
    }

    // [HttpDelete]
    // [EnableRateLimiting("concurrency-by-ip")]
    // public async Task<ActionResult<int>> DeleteAddUsers(CancellationToken cancellationToken)
    // {
    //     return Ok( await _userService.DeleteAllUsers(cancellationToken));
    // }
}