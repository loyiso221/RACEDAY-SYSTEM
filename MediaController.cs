using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Storage;
using System.Security.Claims;
namespace RaceDay.Api.Controllers;
[ApiController]
[Route("api")]
public class MediaController(RaceDayContext db,BlobStorageService blobs):ControllerBase
{
    private int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpPost("events/{eventId:int}/banner")][Authorize(Roles="Organizer")][RequestSizeLimit(5*1024*1024)]
    public async Task<IActionResult> Banner(int eventId,IFormFile file,CancellationToken ct){var e=await db.Events.FindAsync(new object[]{eventId},ct);if(e==null)return NotFound();if(e.OrganizerID!=UserId)return Forbid();try{e.BannerImageUrl=await blobs.UploadAsync(file,"event-banners",ct);await db.SaveChangesAsync(ct);return Ok(new{e.EventID,e.BannerImageUrl});}catch(InvalidOperationException ex){return BadRequest(new{message=ex.Message});}}
    [HttpPost("profile/me/picture")][Authorize(Roles="Participant,Organizer")][RequestSizeLimit(5*1024*1024)]
    public async Task<IActionResult> ProfilePicture(IFormFile file,CancellationToken ct){var u=await db.Users.FindAsync(new object[]{UserId},ct);if(u==null)return NotFound();try{u.ProfileImageUrl=await blobs.UploadAsync(file,"profile-pictures",ct);await db.SaveChangesAsync(ct);return Ok(new{u.UserID,u.ProfileImageUrl});}catch(InvalidOperationException ex){return BadRequest(new{message=ex.Message});}}
}
