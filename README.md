
# SystemLibrary Common Episerver

## Description
Library with classes, methods and dijits for every &gt;= .NET 8 episerver application

## Requirements
&gt;= .NET 8

&gt;= Episerver 12.33.0

## Access & Contributing
View full source code or contribute, email [support@systemlibrary.com](mailto:support@systemlibrary.com) to request access with your github username. Read-only access is granted upon request.
- Forking, cloning, and submitting pull requests are possible once access is granted  

### Features

#### 📦 BoxSelection
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-boxselection.png" alt="Box Selection Preview" style="max-width: 75%; height: auto;" />
Pick icons, images, or colors — ideal for visual choices and content tagging.

Supports a custom background color, a custom image or select any of the built-in font-awesome images.

#### 📅 DateSelection
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-dateselection.png" alt="Date Selection Preview" style="max-width: 75%; height: auto;" />
The built-in datetime selection morphed to date selection, with refined styling.

#### 🔽 MultidropdownSelection
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-multidropdownselection.png" alt="MUlti Dropdown Selection Preview" style="max-width: 75%; height: auto;" />
Select from enums or add custom text — supports both structured and free-form lists.

#### 💬 Message
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-message.png" alt="Message Preview" style="max-width: 75%; height: auto;" />
Inline help text with auto-toggle for long content — guide editors directly in the UI.

#### 🧩 ContentIcon
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-contenticon.png" alt="Content Icon Preview" style="max-width: 75%; height: auto;" />
Show icons in the Page Tree, Block Tree and "New Content" — choose from FontAwesome or custom images.

Supports custom images and built-in font-awesome images.

#### 🔗 Parent Link Reference
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-parentlinkreference.png" alt="Parent Link Reference Preview" style="max-width: 75%; height: auto;" />
Auto-link to the parent container — always know where content lives.

#### ⚙️ JsonEdit
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsonedit.png" alt="Json Edit Preview" style="max-width: 75%; height: auto;" />

<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsoneditor-view.png" alt="Json Edit Preview"style="max-width: 75%; height: auto;" />
Edit and preview simple JSON objects with support for placeholders, required fields, and rich text (via `XhtmlString`). 

Deserialize using `.Json()` or `.JsonEditAsObject()`.

#### 👤 CurrentUser  
Static global `CurrentUser` class for user-specific data access  

#### 🧩 Extension Methods  
Includes extensions for `XhtmlString`, `ContentReference`, `ContentArea`, etc. — such as `.Is()` and `.IsNot()`  

#### ⚛️ React Server-Side Rendering  
One-liner conversion from a Block or ViewModel into React rendering results  

#### 🧱 DefaultComponent  
Built-in `DefaultComponent` means no need to create an `AsyncComponent` if you only need the block available as a model in the View

#### 💻 One-Line Setup
```csharp
class LogWriter { ... } // Your own

var opt = new CmsFrameworkOptions();
services.AddCommonCmsServices<AppCurrentUser, LogWriter>(opt).AddFind();
app.UseCommonCmsApp(opt);
```
Registers common services and middlewares for Optimizely CMS in one line each
Includes cache, auth, CMS, TinyMCE, routing, cookies, and shared view locations

## Latest Release Notes
- 8.3.0.1
- ContentReference, removed one if-statement which never is true in SSR (fix)
- Dep, SystemLibrary.Common.Framework from 8.0.0.33 to 8.3.0.1 (breaking change)
- Dep, Episerver CMS from 12.32.4 to 12.33.0 (breaking change)
- Dep, Episerver TinyMce from 4.8.3 to 5.0.1 (breaking change)

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
Free

### Dependencies
- [SystemLibrary.Common.Framework](https://github.com/systemlibrary/systemlibrary-common-framework), Free with Tiered Pricing for additional features
- [Chart.js](https://github.com/chartjs/Chart.js), licensed under the MIT License.
- [Prometheus-net](https://www.nuget.org/packages/prometheus-net), licensed under the MIT License.
- [FontAwesome](https://fontawesome.com/), Used under Creative Commons Attribution 4.0. Please ensure appropriate attribution is maintained in your use.