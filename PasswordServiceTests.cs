using RaceDay.Api.Security;
using Xunit;
namespace RaceDay.Tests;
public class PasswordServiceTests
{
 [Fact] public void Hash_IsNotPlainText_AndVerifiesCorrectPassword() { var s=new PasswordService();var hash=s.Hash("StrongPass123!");Assert.NotEqual("StrongPass123!",hash);Assert.True(s.Verify(hash,"StrongPass123!")); }
 [Fact] public void Verify_RejectsWrongPassword() { var s=new PasswordService();var hash=s.Hash("StrongPass123!");Assert.False(s.Verify(hash,"WrongPass123!")); }
}
