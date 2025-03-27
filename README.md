# SystemLibrary Common Episerver

## Description
Library with classes, methods and dijits for every &gt;= .NET 8 episerver application

### Features
##### BoxSelection
![Box Selection Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-boxselection.png "")
- Supports showing any free FontAwesome Icon
- Supports showing your own custom images
- Supports setting background color, to be used as a color picker  

##### DateSelection
![Date Selection Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-dateselection.png "")
- Supports showing DateTime picker as 'Date'
- Default style is overridden for the DateTime picker

##### MultidropdownSelection
![Box Selection Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-multidropdownselection.png "")
- Supports adding one or more items from a dropdownlist (enum)
- Optionally supports adding free text to an IList of strings

##### Message
![Message Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-message.png "")
- Display a simple help text to the editors
- Has an built-in toggle functionality that activates if text to display is large  

##### ContentIcon
![Content Icon Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-contenticon.png "")
- Shows icons in both Page Tree and the 'New Content' dialog
- Supports choosing any of the free FontAwesome Icons
- Supports your own custom images  

##### Parent Link Reference
![Parent Link Reference Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-parentlinkreference.png "")
- Creates a link to the parent, where 'this' content is stored
- No more wondering where content is stored, you have a link to it now  

##### JsonEdit
![Json Edit Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsonedit.png "")
![Json Edit Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsoneditor-view.png "")
- A simple json editor for simple objects
- Data is stored as string, so you invoke the StringExtension .Json() to get it as a C# class or .JsonEditAsObject() if class contains XhtmlString fields for rich text editable text
- Contains ways to add placeholders to input fields, required message to each field, and a displayName for each property  

##### One Line Setup
* Setup IApplicationBuilder in one line:  
    app.UseCommonCmsApp();
    - registers common middlewares for Optimizely CMS, like cache, authentication, cookies, routing to controllers and more
* Setup IServiceCollection in one line:  
    services.AddCommonCmsServices&lt;CurrentUser, LogWriter&gt;().AddFind();
    - registers common services for Optimizely CMS, like cache, authenticaation, Cms, TinyMce, and more...
    - registers common view locations
- Contains extensions for XhtmlString, ContentReference, ContentArea, etc ... such as Is() and IsNot()
- Contains 'CurrentUser' class, either new it up or inject it
- Contains ReactServerSide rendering results to convert a Block to React
- Contains a DefaultComponent so no need to create a AsyncComponent if all you need is the block available in the View as a Model

## Requirements
- &gt;= .NET 8
- &gt;= Episerver 12.32.4

## Latest Release Notes
- 8.1.0.5
- Based on environment and hosting option we adjust the registration of appsettings.json, its opted out for in DXP (fix)
- Removed direct dependency on MicrosoftClearScript
- Added dep to JavaScriptEngineSwitcher.V8.Native.linux-x64 for hosting in DXP with Linux containers (feature)

#### Major Breaking Versions
- 8.1.0.1
- Removed SystemLibrary.Common.Net and SystemLibrary.Common.Web for SystemLibrary.Common.Framework (breaking change)
- CurrentUser renamed to AppCurrentUser (breaking change)
- CurrentUser created and made static, creating a new AppCurrentUser on every invocation (at time being) (breaking change)
- CmsAppBuilderOptions renamed to CmsFrameworkOptions, one options object for both Services and Middlewares (breaking change)

- 8.0.0.1
- Updated target framework to net8 (breaking change)
- Updated Episerver.CMS minimum dep version to 12.32.2 (breaking change)
- CreateHostBuilder has new argument Hosting (breaking change)
- appSettings: moved 'showComponentEditLink' to under 'ssr' config element (breaking change)

- 7.13.0.1
- Updated SystemLibrary.Common.Net dep where Encrypt() is rewritten and Config files are never read from 'bin', unless project name contains '.Test' (breaking change)
- Removed "appUrl" from appSettings, only inside 'Manage Websettings' we can configure the primary "appUrl" per site (breaking change)
- AppSettings is now internal, use PackageConfigInstance.Current instead which exposes some settings (breaking change)
- StringExtension.IsFile removed, as SystemLibrary.Common.Net has it already (breaking change)

#### Version history
- View git history of this file if interested

## Installation
- Simply install the nuget package
- [Installation guide](https://systemlibrary.github.io/systemlibrary-common-episerver/Install.html)


## Documentation
- [Documentation with code samples](https://systemlibrary.github.io/systemlibrary-common-episerver/)

## Nuget
- [Nuget package page](https://www.nuget.org/packages/SystemLibrary.Common.Episerver/)

## Source
- [Github](https://github.com/systemlibrary/systemlibrary-common-episerver)

## Suggestions and feedback
- [Send us an email](mailto:support@systemlibrary.com)

## License
- Free