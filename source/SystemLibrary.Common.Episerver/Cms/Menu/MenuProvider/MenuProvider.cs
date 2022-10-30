//using System.Collections.Generic;

//using EPiServer.Shell.Navigation;

//namespace SystemLibrary.Common.Episerver.Cms.Menu;

//[MenuProvider]
//public class MenuProvider : IMenuProvider
//{
//    public MenuProvider()
//    {
//    }
//    public IEnumerable<MenuItem> GetMenuItems()
//    {
//        string showOnMenu = MenuPaths.Global + "/cms/cmsMenuItem/";

//        // add a menu at top of the 'Cms' menu, which directs to a whole new site/controller, full url or relative
//        var cmsMenu = new UrlMenuItem("Cms Button",
//            showOnMenu,
//        "https://www.systemlibrary.com")
//        {
//            IsAvailable = (request) => true
//        };

//        // section menu is 'Left navigation menu' in the main menu, where you switch between Find, Cms, etc...
//        showOnMenu = "/global/systemlibrarycommonepiserver";
//        var sectionMenu = new SectionMenuItem("SystemLibrary", showOnMenu);
//        sectionMenu.IsAvailable = ((_) => true);

//        // adds a 'a href' under 'section menu' above 
//        var firstMenuItem = new UrlMenuItem("Main", showOnMenu + "/index", "/SystemLibrary/Common/Episerver/Tools/");
//        firstMenuItem.IsAvailable = ((_) => true);

//        // add another custom tab under 'admin' which invokes 'new-admin-tab' controller
//        var newAdminTab = new UrlMenuItem("New Admin Tab",
//        MenuPaths.Global + "/cms/admin/new-tab",
//        "/EPiServer/EPiServer.Cms.UI.Admin/default#/SystemLibrary/Common/Episerver/FontAwesome/Style")
//        {
//            //EPiServer/EPiServer.Cms.UI.Admin/default#/
//            IsAvailable = (request) => true
//        };

//        return new MenuItem[] { cmsMenu, newAdminTab, sectionMenu, firstMenuItem };
//    }
//}