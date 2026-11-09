using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using System.Text.Json;
namespace RaceDay.Web.Controllers;
public class ParticipantController(RaceDayApiClient api) : Controller
{
    private bool Allowed()=>HttpContext.Session.GetString("Role")=="Participant";
    public async Task<IActionResult> Index(){if(!Allowed())return RedirectToAction("Login","Account");var all=await api.GetAsync<List<JsonElement>>(HttpContext,"api/events")??new(); ViewBag.Events=all.Where(e=>e.TryGetProperty("eventDate",out var date)&&DateTime.TryParse(date.ToString(),out var dt)&&dt.Date>=DateTime.Today&&(!e.TryGetProperty("status",out var status)||status.ToString()!="Cancelled")).ToList();return View();}
    public async Task<IActionResult> Details(int id){ViewBag.Event=await api.GetAsync<JsonElement>(HttpContext,$"api/events/{id}");ViewBag.Categories=await api.GetAsync<List<JsonElement>>(HttpContext,$"api/events/{id}/categories")??new();return View();}
    [HttpPost][ValidateAntiForgeryToken] public async Task<IActionResult> Enroll(int categoryId){if(!Allowed())return Forbid();using var r=await api.JsonAsync(HttpContext,HttpMethod.Post,"api/enrollments",new{categoryID=categoryId});TempData[r.IsSuccessStatusCode?"Success":"Error"]=r.IsSuccessStatusCode?"You have enrolled successfully.":"Enrolment failed. You may already be enrolled or registration may be closed.";return RedirectToAction(nameof(MyEnrollments));}
    public async Task<IActionResult> MyEnrollments(){if(!Allowed())return Forbid();ViewBag.Rows=await api.GetAsync<List<JsonElement>>(HttpContext,"api/enrollments/mine")??new();return View();}
    public async Task<IActionResult> MyResults(){if(!Allowed())return Forbid();ViewBag.Rows=await api.GetAsync<List<JsonElement>>(HttpContext,"api/results/mine")??new();return View();}
}
