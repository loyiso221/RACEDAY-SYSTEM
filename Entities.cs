namespace RaceDay.Api.Models;
public class Role { public int RoleID {get;set;} public string RoleName {get;set;} = ""; public string? Description {get;set;} }
public class User {
 public int UserID {get;set;} public int RoleID {get;set;} public Role? Role {get;set;}
 public string FirstName {get;set;} = ""; public string LastName {get;set;} = "";
 public string Email {get;set;} = ""; public string PasswordHash {get;set;} = "";
 public string? PhoneNumber {get;set;} public DateTime CreatedAt {get;set;} public bool IsActive {get;set;} = true;
}
public class RaceEvent {
 public int EventID {get;set;} public int OrganizerID {get;set;} public User? Organizer {get;set;}
 public string EventName {get;set;} = ""; public string? Description {get;set;} public DateTime EventDate {get;set;}
 public string Location {get;set;} = ""; public string Status {get;set;} = "Upcoming"; public DateTime CreatedAt {get;set;}
}
public class Category {
 public int CategoryID {get;set;} public int EventID {get;set;} public RaceEvent? Event {get;set;}
 public string CategoryName {get;set;} = ""; public decimal DistanceKM {get;set;} public int MinimumAge {get;set;}
 public int? MaximumAge {get;set;} public decimal EntryFee {get;set;}
}
public class EventEnrollment {
 public int EnrollmentID {get;set;} public int ParticipantID {get;set;} public User? Participant {get;set;}
 public int CategoryID {get;set;} public Category? Category {get;set;} public DateTime EnrollmentDate {get;set;}
 public string EnrollmentStatus {get;set;} = "Confirmed"; public string? BibNumber {get;set;}
}
public class RaceResult {
 public int ResultID {get;set;} public int EnrollmentID {get;set;} public EventEnrollment? Enrollment {get;set;}
 public int? FinishTimeSeconds {get;set;} public int? Position {get;set;} public string ResultStatus {get;set;} = "Finished";
 public DateTime RecordedAt {get;set;}
}
