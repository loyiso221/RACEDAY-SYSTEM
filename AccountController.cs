using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
using System.Text.Json;
namespace RaceDay.Web.Controllers;
public class AccountController(RaceDayApiClient api) : Controller
{
    [HttpGet] public IActionResult Login()=>View(new LoginViewModel());
    [HttpPost] [ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(!ModelState.IsValid)return View(model);
        using var response=await api.LoginAsync(HttpContext,new {model.Email,model.Password});
        if(!response.IsSuccessStatusCode){ModelState.AddModelError("","Login failed. Check your email and password.");return View(model);}
        var json=await response.Content.ReadFromJsonAsync<JsonElement>();
        string Read(string key)=>json.TryGetProperty(key,out var v)?v.ToString():"";
        HttpContext.Session.SetString("Role",Read("role") is {Length:>0} role?role:Read("Role"));
        HttpContext.Session.SetInt32("UserId",int.TryParse(Read("userID"),out var id)?id:int.Parse(Read("UserID")));
        HttpContext.Session.SetString("Email",model.Email); HttpContext.Session.SetString("FirstName",Read("firstName") is {Length:>0} first?first:Read("FirstName"));
        return RedirectToAction("Index", "Home");
    }
    [HttpGet] public IActionResult Register()=>View(new RegisterViewModel());
    [HttpPost] [ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if(!ModelState.IsValid)return View(model);
        using var response=await api.JsonAsync(HttpContext,HttpMethod.Post,"api/auth/register",model);
        if(!response.IsSuccessStatusCode){ModelState.AddModelError("","Registration failed: "+await response.Content.ReadAsStringAsync());return View(model);}
        TempData["Success"]="Registration successful. Please log in."; return RedirectToAction(nameof(Login));
    }
    [HttpPost] [ValidateAntiForgeryToken] public async Task<IActionResult> Logout()
    { using var response=await api.SendAsync(HttpContext,HttpMethod.Post,"api/auth/logout"); HttpContext.Session.Clear(); return RedirectToAction("Index","Home"); }
}
