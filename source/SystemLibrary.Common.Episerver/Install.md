# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## First time usage
- Setup your episerver web application with the most common settings targetting .NET >= 6

0. Create new empty database named "Demo" on your local SQL instance
1. Create a new project "AspNet Core Empty" for .NET 6 project
2. Add EPiServer.Framework >= 12.8.0
3. Add Episerver.Cms >= 12.8.0
4. Add EPiServer.CMS.AspNetCore.Routing >= 12.8.0
5. Add SystemLibrary.Common.Episerver >= 6.0.0.2
6. Rename "Program.cs" to "Startup.cs" 
7. Create module.config at root in your web project, make sure its build is set to "Content" in your project
8. Create appSettings.json at root in your web project, make sure its build is set to "Content" in your project
9. Create Cms/Cms.cs where "Cms/" is a folder at root in your project
10. If "Properties/launchSettings.json" do not exist, create it
11. Copy paste code below, into their respective files
12. Note: You might need to change EpiserverDb connection string, if youre not running "SqlExpress" as instance name

Cms/Cms.cs
```csharp 
using SystemLibrary.Common.Episerver;

namespace Demo;

public class Cms : BaseCms
{
}
```

module.config:
```xml 
<?xml version="1.0" encoding="utf-8"?>
<module>
	<clientResources>
		<add name="epi-cms.widgets.base" path="/SystemLibrary/Common/Episerver/ContentIconAttribute/FontAwesome" resourceType="Style" />
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
			"hideLanguageColumnInVersionGadget": false,
			"contentCreationBackgroundColor": "",
			"contentCreationBorderColor": "",
			"pageTreeSelectedContentBorderColor": "",
			"contentTitleColor": "",
			"activeProjectBarBackgroundColor": ""
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

#### Run application
- Ctrl + F5 inside Visual Studio, should now run your application in IIS express and initialize the database with all episerver tables and a new user: demo/Demo123!
- Note: Initialization of Epi only occurs if the database is empty, and initialization of user, site and languages,  only occurs if there's no user already existing in the DB (aspnetuser db-table)
- Visit http://localhost:51010/episerver and login screen appears
- Note: "episerver.demo" requires hosts file change on Windows
- Note2: "episerver.demo" is defined in "Properties\launchSettings" if you want to change it

## Package Configurations
* Default (and modifiable) configurations in this package:

appSettings.json:
```json  
{
	...,
	"systemLibraryCommonEpiserver": {
		"cmsEdit": {
			"hideLanguageColumnInVersionGadget": true,
			"contentCreationBackgroundColor": "",
			"contentCreationBorderColor": "",
			"pageTreeSelectedContentBorderColor": "",
			"contentTitleColor": "",
			"activeProjectBarBackgroundColor": ""
		}
	},
	...
}
```  
