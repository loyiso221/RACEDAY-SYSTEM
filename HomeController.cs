using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using System.Text.Json;
namespace RaceDay.Web.Controllers;
public class HomeController(RaceDayApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        if(HttpContext.Session.GetString("ApiSessionCookie") is not null)
        {
            using var check=await api.SendAsync(HttpContext,HttpMethod.Get,"api/auth/me");
            if(!check.IsSuccessStatusCode){HttpContext.Session.Clear();}
        }
        if(HttpContext.Session.GetString("Role")=="Organizer")return RedirectToAction("Index","Organizer");
        if(HttpContext.Session.GetString("Role")=="Participant")return RedirectToAction("Index","Participant");
        ViewBag.Events=await api.GetAsync<List<JsonElement>>(HttpContext,"api/events")??new(); return View();
    }
}
