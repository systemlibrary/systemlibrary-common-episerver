# SystemLibrary Common Episerver

## Requirements
- &gt;= .NET 6
- &gt;= Episerver 12


## Latest Version
- 6.0.0.2
- Updated latest dependencies
- Bumped version to reflect we are on .NET 6
- FontAwesome Solid Icons now works in ContentIcon attribute
- ContentIcon attribute bug where Solid Icons from FontAwesome not displaying fixed
- ContentIcon attribute with a custom relative icon path, now also shows the icon in the page tree
- Added "PageDataExtensions" such as Is() IsNot() and ToFriendlyUrl()
- Renamed ServiceCollectionEpiserverOptions to CommonEpiserverApplicationServicesOptions (Breaking Change)
- Renamed EpiserverApPBuilderOptions to CommonEpiserverApplicationBuilderOptions (Breaking Change)
- Default password for demo user is now Demo123! instead of Admin123!
- ViewLocations now actually registered - they were never invoked in previous version iirc
- ParentLinkReference a new custom property: public virtual ParentLinkReference LinkRef { get;set;} on any block or page, try it!
- Two new configurations: activeProjectBarBackgroundColor and hideLanguageColumnInVersionGadget, available in appSettings.json
- Install documentation is completely rewritten

## Description
A library of classes and methods for any .NET &gt;= 6 episerver web application

* Setup IApplicationBuilder in one line: app.CommonEpiserverApplicationBuilder();
* Setup IServiceCollection in one line: services.CommonEpiserverApplicationServices&lt;CurrentUser&gt;().AddCms().AddTinyMce();

The two methods in short enables:
* serving of static common file types (css, js, png, jpg, ...)
* routing requests to controllers
* registers services for AspNet.Mvc
* registers and enabled Authorization and Authentication
* registers and initializes Episerver Cms
* registers a new "demo/Demo123!" user if there's no user in the DB
* registers a few view locations, and you can add more when calling CommonEpiserverApplicationServices()
* enables the login screen on "https://domain.com/episerver"

Contains other modules such as:
- ContentIcon, custom icon for blocks and pages with over several thousand to pick from (FontAwesome Free Icons v6), and PageTree is also using the icons
- Extensions like Is() and IsNot() for XhtmlString, ContentReference and ContentArea
- New property types, such as: ParentLinkReference, which creates a Link in "Property Mode" to its parent, no more 'Where is this content stored'
- Contains a "CurrentUser" you can inject or new up

Multiple dijit widgets coming soon, like colorpicker and much much more!

## Docs
Documentation with samples:
https://systemlibrary.github.io/systemlibrary-common-episerver/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Episerver/

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- It's free forever, copy paste as you'd like