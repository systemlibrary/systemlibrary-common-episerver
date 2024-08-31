using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Users;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class CurrentUserTests
{
    [TestMethod]
    public void CurrentUser_Flags_And_Role_Check_Do_Not_Throw()
    {
        var user = new CurrentUser();

        var result = user.IsCmsUser || user.IsAdministrator || user.IsAuthenticated || user.IsApproved || user.IsLockedOut || user.IsInRole("Hello world");
        
        Assert.IsFalse(result, "Result is true, expected false");
    }

    [TestMethod]
    public void CurrentUser_String_Properties_Does_Not_Throw()
    {
        var user = new CurrentUser();

        var result = user.Name + user.GivenName + user.Surname + user.PhoneNumber + user.PhoneNumberConfirmed + user.Email + user.EmailConfirmed;
        
        Assert.IsTrue(result == "FalseFalse", "Result got data " + result);
    }
}
