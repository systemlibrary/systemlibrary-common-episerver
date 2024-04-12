# SystemLibrary Common Episerver

## Description
Library with classes, methods and dijits for every &gt;= .NET 7 episerver application

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


## Requirements
- &gt;= .NET 7
- &gt;= Episerver 12.26.0

## Latest Version
Release 7.9.0.2
- ReactServerSideRendering the key generator appends data from url, linkitem and linkitemcollection too
- ToExpandoObject skips types of Message, ParentLinkReference, ReadOnlySpan<byte>, ReadOnlySpan<char>, System.Type and System.Encoding

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