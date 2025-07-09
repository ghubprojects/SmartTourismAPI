using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTourismAPI.Application.Features.Users;

namespace SmartTourismAPI.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public class UserController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetUser([FromQuery] GetUserRequest request) {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
