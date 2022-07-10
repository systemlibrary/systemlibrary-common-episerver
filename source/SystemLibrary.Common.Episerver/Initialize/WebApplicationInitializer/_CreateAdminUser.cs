using System;
using System.Threading.Tasks;

using EPiServer.Shell.Security;

using Microsoft.Data.SqlClient;

namespace SystemLibrary.Common.Episerver.Initialize
{
    partial class WebApplicationInitializer
    {
        async Task CreateAdminUser()
        {
            var email = ServiceCollectionExtensions.Options.DefaultAdminEmail;
            var password = ServiceCollectionExtensions.Options.DefaultAdminPassword;
            var userName = email.Substring(0, email.IndexOf('@'));

            var result = await _uIUserProvider.CreateUserAsync(userName, password, email, null, null, true);

            if (result.Status == UIUserCreateStatus.Success)
            {
                var roles = Roles.CmsRoles;

                foreach (var role in roles)
                {
                    var exists = await _uIRoleProvider.RoleExistsAsync(role);
                    if (!exists)
                        await _uIRoleProvider.CreateRoleAsync(role);
                }

                await _uIRoleProvider.AddUserToRolesAsync(result.User.Username, roles);
            }
            else
            {
                Log.Error("Result creating admin was: " + result.Status);
            }
        }

        bool IsAnyUserOrAdminAlreadyRegistered()
        {
            if(NoUsersExistsInDb() > 0)
                return true;

            var email = ServiceCollectionExtensions.Options.DefaultAdminEmail;

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

            try
            {
                var constring = AppSettings.Current.ConnectionStrings.EPiServerDB;
                int result = 0;
                using (var sqlcon = new SqlConnection(constring))
                {
                    sqlcon.Open();
                    using (var sqlcmd = new SqlCommand(q, sqlcon))
                    {
                        using (var sqlReader = sqlcmd.ExecuteReader())
                        {
                            if (sqlReader.HasRows)
                                result = 1;
                        }
                    }
                    sqlcon.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return -1;
            }
        }
    }
}
