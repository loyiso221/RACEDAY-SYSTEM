using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/profile"),Authorize]
public class UsersController(RaceDayContext db) : ControllerBase
{
 int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 [HttpGet("me")] public async Task<IActionResult> Get() => Ok(await db.Users.Where(x=>x.UserID==UserId).Select(x=>new{x.UserID,x.FirstName,x.LastName,x.Email,x.PhoneNumber,x.CreatedAt}).SingleAsync());
 [HttpPut("me")] public async Task<IActionResult> Update(ProfileUpdateRequest r) { var u=await db.Users.FindAsync(UserId); if(u is null)return NotFound(); u.FirstName=r.FirstName.Trim();u.LastName=r.LastName.Trim();u.PhoneNumber=r.PhoneNumber;await db.SaveChangesAsync();return NoContent(); }
}
