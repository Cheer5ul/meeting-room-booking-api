using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts.UserContracts;
using RoomBooking.Core.Abstractions.Services;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;        
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsers(cancellationToken);

        var response = users.Select(u => new UserResponse(
            u.Id,
            u.Name,
            u.Email,
            u.Department));

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserById(id, cancellationToken);
        
        if (user == null)
            return NotFound();
        
        var response = new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Department);
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] UserRequest userRequest,
        CancellationToken cancellationToken)
    {
        var (user, error) = Core.Models.User.Create(
            Guid.NewGuid(),
             userRequest.Name,
             userRequest.Email,
            userRequest.Department);

        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }

        var userId = await _userService.CreateUser(user, cancellationToken);
        
        return Ok(userId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UserRequest userRequest, CancellationToken cancellationToken)
    {
        
        var userId = await _userService.UpdateUser(
            id,
            userRequest.Name,
            userRequest.Email,
            userRequest.Department,
            cancellationToken);
        
        return Ok(userId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var userId = await _userService.DeleteUser(id, cancellationToken);
        return Ok(userId);
    }
}