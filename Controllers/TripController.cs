using Cwiczenie12.DTOs;
using Cwiczenie12.Models;
using Cwiczenie12.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenie12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly IDbService _dbService;

    public TripController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpGet("/api/trips")]
    public async Task<IActionResult> GetTripsAsync([FromQuery] int page = 1, int pageSize = 10)
    {
        var context = await _dbService.GetTrips(page, pageSize);
        return Ok(context);
    }

    [HttpDelete("/api/clients/{idClient}")]
    public async Task<IActionResult> DeleteClientAsync(int idClient)
    {
        try
        {
            await _dbService.DeleteClientAsync(idClient);
            return Ok("Client deleted");
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("/api/trips/{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTripAsync([FromRoute] int idTrip, [FromBody] ClientToTripDTO request)
    {
        try
        {
            await _dbService.AddClientToTripAsync(idTrip, request);
            return Ok("Client added");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}

