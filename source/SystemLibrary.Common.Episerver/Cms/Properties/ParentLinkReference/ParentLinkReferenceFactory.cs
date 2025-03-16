using EPiServer.Cms.Shell.Extensions;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

using Microsoft.Data.SqlClient;

using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Episerver.Attributes;

public class ParentLinkReferenceFactory : ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        var parentId = 0;

        try
        {
            if (metadata?.EditorConfiguration?.ContainsKey("parentLinkReferenceId") == true)
                metadata.EditorConfiguration.Remove("parentLinkReferenceId");

            var owner = metadata?.FindOwnerContent();

            if (owner != null && owner is not PageData)
            {
                var parentLink = owner?.ParentLink;

                parentId = parentLink?.ID ?? 0;

                if (parentLink?.WorkID == 0)
                {
                    var cacheKey = nameof(ParentLinkReferenceFactory) + "GetSelections" + "ParentId" + parentLink.ID + "#" + parentLink.WorkID + "#" + owner.ContentLink?.ID;
                    var cached = Cache.Get<object>(cacheKey);
                    if (cached == null)
                    {
                        var query = GetQuery(parentId);

                        parentId = ExecuteQuery(query);

                        Cache.Set(cacheKey, parentId, CacheDuration.M);
                    }
                    else
                    {
                        parentId = (int)cached;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        metadata.EditorConfiguration.TryAdd("allPropertiesShowPropertiesAsColumns", AppSettings.Current.Edit.AllPropertiesShowPropertiesAsColumns);
        metadata.EditorConfiguration.TryAdd("parentLinkReferenceId", parentId);

        return items;
    }

    static string GetQuery(int parentId)
    {
        var query = @"DECLARE @pkId INT
        DECLARE @parentId INT

        SET @parentId = 0
        SET @pkId = 0

        IF @pkId = 0 BEGIN
	        SET @pkID = (SELECT TOP(1) fkChildID FROM tblTree
	        WHERE fkChildID = " + parentId + @" 
	        AND fkParentID = 4
	        AND NestingLevel = 1)
        END

        IF @pkId > 0 BEGIN
	        DECLARE @guid NVARCHAR(64)
	        SET @guid = (SELECT TOP(1) ContentGUID FROM tblContent WHERE pkID = @pkID)
	
	        IF @guid IS NOT NULL AND LEN(@guid) >= 32 BEGIN
		        SET @parentId = (SELECT TOP(1) pkID FROM tblPage WHERE ContentAssetsID = @guid)
	        END	
        END

        SELECT @parentId as 'ID'";

        return query;
    }

    static int ExecuteQuery(string query)
    {
        var result = 0;

        try
        {
            var constring = AppSettings.Current.ConnectionStrings.EPiServerDB;
            using (var sqlcon = new SqlConnection(constring))
            {
                sqlcon.Open();
                using (var sqlcmd = new SqlCommand(query, sqlcon))
                {
                    using (var sqlReader = sqlcmd.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            result = Convert.ToInt32(sqlReader.GetValue(0));
                            break;
                        }
                    }
                }
                sqlcon.Close();
            }
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        return result;
    }
}

