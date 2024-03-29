﻿# Installation
* Create a new Episerver Web Application and have it running on your local machine in no time with .NET 7!

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## Setup new episerver website - in 5 min!

0. Create new empty database named "Demo" on your local SQL instance
1. Create a new project "AspNet Core Empty" for .NET 7
2. Delete appSettings.development.json
3. Add nuget package Episerver.Cms >= 12.26.0
4. Add nuget package SystemLibrary.Common.Episerver >= 7.1.0.1
	- Note: Try compiling. If error in package versions, very often Episerver.Cms has some deps that arent updated. Simply view output in console in Visual Studio and update/add the package/version that is being complained about
5. Create '~/module.config', set its build to 'Content' 
6. Create '~/Cms/Cms.cs'
7. Create '~/Properties/launchSettings.json' (it should exist already)
8. Create '~/Content/Pages/StartPage/StartPage.cs'
9. Create '~/Content/Pages/StartPage/StartPageController.cs'
10. Create '~/Content/Pages/StartPage/Index.cshtml', set its build to 'Content'
11. Create '~/appSettings.json', set its build to 'Content' (it should exist already)
12. Copy paste code below, into their respective files

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
using System.ComponentModel.DataAnnotations;

using EPiServer.Core;
using EPiServer.DataAnnotations;

using SystemLibrary.Common.Episerver.Cms.Attributes;
using SystemLibrary.Common.Episerver.Cms.Properties;
using SystemLibrary.Common.Episerver.FontAwesome;

namespace Demo;

[ContentType(DisplayName = "Start Page", GUID = "7C3BBD03-5D83-4E75-A1E9-BCF5DF5F99B2")]
[ContentIcon(FontAwesomeSolid.house)]
public class StartPage : PageData
{
	[Display(Name = "Tip", Description = "Welcome to your brand new website")]
	public virtual Message Tip { get; set; }

	[Display(Name = "Parent", Description = "A link to the parent, where 'this' is created/living under")]
	public virtual ParentLinkReference ParentRef { get; set;}

	[Display(Name = "Expires", Description = "A date time property, but only date can be selected, time is hidden")]
	[DateSelection]
	public virtual DateTime Expires { get; set; }
}
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
		<add name="epi-cms.widgets.base" path="/SystemLibrary/CommonEpiserverCms/Edit/Style" resourceType="Style" /> 
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
		"appUrl": "https://localhost:50001",
		"edit": {
			"newContentDialogHideRequiredTitle": false,
			"allPropertiesShowPropertyDescriptions": false,
			"allPropertiesShowPropertiesAsColumns": false,
			"allPropertiesShowCheckBoxOnRightSide": false,
			"versionGadgetHideLanguageColumn": false,
			"newContentDialogItemBackgroundColor": "",
			"newContentDialogItemBorderColor": "",
			"pageTreeSelectedContentBorderColor": "",
			"projectBarActiveProjectBackgroundColor": ""
		},
		"properties": {
			"message": {
				"backgroundColor": "#fff9e6",
				"textColor": "#000"
			}
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
- Index.cshtml not found - typos in your paths/files and folders are most likely the cause or you've forgotten to copy paste something
- Forgotten to set 'build' as 'Content' on module.config/Index.cshtml
- SystemLibrary.Common.Episerver runs its initialization if, and only if, there are 0 users in the DB
  - Creates a user "demo/Demo123!" as administrator
  - Creates a new website with "domain" from launchSettings.json
  - Disables all languages except the enabled ones based on your startup configuration (Program.cs)
  - Adjusts all System Tabs, like "Settings" for properties, to be order "9000 + current", so 'Settings' is number 9030, instead of 30
    - Resulting in all our tabs are "left sided", and all episerver tabs are "on the far right" 
    - Order takes affect after a restart of IIS as the very first initialization has already loaded the orders in memory by the CMS


#### Run application
- Ctrl + F5 in Visual Studio, should start IIS Express and display an error on our startpage, as we have not created one yet
- Visit http://localhost:51010/episerver
- Log in with demo/Demo123!
- If redirected to a 404, navigate again to http://localhost:51010/episerver
- Create a StartPage in Edit Mode and publish it
- Register StartPage as the start for your site (Admin > Config > Manage Websites)
- Visit http://localhost:51010 should now give 200 OK status code

## Package Configurations
* Default (and modifiable) configurations in this package:

appSettings.json:
```json  
{
	...,
	"systemLibraryCommonEpiserver": {
		"appUrl": "https://localhost:50001",
		"edit": {
			"newContentDialogHideRequiredTitle": false,
			"allPropertiesShowPropertyDescriptions": false,
			"allPropertiesShowPropertiesAsColumns": false,
			"allPropertiesShowCheckBoxOnRightSide": false,
			"versionGadgetHideLanguageColumn": false,
			"newContentDialogItemBackgroundColor": "",
			"newContentDialogItemBorderColor": "",
			"pageTreeSelectedContentBorderColor": "",
			"projectBarActiveProjectBackgroundColor": ""
		},
		"properties": {
			"message": {
				"backgroundColor": "#fff9e6",
				"textColor": "#000"
			}
		}
	}
	...
}
```  
