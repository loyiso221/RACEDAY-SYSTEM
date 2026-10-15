using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public record RegisterRequest([Required,MaxLength(100)] string FirstName,[Required,MaxLength(100)] string LastName,
 [Required,EmailAddress,MaxLength(150)] string Email,[Required,MinLength(8)] string Password,
 [Required] string Role,[MaxLength(20)] string? PhoneNumber);
public record LoginRequest([Required,EmailAddress] string Email,[Required] string Password);
public record ProfileUpdateRequest([Required,MaxLength(100)] string FirstName,[Required,MaxLength(100)] string LastName,[MaxLength(20)] string? PhoneNumber);
public record EventRequest([Required,MaxLength(150)] string EventName,[MaxLength(500)] string? Description,
 DateTime EventDate,[Required,MaxLength(200)] string Location,string Status = "Open");
public record CategoryRequest([Required,MaxLength(100)] string CategoryName,decimal DistanceKM,int MinimumAge,int? MaximumAge,decimal EntryFee);
public record EnrollRequest(int CategoryID);
public record ResultRequest(int? FinishTimeSeconds,int? Position,string ResultStatus = "Finished");
