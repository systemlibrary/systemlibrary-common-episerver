# Installation
* Create a new Episerver Web Application and have it running on your local machine in no time with >= .NET 7

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## Download demo
Download a demo of the application parts of the setup below:
[Download Demo.zip](https://systemlibrary.github.io/systemlibrary-common-episerver/demo.zip)


## Setup new episerver website in 10 minutes
0. Create new empty database named "Demo" on your local SQL instance
1. Create a new project "AspNet Core Empty" for >= .NET 7
2. Delete appSettings.development.json
3. Add nuget package Episerver.Cms >= 12.26.0
	- Last tested version is 12.26.0 which was fine
4. Add nuget package SystemLibrary.Common.Episerver >= 7.13.0.3
	- Note: Try compiling. If error in package versions, very often Episerver.Cms has some deps that arent updated. Simply view output in console in Visual Studio and update/add the package/version that is being complained about
5. Create '~/module.config', set its build to 'Content' 
6. Create '~/Initialize/LogWriter/LogWriter.cs'
7. Create '~/Cms/Cms.cs'
8. Create '~/Properties/launchSettings.json' (it should exist already)
9. Create '~/Content/Pages/StartPage/StartPage.cs'
10. Create '~/Content/Pages/ErrorPage/ErrorPage.cs'
11. Create '~/Content/Pages/StartPage/StartPageController.cs'
12. Create '~/Content/Pages/ErrorPage/ErrorPageController.cs'
13. Create '~/Content/Pages/StartPage/Index.cshtml', set its build to 'Content'
14. Create '~/Content/Pages/ErrorPage/Index.cshtml', set its build to 'Content'
15. Create '~/appSettings.json', set its build to 'Content' (it should exist already)
16. Create '~/Content/Pages/_ViewStart.cshtml'
16. Create '~/Content/Pages/_PageLayout.cshtml'
17. Create '~/Content/_ViewImports.cshtml'
18. Create '~/Views/_ViewImports.cshtml'
19. Create '~/Views/MainMenu.cshtml'
20. Copy paste code below, into their respective files


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

using SystemLibrary.Common.Episerver.Attributes;
using SystemLibrary.Common.Episerver.Properties;
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

~/Content/Pages/ErrorPage/ErrorPage.cs
```csharp
using EPiServer.Core;
using EPiServer.DataAnnotations;

using SystemLibrary.Common.Episerver;
using SystemLibrary.Common.Episerver.Attributes;

namespace Demo.Content.Pages;

[ContentType(DisplayName = "Error Page", Description = "Error", GUID = "10A8191B-C9AE-433A-860B-7B7EEE758A3C", GroupName = "Single Pages")]
[ContentIcon(SystemLibrary.Common.Episerver.FontAwesome.FontAwesomeSolid.stop)]
public class ErrorPage : PageData, IErrorPage
{
	public virtual IList<int> StatusCodes { get; set; }
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

~/Content/Pages/ErrorPage/ErrorPageController.cs
```csharp 
using EPiServer.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

namespace Demo.Content.Pages;

public class ErrorPageController : PageController<ErrorPage>
{
	public ViewResult Index(ErrorPage currentPage, string path)
	{
		return View("Index", currentPage);
	}
}
```

~/Content/Pages/StartPage/Index.cshtml
```csharp 
@model object
Startpage
```

~/Content/Pages/ErrorPage/Index.cshtml
```csharp 
@{ Layout = null; }
@model object
<h1>Error page</h1>
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

~/Initialize/LogWriter/LogWriter.cs
```csharp 
using SystemLibrary.Common.Web;

public class LogWriter : ILogWriter
{
	public void Write(string message)
	{
		Dump.Write(message);
	}
	public void Error(string message)
	{
		Dump.Write(message);
	}
	public void Warning(string message)
	{
		Dump.Write(message);
	}
	public void Debug(string message)
	{
		Dump.Write(message);
	}
	public void Information(string message)
	{
		Dump.Write(message);
	}
	public void Trace(string message)
	{
		Dump.Write(message);
	}
}
```
 
~/Program.cs
```csharp 
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
			Log.Error(ex);
		}
	}

	public void ConfigureServices(IServiceCollection services)
	{
		var options = new CmsServicesCollectionOptions();

		options.InitialLanguagesEnabled = "no";

		services.AddCommonCmsServices<CurrentUser, LogWriter>(options);
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		var options = new CmsAppBuilderOptions();

		options.UseHttpsRedirection = false;

		app.UseCommonCmsApp(env, options);
	}
}
```

~/Content/Pages/_PageLayout.cshtml
```xml 
@model object
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1" />
</head>
<body style="color: green;">
	@try
	{
		@await Html.PartialAsync("~/Views/MainMenu.cshtml");
	}
	catch (Exception ex)
	{
		@Html.ViewException((Model as PageData, ex))
	}

	@try
	{
		@RenderBody()
	}
	catch (Exception ex)
	{
		@Html.ViewException((Model as PageData, ex))
	}
</body>
</html>
```

~/Views/_ViewImports_.cshtml
```xml 
@using Demo;
@using Demo.Content.Pages;
@using EPiServer;
@using EPiServer.Core;
@using EPiServer.Web.Mvc.Html;
@using React.AspNet;
@using System.Diagnostics;
@using SystemLibrary.Common.Net;
@using SystemLibrary.Common.Web;
@using SystemLibrary.Common.Episerver.Extensions;

@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

~/Content/_ViewImports_.cshtml
```csharp 
@using Demo;
@using Demo.Content.Pages;
@using EPiServer;
@using EPiServer.Core;
@using EPiServer.Web.Mvc.Html;
@using React.AspNet;
@using System.Diagnostics;
@using SystemLibrary.Common.Net;
@using SystemLibrary.Common.Web;
@using SystemLibrary.Common.Episerver.Extensions;

@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

~/Content/Pages/_ViewStart.cshtml
```csharp 
@{ Layout = "~/Content/Pages/_PageLayout.cshtml"; }
```

~/Views/MainMenu.cshtml
```xml 
<h3>Main Menu</h3>
```

~/appSettings.json:
```json 
{
	"ConnectionStrings": {
		"EPiServerDB": "Data Source=.\\sqlexpress;Initial Catalog=Demo;Connection Timeout=10;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=true;"
	},

	"systemLibraryCommonNet": {
		"dump": {
			"folder": "%HomeDrive%/Logs/",
			"fileName": "DumpWrite.log",
		}
	},

	"systemLibraryCommonWeb": {
		"log": {
			"level": "Information"
		},

		"cache": {
			"duration": 180,
			"fallbackDuration": 600,
			"containerSizeLimit": 60000
		},
		
		"client": {
			"timeout": 40001,
			"retryTimeout": 10000,
			"ignoreSslErrors": true,
			"useRetryPolicy": true,
			"throwOnUnsuccessful": true,
			"useRequestBreakerPolicy": false,
			"clientCacheDuration": 1200
		},
	},
	
	"systemLibraryCommonEpiserver": {
		"edit": {
			"newContentDialogHideRequiredTitle": false,
			"allPropertiesScrollableDocumentHeader": false,
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
- Index.cshtml not found - typos in your paths/files and folders are most likely the cause
- Forgotten to set 'build' as 'Content' on module.config/Index.cshtml
- SystemLibrary.Common.Episerver runs its initialization if, and only if, there are 0 users in the DB
  - Creates a user "demo/Demo123!" as administrator
  - Creates a new website with "domain" from launchSettings.json
  - Disables all languages except the enabled ones based on your startup configuration (Program.cs)
  - Adjusts all System Tabs, like "Settings" for properties, to be order "9000 + current", so 'Settings' is number 9030, instead of 30
	- Resulting in all our tabs are "left sided", and all episerver tabs are "on the far right" 
	- Order takes affect after a restart of IIS as the very first initialization has already loaded the orders in memory by the CMS
- SystemLibrary.Common.Web automatically redirect http to https by default, remember to set it off in the option in program.cs

#### Run application
- Ctrl + F5 in Visual Studio, should start IIS Express and display an error on our startpage, as we have not created one yet
- Visit http://localhost:51010/episerver/cms
- Log in with demo/Demo123!
- Redirects possibly to 'start page', which still gives 404, navigate again to: http://localhost:51010/episerver/cms
- Create a StartPage in Edit Mode and publish it
- Register StartPage as the start for your site (Admin > Config > Manage Websites)
- Visit http://localhost:51010/ should now give 200 OK status code
- Go to http://localhost:51010/episerver/cms
- Create a Error Page, set status code to 404, and publish the page
- Visit http://localhost:51010/do-not-exist should show the Error Page you've published

## Package Configurations
* Below are the default and modifiable configurations for this package

###### appSettings.json:
```json  
{
	...,
	"systemLibraryCommonEpiserver": {
		"debug": false,
		
		"edit": {
			"newContentDialogHideRequiredTitle": false,
			"allPropertiesScrollableDocumentHeader": false,
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
