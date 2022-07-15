# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## First time usage

- This assumes you know how to host a .NET Core Web Application and run it just fine...

- Initialize your episerver application with a default set of services (classes, injectable) and middlewares (pipeline, classes runs in order they are registered):
 
```csharp 
using SystemLibrary.Common.Episerver.Extensions;

public class Initialize 
{
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		var options = new EpiserverWebApplicationOptions();
		app.CommonEpiserverAppBuilder(options);
	}
	
	public void ConfigureServices(IServiceCollection services)
	{
		var options = new ServiceCollectionEpiserverOptions();
		services.CommonEpiserverServices(options);
	}
}
```

Then inside your program.cs (main method) use the 'Initialize' class
```csharp 
static void main(string[] args) {
	Host.CreateDefaultBuilder(args)
		.ConfigureWebHostDefaults(config =>
		{
			config.UseStartup<Initialize>();
		})
		//other options...
		.Build()
		.Run();
}
```

- Create module.config at root if not existing, and add endpoints:
```csharp  
<?xml version="1.0" encoding="utf-8"?>
<module>
	<clientResources>
		<add name="epi-cms.widgets.base" path="/SystemLibrary/Common/Episerver/ContentIconAttribute/FontAwesome" resourceType="Style"/>
		<add name="epi-cms.widgets.base" path="/SystemLibrary/Common/Episerver/CmsEditor/Styles" resourceType="Style"/>
	</clientResources>
</module>
```

- Remember to add EpiserverDb into your appSettings.json:
```json 
{
	...,
	"ConnectionStrings": {
		"EPiServerDB": "Data Source=..."
	},
	...
}
```

- If you now run your .NET 6 Web Application against an empty database, it should create all Episerver DB tables, create a 'demo' administrator with password 'Demo123!', so visit http://yoursite:yourPort/episerver


## Package Configurations
* Default (and modifiable) configurations in this package:

appSettings.json:
```json  
	{
		...,
		"systemLibraryCommonEpiserver": {
			"logMessageBuilder": {
				"appendLoggedInState": true,
				"appendCurrentPage": true,
				"appendCurrentUrl": true,
				"appendIp": true,
				"appendBrowser": true,
				"appendCookieInfo": true
			},
			"cache": {
				"defaultDuration": 180
			},
			"editMode": {
				"companyColor": "#B84D94" //A css color or hex
			}
		},
		...
	}
```  
