using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api")]
public class CategoriesController(RaceDayContext db) : ControllerBase
{
 int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 [HttpGet("events/{eventId:int}/categories"),AllowAnonymous] public async Task<IActionResult> List(int eventId) => Ok(await db.Categories.Where(c=>c.EventID==eventId).ToListAsync());
 [HttpPost("events/{eventId:int}/categories"),Authorize(Roles="Organizer")] public async Task<IActionResult> Create(int eventId,CategoryRequest r) {
  var e=await db.Events.FindAsync(eventId);if(e is null)return NotFound("Event not found.");if(e.OrganizerID!=UserId)return Forbid();
  if(r.DistanceKM<=0||r.MinimumAge<0||(r.MaximumAge.HasValue&&r.MaximumAge<r.MinimumAge)||r.EntryFee<0)return BadRequest("Invalid category values.");
  var c=new Category{EventID=eventId,CategoryName=r.CategoryName,DistanceKM=r.DistanceKM,MinimumAge=r.MinimumAge,MaximumAge=r.MaximumAge,EntryFee=r.EntryFee};db.Categories.Add(c);await db.SaveChangesAsync();return Ok(c);
 }
}
