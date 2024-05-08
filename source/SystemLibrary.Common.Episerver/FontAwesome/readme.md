# Font Awesome

Allows rendering icons from font awesome in "New Content Dialog" and in "Page Tree" through ContentIconAttribute

Current font awesome version: v.6.5.2

## Update FontAwesome 
### Download FontAwesome
- Unzip the font-awesome download for web, the free package

### Update fontawesome-bundled.min.css
- Copy + Paste 'brand.min.css' and 'solid.min.css' at end of 'fontawesome.min.css', which all comes from the downloaded font awesome zip file
- Copy paste 'fontawesome.min.css' and replace with the part of 'fontawesome-bundled.min.css' that is from FontAwesome, preserve the custom css rules at bottom
- Search in the new fontawesome-bundled.min.css for "woff2" and update the url to what it was as it must load from "SystemLibrary" controller
- Comment out the part after "woff2" regarding ttf, we only use the woff2 part

### Copy Images
- Copy all solid, brands and regular images to the projects icon folder where they belong

### Update Enums
- For instance, updating Enum 'FontAwesomeBrands', then open the folder where all its icons exists
- Then open 'cmd' and print out all files in current folder: 'dir /s' on windows
- Use notepad++ to replace .svg with commas, etc... to make it C# Enum friendly...
- Prefix enum names that starts with number with "__enum_"
- Replace - with _
- If some key names error as they are keywords in C#? Simply delete those few icons...
