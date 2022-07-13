# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## First time usage

- Configure services:
```csharp  
	public void ConfigureServices(IServiceCollection services)
	{
		services.CommonEpiServices();
	}
```
- Configure app middleware:
```csharp  
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.CommonEpiserverAppBuilder();
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

After setup now classes and methods can be used by including their namespace.


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
