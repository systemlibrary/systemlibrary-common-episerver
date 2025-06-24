
# SystemLibrary Common Episerver

## Description
Library with classes, methods and dijits for every &gt;= .NET 8 episerver application

## Requirements
&gt;= .NET 8

&gt;= Episerver 12.33.1

## Access & Contribute
[**GitHub Source**](https://github.com/systemlibrary/systemlibrary-common-episerver-private)

To request access, email `support@systemlibrary.com` with your GitHub username and specify the repo.

Read-only access is granted on request — no questions asked.  
Once approved, you can fork, clone, and submit pull requests.

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
<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsonedit.png" alt="Json Edit Property Preview" style="max-width: 
75%; height: auto;" />

<img src="https://raw.githubusercontent.com/systemlibrary/systemlibrary-common-episerver/main/assets/images/cms-property-jsoneditor-view.png" alt="Json Edit Property Preview" style="max-width: 75%; height: auto;" />
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
- 8.4.0.4
- IsCmsUser: user-agent containing spider is ignored (fix)
- Updated deps

#### Version history
- View git history of this file if interested

## Installation
- Simply install the nuget package
- [Installation guide](https://systemlibrary.github.io/systemlibrary-common-episerver/Install.html)

## Documentation
- [Documentation with code samples](https://systemlibrary.github.io/systemlibrary-common-episerver/)

## Nuget
- [Nuget package page](https://www.nuget.org/packages/SystemLibrary.Common.Episerver/)

## License
Free

### Dependencies
- [SystemLibrary.Common.Framework](https://github.com/systemlibrary/systemlibrary-common-framework), Free with Tiered Pricing for additional features
- [Optimizely CMS](https://www.optimizely.com/products/content-management/), commercial licensed 
- [Chart.js](https://github.com/chartjs/Chart.js), licensed under the MIT License.
- [Prometheus-net](https://www.nuget.org/packages/prometheus-net), licensed under the MIT License.
- [FontAwesome](https://fontawesome.com/), Used under Creative Commons Attribution 4.0. Please ensure appropriate attribution is maintained in your use.