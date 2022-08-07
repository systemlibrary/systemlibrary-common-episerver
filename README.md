# SystemLibrary Common Episerver

## Description
Library with classes, methods and dijits for every .NET &gt;= 6 episerver web application

### Features

#### BoxSelection
![Box Selection Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-boxselection.png "")
- Supports showing any free FontAwesome Icon
- Supports showing your own custom images
- Supports setting background color, to be used as a color picker  

#### Message
![Message Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-message.png "")
- Display a simple help text to the editors
- Has an built-in toggle functionality that activates if text to display is large  

#### ContentIcon
![Content Icon Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-contenticon.png "")
- Shows icons in both Page Tree and the 'New Content' dialog
- Supports choosing any of the free FontAwesome Icons
- Supports your own custom images  

#### Parent Link Reference
![Parent Link Reference Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-parentlinkreference.png "")
- Creates a link to the parent, where 'this' content is stored
- No more wondering where content is stored, you have a link to it now  

#### JsonEdit
![Json Edit Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsonedit.png "")
![Json Edit Preview](https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsoneditor-view.png "")
- A simple json editor for simple objects
- Data is stored as string, so you invoke the StringExtension .ToJson() to get it as a C# class
- Contains ways to add placeholders to input fields, required message to each field, and a displayName for each property  

#### One Line Setup
* Setup IApplicationBuilder in one line:  
    app.CommonEpiserverApplicationBuilder();
    - Routing requests to content controllers and mvc controllers
    - New user "demo/Demo123!" if no user exists in db
    - Enables login on relative path '/episerver'
    - Register middleware for serving static files such as css, js, png, jpg, ...
    - Register middleware for Authorization and Authentication attributes  
* Setup IServiceCollection in one line:  
    services.CommonEpiserverApplicationServices&lt;CurrentUser&gt;().AddCms().AddTinyMce();
    - Register AspNet.Mvc services
    - Routing requests to controllers
    - Add view locations or area view locations by setting them in the options sent to: CommonEpiserverApplicationServices()
    - Register service for serving static files such as css, js, png, jpg, ...
  
#### Additionally
- Contains extensions for XhtmlString, ContentReference, ContentArea, etc ... such as Is() and IsNot()
- Contains 'CurrentUser' class, either new it up or inject it

## Requirements
- &gt;= .NET 6
- &gt;= Episerver 12

## Latest Version
- 6.2.0.1
- Updated docs
- Updated deps
- New: validator class, for validation messages before publishing content
- BoxSelection: max width set so 6 boxes shows per row
- Message: info button on right side, and its clickable
- /Components/ is no longer a default view location (breaking change)
- A web-option variable was renamed: HttpRediretionAndHsts to HttpToHttpsRedirectionAndHsts (breaking change)

#### Version history
- View git history of this file if interested

## Installation
https://systemlibrary.github.io/systemlibrary-common-episerver/Install.html

## Docs
Documentation with code samples:  
https://systemlibrary.github.io/systemlibrary-common-episerver/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Episerver/

## Source
https://github.com/systemlibrary/systemlibrary-common-episerver

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- Free