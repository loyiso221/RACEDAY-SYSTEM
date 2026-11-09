using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class ProfileController(RaceDayApiClient api):Controller
{
    private bool LoggedIn()=>HttpContext.Session.GetString("ApiSessionCookie")!=null;
    [HttpGet] public async Task<IActionResult> Index(){if(!LoggedIn())return RedirectToAction("Login","Account");var p=await api.GetAsync<ProfileViewModel>(HttpContext,"api/profile/me");return p==null?RedirectToAction("Login","Account"):View(p);}
    [HttpPost][ValidateAntiForgeryToken] public async Task<IActionResult> Index(ProfileViewModel m,IFormFile? picture){if(!LoggedIn())return Forbid();using var r=await api.JsonAsync(HttpContext,HttpMethod.Put,"api/profile/me",new{m.FirstName,m.LastName,m.PhoneNumber});if(!r.IsSuccessStatusCode)ModelState.AddModelError("","Profile update failed.");if(picture!=null&&picture.Length>0){using var form=new MultipartFormDataContent();await using var s=picture.OpenReadStream();var part=new StreamContent(s);part.Headers.ContentType=new System.Net.Http.Headers.MediaTypeHeaderValue(picture.ContentType);form.Add(part,"file",picture.FileName);using var upload=await api.SendAsync(HttpContext,HttpMethod.Post,"api/profile/me/picture",form);if(!upload.IsSuccessStatusCode)ModelState.AddModelError("","Profile saved, but picture upload failed.");}if(ModelState.IsValid)TempData["Success"]="Profile updated.";return View(m);}
}
