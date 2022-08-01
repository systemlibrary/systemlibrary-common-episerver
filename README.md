# SystemLibrary Common Episerver

## Requirements
- &gt;= .NET 6
- &gt;= Episerver 12


## Latest Version
- 6.1.0.1
- New custom dijits: Message, BoxSelection and JsonEdit
- New property list of T? Simply create a new [PropertyDefinitionTypePlugIn(Description = "...", GUID = "...")] class TProperty : IListProperty&lt;Car&gt;
- Updated latest dependencies

## Description
A library of classes and methods for any .NET &gt;= 6 episerver web application

### Initialize App In One Line
* Setup IApplicationBuilder in one line: app.CommonEpiserverApplicationBuilder();
* Setup IServiceCollection in one line: services.CommonEpiserverApplicationServices&lt;CurrentUser&gt;().AddCms().AddTinyMce();

### New Custom Dojo/Dijits

##### Parent Link Reference
![Parent Link Reference Preview](assets/images/cms-property-parentlinkreference.png?raw=true "Parent Link Reference Preview")

##### BoxSelection
![Box Selection Preview](assets/images/cms-property-boxselection.png?raw=true "Box Selection Preview")

##### Message
![Message Preview](assets/images/cms-property-message.png?raw=true "Message Preview")

##### JsonEdit
![Json Edit Preview](assets/images/cms-property-jsonedit1.png?raw=true "Json Edit Preview")
![Json Edit Preview](assets/images/cms-property-jsonedit2.png?raw=true "Json Edit Preview")

##### ContentIcon
![Content Icon Preview](assets/images/cms-property-contenticon.png?raw=true "Content Icon Preview")

### Details

####
The two one-liners enables:
* AspNet.Mvc
* Serving of static file types such as css, js, png, jpg, ...
* Routing requests to controllers
* Authorization and Authentication attributes
* Episerver Cms
* A new user "demo/Demo123!" if no user exists already
* View locations, which you add more through the one-liner: CommonEpiserverApplicationServices()
* Enables login on url https://domain.com/episerver

#### Custom Dojo/Dijits
- Parent Link Reference, a link to where the "current content" is stored, so no more "where is this content stored?"

- BoxSelection, an "advanced" checkbox/radio list, with options to display background colors, or images from FontAwesome package

- Message, a custom message rendered as a property in "Settings View", does not store anything, just a "ui help text"

- JsonEdit, a simple editor to edit json formatted data that is stored in a string property, which you easily just call "nameOfProperty".ToJson&lt;List&lt;Car&gt;&gt;(); to get the data as a list of some C# object

- ContentIcon, pick a content icon for your types either from FontAwesome or add your own custom image by a relative path. The page icon also shows up in the page tree

- Extensions like Is() and IsNot() for XhtmlString, ContentReference and ContentArea

- Contains a "CurrentUser" which you can simply "var user = new CurrentUser()" anywhere, or inject it

## Docs
Documentation with samples:
https://systemlibrary.github.io/systemlibrary-common-episerver/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Episerver/

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- It's free forever, copy paste as you'd like