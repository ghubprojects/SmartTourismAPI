﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTourismAPI.Application.Features.Reviews;

namespace SmartTourismAPI.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("reviews")]
public class ReviewsController(IMediator mediator) : ControllerBase {
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request) {
        await _mediator.Send(request);
        return Ok();
    }
}
