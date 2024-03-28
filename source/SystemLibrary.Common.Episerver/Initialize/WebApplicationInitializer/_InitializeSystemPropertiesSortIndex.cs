namespace SystemLibrary.Common.Episerver;

partial class WebApplicationInitializer
{
    static void InitializeSystemPropertiesSortIndex()
    {
        var q = @"UPDATE TOP(10) tblPropertyDefinitionGroup 
                        SET GroupOrder = GroupOrder + 9000 
                        WHERE SystemGroup = 'True' 
                        AND GroupOrder < 10000 
                        AND GroupOrder > 9";

        ExecuteQuery(q);
    }
}
