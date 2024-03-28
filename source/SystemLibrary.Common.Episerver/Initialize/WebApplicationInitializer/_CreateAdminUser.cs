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

        var result = _uIUserProvider.CreateUserAsync(userName, password, email, null, null, true)
            .ConfigureAwait(true)
            .GetAwaiter()
            .GetResult();

        Log.Info("Created a new admin user");

        if (result.Status == UIUserCreateStatus.Success)
        {
            var roles = Roles.CmsRoles;

            foreach (var role in roles)
            {
                var exists = _uIRoleProvider.RoleExistsAsync(role)
                    .ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult();

                if (!exists)
                    _uIRoleProvider.CreateRoleAsync(role)
                        .ConfigureAwait(true)
                        .GetAwaiter()
                        .GetResult();
            }

            _uIRoleProvider.AddUserToRolesAsync(result.User.Username, roles)
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

        var res = _uIUserProvider.GetUserAsync(userName)
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
