﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareMyEvents.Api.Exceptions;
using ShareMyEvents.Domain.Dtos.Requests.EventRequests;
using ShareMyEvents.Domain.Dtos.Responses.EventResponses;
using ShareMyEvents.Domain.Enums;
using ShareMyEvents.Domain.Interfaces;

namespace ShareMyEvents.Api.Controllers;
[Route("events")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ApiController]
public class EventController: ApiController
{
    private readonly IEventService _service;

    public EventController (
        IMediator mediator, 
        CancellationTokenSource cancellationToken, 
        IEventService service) : base(mediator, cancellationToken)
    {
        _service = service;
    }

    /// <summary>
    /// Get event by id
    /// </summary>
    /// <response code="200">Returns requested event</response>
    /// <response code="400">The request is not valid</response>
    /// <response code="404">If the event doesn't exist</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}", Name = "GetEvent")]
    public async Task<ActionResult<EventPageResponse>> GetEventAsync (int id)
    {
        EventPageResponse eventPageResponse;

        try
        {
            eventPageResponse = await _service.GetByIdAsync(id, _cancellationToken);
        }
        catch(NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(Exception ex)
        {
            return Problem(ex.Message, "NewEventAsync", 500);
        }

        return Ok(eventPageResponse);
    }

    /// <summary>
    /// Create new event
    /// </summary>
    /// <response code="201">Returns event freshly created</response>
    /// <response code="400">The request is not valid</response>
    /// <response code="500">Internal server error</response>
    /// <returns>The freshly created Event</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(Role.IdentifiedUser))]
    [Route("new")]
    public async Task<IActionResult> NewEventAsync ([FromBody] EventCreateDto request)
    {
        var result = await _sender.Send(new Requests.EventRequests.EventCreateCommandRequest(request), _cancellationToken);

        if(result.IsFailed)
        {
            return HandleFailure(result);
        }
            
        return CreatedAtRoute(routeName: "GetEvent", routeValues: new { id = result.Response!.EventId }, value: result.Response);
        
    }

    //[HttpPost]
    //[Route("access")]
    //public ActionResult<ParticipantAccessResponse> New ([FromBody] ParticipationAccessRequest request)
    //{
    //    return StatusCode(StatusCodes.Status201Created);
    //}

    /// <summary>
    /// Update event title
    /// </summary>
    /// <response code="200">Returns updated title associated with it event id</response>
    /// <response code="400">The request is not valid</response>
    /// <response code="404">If the event doesn't exist</response>
    /// <response code="500">Internal server error</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(Role.IdentifiedUser))]
    [Route("update/{id}/title")]
    public async Task<ActionResult<EventUpdateTitleResponse>> UpdateEventTitleAsync (int id, [FromBody] EventUpdateTitleDto request)
    {
        EventUpdateTitleResponse eventUpdateTitleResponse;

        try
        {
            eventUpdateTitleResponse = await _service.UpdateTitleResponseAsync(id, request, _cancellationToken);
        }
        catch(NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(Exception ex)
        {
            return Problem(ex.Message, "NewEventAsync", 500);
        }

        return Ok(eventUpdateTitleResponse);
    }

    /// <summary>
    /// Update event description
    /// </summary>
    /// <response code="200">Returns updated description associated with it event id</response>
    /// <response code="400">The request is not valid</response>
    /// <response code="404">If the event doesn't exist</response>
    /// <response code="500">Internal server error</response>
    [HttpPut]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(Role.IdentifiedUser))]
    [Route("update/{id}/description")]
    public async Task<ActionResult<EventUpdateDescriptionResponse>> UpdateEventDescriptionAsync (int id, [FromBody] EventUpdateDescriptionDto request)
    {
        EventUpdateDescriptionResponse eventUpdateDescriptionResponse;

        try
        {
            eventUpdateDescriptionResponse = await _service.UpdateDescriptionResponseAsync(id, request, _cancellationToken);
        }
        catch(NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(Exception ex)
        {
            return Problem(ex.Message, "NewEventAsync", 500);
        }

        return Ok(eventUpdateDescriptionResponse);
    }

    /// <summary>
    /// Update event date
    /// </summary>
    /// <response code="200">Returns updated date associated with it event id</response>
    /// <response code="400">The request is not valid</response>
    /// <response code="404">If the event doesn't exist</response>
    /// <response code="500">Internal server error</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(Role.IdentifiedUser))]
    [Route("update/{id}/date")]
    public async Task<ActionResult<EventUpdateDateResponse>> UpdateEventDateAsync (int id, [FromBody] EventUpdateDateDto request)
    {
        EventUpdateDateResponse eventUpdateDateResponse;

        try
        {
            eventUpdateDateResponse = await _service.UpdateDateResponseAsync(id, request, _cancellationToken);
        }
        catch(NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(Exception ex)
        {
            return Problem(ex.Message, "UpdateEventDateAsync", 500);
        }

        return Ok(eventUpdateDateResponse);
    }

    /// <summary>
    /// Delete event by Id
    /// </summary>
    /// <response code="204">Event has been deleted well</response>
    /// <response code="400">The request is not valid</response>
    /// <response code="404">If the event doesn't exist</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(Role.IdentifiedUser))]
    public async Task<IActionResult> DeleteEventAsync (int id)
    {
        try
        {
            await _service.DeleteAsync(id, _cancellationToken);
        }
        catch(NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(Exception ex)
        {
            return Problem(ex.Message, "DeleteEventAsync", 500);
        }

        return NoContent();
    }
}
