# SystemLibrary Common Episerver

## Requirements
- &gt;= .NET 6
- &gt;= Episerver 12

## Latest Version
- 6.1.0.3
- New dijits: Message, BoxSelection, JsonEdit
- Cleaned up the "built-in" css file that loads in "Edit Mode"
- Updated latest dependencies
- Updated documentations and sample code for most public methods and properties and classes

## Description
A library of classes, methods and dijits for every .NET &gt;= 6 episerver web application

### BoxSelection
![Box Selection Preview](assets/images/cms-property-boxselection.png?raw=true "Box Selection Preview")
- Supports showing any free FontAwesome Icon
- Supports showing your own custom images
- Supports setting background color, to be used as a color picker

### Message
![Message Preview](assets/images/cms-property-message.png?raw=true "Message Preview")
- Display a simple help text to the editors
- Has an in-build toggle functionality that activates if text to display is large

### ContentIcon
![Content Icon Preview](assets/images/cms-property-contenticon.png?raw=true "Content Icon Preview")
- Shows icons in both Page Tree and the 'New Content' dialog
- Supports choosing any of the free FontAwesome Icons
- Supports your own custom images

### Parent Link Reference
![Parent Link Reference Preview](assets/images/cms-property-parentlinkreference.png?raw=true "Parent Link Reference Preview")
- Creates a link to the parent, where 'this' content is stored
- No more wondering where content is stored, you have a link to it now

### JsonEdit
![Json Edit Preview](assets/images/cms-property-jsonedit.png?raw=true "Json Edit Preview")
![Json Edit Preview](assets/images/cms-property-jsoneditor-view.png?raw=true "Json Edit Preview")
- A simple json editor for simple objects
- Data is stored as string, so you invoke the StringExtension .ToJson() to get it as a C# class
- Contains ways to add placeholders to input fields, required message to each field, and a displayName for each property

### One-Line Setup!
* Setup IApplicationBuilder in one line: 
    app.CommonEpiserverApplicationBuilder();
* Setup IServiceCollection in one line:
    services.CommonEpiserverApplicationServices&lt;CurrentUser&gt;().AddCms().AddTinyMce();

#### Information
The two one-liners enables:
* AspNet.Mvc
* Serving of static file types such as css, js, png, jpg, ...
* Routing requests to controllers
* Authorization and Authentication attributes
* Episerver Cms
* A new user "demo/Demo123!" if no user exists already
* View locations, which you add more through the one-liner: CommonEpiserverApplicationServices()
* Enables login on url https://domain.com/episerver

### Additionally
- Has a lot of extensions, like 'Is()' and 'IsNot()' for string, XhtmlString, ContentReference, ContentArea, StringBuilder...
- Has a "CurrentUser" you can either "new CurrentUser()" anywhere or inject it

## Docs
Documentation with samples and installation instructions:  
https://systemlibrary.github.io/systemlibrary-common-episerver/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Episerver/

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- It's free forever, copy paste as you'd like