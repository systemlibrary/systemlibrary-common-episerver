# Installation
* Create a new Episerver Web Application and have it running on your local machine in no time with .NET 6!

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## First time usage

0. Create new empty database named "Demo" on your local SQL instance
1. Create a new project "AspNet Core Empty" for .NET 6
2. Add EPiServer.Framework >= 12.8.0
3. Add Episerver.Cms >= 12.8.0
4. Add EPiServer.CMS.AspNetCore.Routing >= 12.8.0
5. Add SystemLibrary.Common.Episerver >= 6.0.0.2
6. Rename "Program.cs" to "Startup.cs" 
7. Create module.config at root in your web project, make sure its build is set to "Content" in your project
8. Create appSettings.json at root in your web project, make sure its build is set to "Content" in your project
9. Create Cms/Cms.cs where "Cms/" is a folder at root in your project
10. If "Properties/launchSettings.json" do not exist, create it
11. Create Content/Pages/StartPage/StartPage.cs
12. Create Content/Pages/StartPage/StartPageController.cs
13. Create Content/Pages/StartPage/Index.cshtml
14. Copy paste code below, into their respective files

Cms/Cms.cs
```csharp 
using SystemLibrary.Common.Episerver;

namespace Demo;

public class Cms : BaseCms
{
}
```

Content/Pages/StartPage/StartPage.cs
```csharp 
using EPiServer.Core;
using EPiServer.DataAnnotations;

using SystemLibrary.Common.Episerver.Attributes;

namespace Demo;

[ContentType(DisplayName = "Start Page", GUID = "7C3BBD03-5D83-4E75-A1E9-BCF5DF5F99B2")]
[ContentIcon(FontAwesomeSolid.house)]
public class StartPage : PageData { }
```

Content/Pages/StartPage/StartPageController.cs
```csharp 
using EPiServer.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

namespace Demo;

public class StartPageController : PageController<StartPage>
{
	public ActionResult Index(StartPage currentContent)
	{
		return View();
	}
}
```

Content/Pages/StartPage/Index.cshtml
```csharp 
@model object
Startpage
```


module.config:
```xml 
<?xml version="1.0" encoding="utf-8"?>
<module>
	<clientResources>
		<add name="epi-cms.widgets.base" path="/SystemLibrary/Common/Episerver/FontAwesome/Stylesheet" resourceType="Style" />
		<add name="epi-cms.widgets.base" path="/SystemLibrary/Common/Episerver/CmsEdit/Stylesheet" resourceType="Style" /> 
	</clientResources>
</module>
```
 
Startup.cs
```csharp 
using EPiServer.Cms.TinyMce;

using SystemLibrary.Common.Episerver;
using SystemLibrary.Common.Episerver.Extensions;

namespace Demo;

public class Startup
{
	public static void Main(string[] args)
	{
		var appSettingsPath = AppContext.BaseDirectory + "appSettings.json";
		try
		{
			Cms.CreateHostBuilder<Startup>(args, appSettingsPath)
				.Build()
				.Run();
		}
		catch (Exception ex)
		{
			Dump.Write(ex);
		}
	}

	public void ConfigureServices(IServiceCollection services)
	{
		var options = new CommonEpiserverApplicationServicesOptions();

		options.InitialLanguagesEnabled = "no";

		services.CommonEpiserverApplicationServices<CurrentUser>(options)
			.AddCms()
			.AddTinyMce();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		var options = new CommonEpiserverApplicationBuilderOptions();
		app.CommonEpiserverApplicationBuilder(options);
	}
}
```

appSettings.json:
```json 
{
	"ConnectionStrings": {
		"EPiServerDB": "Data Source=.\\sqlexpress;Initial Catalog=Demo;Connection Timeout=10;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=true;"
	},
	"systemLibraryCommonWeb": {
		"log": {
		"isEnabled": true,
		"level": "Info"
		}
	},

	"systemLibraryCommonEpiserver": {
		"cmsEdit": {
			"enabled": true,
			"hideLanguageColumnInVersionGadget": false,
			"contentCreationBackgroundColor": "",
			"contentCreationBorderColor": "",
			"pageTreeSelectedContentBorderColor": "",
			"contentTitleColor": "",
			"activeProjectBarBackgroundColor": "",
			"messagePropertyBackgroundColor": ""
		}
	}
}
```

Properties/launchSettings.json
```json 
{
	"iisSettings": {
		"windowsAuthentication": false,
		"anonymousAuthentication": true,
		"iisExpress": {
			"applicationUrl": "http://localhost:51010/",
			"sslPort": 0
		}
	},
	
	"profiles": {
		"Demo (local IISExpress)": {
			"commandName": "IISExpress",
			"launchBrowser": true
		}
	}
}
```

#### Final Notes Before Running Application
- EpiserverDb connection string targets SqlExpress instance with db name "Demo" with your "Windows Credentials"
- Episerver database tables is only created if the database is empty
- Episerver is only initialized with languages, sites and a new admin user, if there's no users already existing in the DB
- If you run this without changing domain "localhost" in launchSettings, then sites in Admin is of course configured against localhost

#### Run application
- Ctrl + F5 in Visual Studio, start site in IIS Express
- Visit http://localhost:51010/episerver

## Package Configurations
* Default (and modifiable) configurations in this package:

appSettings.json:
```json  
{
	...,
	"systemLibraryCommonEpiserver": {
		"cmsEdit": {
			"enabled": true,
			"hideLanguageColumnInVersionGadget": false,
			"contentCreationBackgroundColor": "",
			"contentCreationBorderColor": "",
			"pageTreeSelectedContentBorderColor": "",
			"contentTitleColor": "",
			"activeProjectBarBackgroundColor": "",
			"messagePropertyBackgroundColor": ""
		}
	},
	...
}
```  
