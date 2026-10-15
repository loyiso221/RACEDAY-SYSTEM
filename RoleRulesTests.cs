using Xunit;
namespace RaceDay.Tests;
public class RoleRulesTests
{
 [Theory] [InlineData("Organizer",true)] [InlineData("Participant",true)] [InlineData("Administrator",false)] [InlineData("admin",false)]
 public void PublicRegistrationAllowsOnlySupportedRoles(string role,bool expected) => Assert.Equal(expected, role is "Organizer" or "Participant");
}
