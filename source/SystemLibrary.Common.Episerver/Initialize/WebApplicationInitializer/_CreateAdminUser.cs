using System.Threading.Tasks;

using EPiServer.Shell.Security;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver;

partial class WebApplicationInitializer
{
    Task CreateAdminUser()
    {
        var email = IServiceCollectionExtensions.Options.DefaultAdminEmail;
        var password = IServiceCollectionExtensions.Options.DefaultAdminPassword;
        var userName = email.Substring(0, email.IndexOf('@'));

        var result = uIUserProvider.CreateUserAsync(userName, password, email, null, null, true)
            .ConfigureAwait(true)
            .GetAwaiter()
            .GetResult();

        Log.Information("Created a new admin user");

        if (result.Status == UIUserCreateStatus.Success)
        {
            var roles = Roles.CmsRoles;

            foreach (var role in roles)
            {
                var exists = uIRoleProvider.RoleExistsAsync(role)
                    .ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult();

                if (!exists)
                    uIRoleProvider.CreateRoleAsync(role)
                        .ConfigureAwait(true)
                        .GetAwaiter()
                        .GetResult();
            }

            uIRoleProvider.AddUserToRolesAsync(result.User.Username, roles)
                .ConfigureAwait(true)
                .GetAwaiter()
                .GetResult();
        }
        else
        {
            Log.Error("Creating admin failed, status: " + result.Status);
        }
        return Task.FromResult(0);
    }

    bool IsAnyUserAlreadyRegisteredInDatabase()
    {
        if(NoUsersExistsInDb() > 0)
            return true;

        var email = IServiceCollectionExtensions.Options.DefaultAdminEmail;

        var userName = email.Substring(0, email.IndexOf('@'));

        var res = uIUserProvider.GetUserAsync(userName)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        return res?.Username != null;
    }

    int NoUsersExistsInDb()
    {
        var q = "SELECT TOP (1) 1 FROM AspNetUsers;";

        return ExecuteQuery(q);
    }
}
