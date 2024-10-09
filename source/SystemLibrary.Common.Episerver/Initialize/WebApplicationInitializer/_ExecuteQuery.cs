using Microsoft.Data.SqlClient;

namespace SystemLibrary.Common.Episerver;

partial class WebApplicationInitializer
{
    /// <summary>
    /// Returns 1 if rows were selected/adjusted
    /// Returns 0 if no rows were adjusted/selected,
    /// Returns -1 on error
    /// </summary>
    static int ExecuteQuery(string q)
    {
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
