using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(RaceDayContext db, PasswordService passwords) : ControllerBase
{
 [HttpPost("register")]
 public async Task<IActionResult> Register(RegisterRequest request)
 {
  var roleName = request.Role.Trim();
  if (roleName is not ("Organizer" or "Participant")) return BadRequest("Role must be Organizer or Participant; Administrator registration is not permitted.");
  if (await db.Users.AnyAsync(x => x.Email == request.Email)) return Conflict("Email is already registered.");
  var role = await db.Roles.SingleOrDefaultAsync(x => x.RoleName == roleName);
  if (role is null) return Problem("Required roles are missing from RaceDayDB. Run scripts/PrepareRaceDayDB.sql and try again.", statusCode: 500);
  var user = new User { FirstName=request.FirstName.Trim(), LastName=request.LastName.Trim(), Email=request.Email.Trim().ToLowerInvariant(),
    PasswordHash=passwords.Hash(request.Password), PhoneNumber=request.PhoneNumber, RoleID=role.RoleID, IsActive=true };
  db.Users.Add(user); await db.SaveChangesAsync();
  return Created("", new { user.UserID, user.FirstName, user.LastName, user.Email, Role=roleName });
 }
 [HttpPost("login")]
 public async Task<IActionResult> Login(LoginRequest request)
 {
  var user = await db.Users.Include(x=>x.Role).SingleOrDefaultAsync(x=>x.Email==request.Email.ToLower());
  if (user is null || !user.IsActive || !passwords.Verify(user.PasswordHash,request.Password)) return Unauthorized("Invalid email or password.");
  HttpContext.Session.SetInt32("UserId",user.UserID); HttpContext.Session.SetString("Role",user.Role!.RoleName); HttpContext.Session.SetString("Email",user.Email);
  return Ok(new { user.UserID,user.FirstName,user.LastName,user.Email,Role=user.Role.RoleName });
 }
 [HttpPost("logout")]
 public IActionResult Logout() { HttpContext.Session.Clear(); return NoContent(); }
 [HttpGet("me")]
 public async Task<IActionResult> Me()
 {
  var id=HttpContext.Session.GetInt32("UserId"); if(id is null) return Unauthorized();
  var u=await db.Users.Include(x=>x.Role).Where(x=>x.UserID==id).Select(x=>new{x.UserID,x.FirstName,x.LastName,x.Email,x.PhoneNumber,Role=x.Role!.RoleName}).SingleOrDefaultAsync();
  return u is null ? Unauthorized() : Ok(u);
 }
}
