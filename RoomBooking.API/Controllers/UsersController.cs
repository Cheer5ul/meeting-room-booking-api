using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts.UserContracts;
using RoomBooking.API.FailureHandlers;
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

    [HttpGet]
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

    [HttpGet("{id:guid}")]
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
    public async Task<ActionResult<Guid>> CreateUser([FromBody] UserRequest userRequest,
        CancellationToken cancellationToken)
    {
        var (user, error) = Core.Models.User.User.Create(
            Guid.NewGuid(),
             userRequest.Name,
             userRequest.Email,
            userRequest.Department);

        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }

        var result = await _userService.CreateUser(user, cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulUser = result.Value;

        return Ok(successfulUser);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UserRequest userRequest, CancellationToken cancellationToken)
    {
        
        var result = await _userService.UpdateUser(
            id,
            userRequest.Name,
            userRequest.Email,
            userRequest.Department,
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

    [HttpDelete("{id:guid}")]
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

    [HttpPost("add-user-address")]
    public async Task<ActionResult<Guid>> AddUserAddressInfo(Guid id, [FromBody] UserAddressInfoRequest addressInfoRequest,
        CancellationToken cancellationToken)
    {
        var result = await _userService.AddAddressInfo(
            id,
            addressInfoRequest.street,
            addressInfoRequest.city,
            addressInfoRequest.state,
            addressInfoRequest.postalCode,
            addressInfoRequest.country,
            cancellationToken);
        

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
}