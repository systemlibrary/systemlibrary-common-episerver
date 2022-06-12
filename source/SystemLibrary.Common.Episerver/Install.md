# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Episerver

## First time usage

- Classes and methods can be used out of the box by including the namespace they live in

- Sample:
```csharp  
	public void ConfigureServices(IServiceCollection services)
	{
		services.CommonEpiServices(); //Extension method is inside this package
	}
```

## Package Configurations
* Default (and modifiable) configurations in this package:

appSettings.json:
```json  
	{
		"systemLibraryCommonEpiserver": {
		}
	}
```  
