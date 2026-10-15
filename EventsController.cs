using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/events")]
public class EventsController(RaceDayContext db) : ControllerBase
{
 int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 [HttpGet,AllowAnonymous] public async Task<IActionResult> List() => Ok(await db.Events.Select(e=>new{e.EventID,e.EventName,e.Description,e.EventDate,e.Location,e.Status,e.OrganizerID}).ToListAsync());
 [HttpGet("{id:int}"),AllowAnonymous] public async Task<IActionResult> Get(int id) { var e=await db.Events.FindAsync(id);return e is null?NotFound():Ok(e); }
 [HttpPost,Authorize(Roles="Organizer")] public async Task<IActionResult> Create(EventRequest r) { var e=new RaceEvent{OrganizerID=UserId,EventName=r.EventName,Description=r.Description,EventDate=r.EventDate,Location=r.Location,Status=r.Status};db.Events.Add(e);await db.SaveChangesAsync();return CreatedAtAction(nameof(Get),new{id=e.EventID},e); }
 [HttpPut("{id:int}"),Authorize(Roles="Organizer")] public async Task<IActionResult> Update(int id,EventRequest r) { var e=await db.Events.FindAsync(id);if(e is null)return NotFound();if(e.OrganizerID!=UserId)return Forbid();e.EventName=r.EventName;e.Description=r.Description;e.EventDate=r.EventDate;e.Location=r.Location;e.Status=r.Status;await db.SaveChangesAsync();return NoContent(); }
 [HttpDelete("{id:int}"),Authorize(Roles="Organizer")] public async Task<IActionResult> Delete(int id) { var e=await db.Events.FindAsync(id);if(e is null)return NotFound();if(e.OrganizerID!=UserId)return Forbid();db.Events.Remove(e);await db.SaveChangesAsync();return NoContent(); }
}
