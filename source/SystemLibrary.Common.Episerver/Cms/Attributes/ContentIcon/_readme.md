# Content Icon Attribute

- Displays icon on "New Content Dialog"
- Displays icon in "Page Tree"

- Uses the free icons from Font Awesome v.6

## Update FontAwesome 
### Download FontAwesome

- Unzip the font-awesome download

### Manually prepare bundled.min.css
- Copy + Paste 'brand.min.css' and 'solid.min.css' at end of 'fontawesome.min.css', which all comes from the downloaded font awesome zip file
- Rename 'fontawesome.min.css' to 'fontawesome-bundled.min.css'
- Replace the new file's content with the one in this project
  - * Note: there are some custom css rules at the bottom of existing "fontawesome-bundled-min.css" which you need to preserve manually

### Copy Images
- Copy all solid, brands and regular images to the projects icon folder where they belong

### Update Enums
- For instance, updating Enum 'FontAwesomeBrands', then open the folder where all its icons exists
- Then open 'cmd' and print out all files in current folder: 'dir /s' on windows
- Use notepad++ to replace .svg with commas, etc... to make it C# Enum friendly...


### Future:
Support a way to add custom images also to the "Page tree"
Support also a way to custom background color of the image, which is now white with a black svg
Support a way of setting custom color to the SVG that are black, inside "img" tag, like so: img {invert(1) sepia(1) saturate(25) hue-rotate(230deg) }