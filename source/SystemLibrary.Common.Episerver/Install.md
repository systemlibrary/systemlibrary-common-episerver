# Installation
* Create a new Episerver Web Application and have it running on your local machine in no time with .NET 6!

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## First time usage

0. Create new empty database named "Demo" on your local SQL instance
1. Create a new project "AspNet Core Empty" for .NET 6
2. Add nuget package EPiServer.Framework >= 12.8.0 
3. Add nuget package Episerver.Cms >= 12.8.0
4. Add nuget package EPiServer.CMS.AspNetCore.Routing >= 12.8.0
5. Add nuget package SystemLibrary.Common.Episerver >= 12.2.0.1
6. Create '~/module.config', set its build to 'Content' 
7. Create '~/Cms/Cms.cs'
8. Create '~/Properties/launchSettings.json'
9. Create '~/Content/Pages/StartPage/StartPage.cs'
10. Create '~/Content/Pages/StartPage/StartPageController.cs'
11. Create '~/Content/Pages/StartPage/Index.cshtml', set its build to 'Content'
12. Create '~/appSettings.json', set its build to 'Content'
13. Copy paste code below, into their respective files

~/Cms/Cms.cs:
```csharp 
using SystemLibrary.Common.Episerver;

namespace Demo;

public class Cms : BaseCms
{
}
```

~/Content/Pages/StartPage/StartPage.cs
```csharp 
using EPiServer.Core;
using EPiServer.DataAnnotations;

using SystemLibrary.Common.Episerver.Cms.Attributes;
using SystemLibrary.Common.Episerver.FontAwesome;

namespace Demo;

[ContentType(DisplayName = "Start Page", GUID = "7C3BBD03-5D83-4E75-A1E9-BCF5DF5F99B2")]
[ContentIcon(FontAwesomeSolid.house)]
public class StartPage : PageData { }
```

~/Content/Pages/StartPage/StartPageController.cs
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

~/Content/Pages/StartPage/Index.cshtml
```csharp 
@model object
Startpage
```


~/module.config:
```xml 
<?xml version="1.0" encoding="utf-8"?>
<module>
	<clientResources>
		<add name="epi-cms.widgets.base" path="/SystemLibrary/Common/Episerver/FontAwesome/Stylesheet" resourceType="Style" />
		<add name="epi-cms.widgets.base" path="/SystemLibrary/Common/Episerver/CmsEdit/Stylesheet" resourceType="Style" /> 
	</clientResources>
</module>
```
 
~/Program.cs
```csharp 
using EPiServer.Cms.TinyMce;

using SystemLibrary.Common.Episerver;
using SystemLibrary.Common.Episerver.Extensions;

namespace Demo;

public class Program
{
	public static void Main(string[] args)
	{
		var appSettingsPath = AppContext.BaseDirectory + "appSettings.json";
		try
		{
			Cms.CreateHostBuilder<Program>(args, appSettingsPath)
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

~/appSettings.json:
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

~/Properties/launchSettings.json
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

#### Pit falls
- EpiserverDb connection string targets SqlExpress instance and the database named "Demo" with your "Windows Credentials"
- SystemLibrary.Common.Episerver initializes langauges and sites also, but only if user count is 0
- SystemLibrary.Common.Episerver only creates the "demo" user if the database do not have a user already, user count must be 0
- SystemLibrary.Common.Episerver registers "default site" as the domain in "launchSettings" file, which is "localhost"
- Index.cshtml not found - typos in your paths/files and folders are most likely the cause or you've forgotten to copy paste something


#### Run application
- Ctrl + F5 in Visual Studio, should start IIS Express and display an error on our startpage, as we have not created one yet
- Visit http://localhost:51010/episerver
- Log in with demo/Demo123!
- Create a StartPage in Edit Mode
- Register StartPage as the start for your site (Admin > Config > Manage Websites)
- Visit http://localhost:51010 should now give 200 OK status code

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
