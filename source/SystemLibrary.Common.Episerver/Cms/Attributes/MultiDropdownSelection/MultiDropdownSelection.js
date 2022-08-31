//CREDS TO: https://tedgustaf.com/blog/2016/custom-editor-for-string-list-properties-in-episerver/
define([
    "dojo/_base/declare",
    "dojo/aspect",
    "dojo/dom-construct",
    "dojo/dom-attr",
    "dojo/dom-style",
    //"dojo/_base/connect",
    "dojo/_base/lang",
    "dojo/query",

    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",

    // "dijit/form/Button",
    // "dijit/form/Select",
    // "dijit/form/TextBox",

    'dojo/text!./Html'
],

    function (
        declare,
        aspect,
        domConstruct,
        domAttr,
        domStyle,
        //  connect,
        lang,
        query,

        _Widget,
        _TemplatedMixin,
        _WidgetsInTemplateMixin,

        template
        // Button,
        // Select,
        // TextBox,
        //Labels
    ) {
        return declare("systemLibrary.Common.Episerver.MultiSelectDropdown", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin], {

            templateString: template, // dojo.cache("systemLibrary.Common.Episerver.MultiSelectDropdown", "Template.html"),

            labels: {
                textboxWatermark: 'Enter a value to add',

                clickToAdd: 'Click to add',
                clickToRemove: 'Click to remove',

                helpText: 'No items are selected'
            },

            value: null,

            _hasSelectionFactory: false,

            constructor: function () {
                this.inherited(arguments);

                // When the property value is set, we refresh the DOM elements representing the strings in the list
                aspect.after(this, '_set', lang.hitch(this, function () {
                    this._refreshStringElements(this.value);
                    
                }));
            },

            postCreate: function () {
                this.inherited(arguments);

                this._loadCssFile();

                // summary: Populates the dropdown (if selection factory options are available), otherwise the textbox is displayed

                if (this.selections && this.selections.length > 0) {
                    this._hasSelectionFactory = true;
                }

                if (this._hasSelectionFactory) {
                    for (var i = 0; i < this.selections.length; i++) {

                        var item = this.selections[i];

                        this.stringSelector.addOption({
                            disabled: false,
                            label: (item.text && item.text !== '') ? item.text : '&nbsp',
                            selected: false,
                            value: item.value
                        });
                    }

                    // Only display dropdown when we have a selection factory attached
                    domStyle.set(this.stringTextbox.domNode, 'display', 'none');
                } else {
                    // Only display textbox when there is no selection factory attached
                    domStyle.set(this.stringSelector.domNode, 'display', 'none');
                }

                this.stringSelector.setDisabled(this.readOnly);
                this.stringTextbox.setDisabled(this.readOnly);
                this.addButton.setDisabled(true); // Disable add button by default, until string is selected or entered
            },

            _getStylesheetLink(id, path) {
                var link = document.createElement('link');
                link.id = id;
                link.rel = 'stylesheet';
                link.type = 'text/css';
                link.media = 'all';
                link.href = path;
                return link;
            },

            _loadCssFile: function () {
                try {
                    var cssPrefixId = 'systemLibraryCommonEpiserverMultiDropdownSelectionStyle';
                    if (!document.getElementById(cssPrefixId + '1')) {
                        var head = document.getElementsByTagName('body')[0];

                        var css1 = this._getStylesheetLink(cssPrefixId + '1', '/SystemLibrary/Common/Episerver/UiHint/MultiDropdownSelection/Style');

                        head.appendChild(css1);
                    }
                }
                catch (e) {
                    console.error(e);
                }
            },

            onChange: function (value) {
                this.inherited(arguments);
            },

            _setValue: function () {
                var strings = this._getAddedStrings();

                this.set("value", strings.length > 0 ? strings : null);

                this._setHelpTextVisibility();

                this.onChange(strings);
            },

            _refreshStringElements: function (strings) {
                if (strings === undefined || strings === null) {
                    return;
                }

                var that = this;

                strings.forEach(function (string, index, array) {
                    if (strings.indexOf(string) === -1) {
                        that._removeStringElement(string);
                    }
                });

                // Add an element for each string in the list
                strings.forEach(function (string, index, array) {

                    var displayName = that._getStringDisplayName(string);

                    that._addStringElement(string, displayName);
                });

                this._setHelpTextVisibility();
            },

            _setHelpTextVisibility: function () {
                if (!this.value || this.value.length === 0) {
                    domStyle.set(this.helpText, 'display', 'inline');
                } else {
                    domStyle.set(this.helpText, 'display', 'none');
                }
            },

            _onTextboxKeyUp: function (e) {
                var value = e.target.value.toString().trim();

                this.addButton.setDisabled(value.toString().trim() === '');
            },

            _onTextboxKeyDown: function (e) {
                if (e.keyCode === 13) // Enter
                {
                    e.target.blur();

                    this._addString(e.target.value.toString().trim());
                }
            },

            _selectedStringChanged: function (value) {
                if (value) {
                    this.addButton.setDisabled(false);
                }
            },

            _onRemoveClick: function (e) {
                var stringValue = domAttr.get(e.srcElement, "data-value").toString().trim();

                this._removeStringElement(stringValue);

                this._setValue();
            },

            _onAddButtonClick: function () {
                if (this._hasSelectionFactory) { // Add string selected in dropdown
                    var selectedValue = this.stringSelector.value;
                    var displayName = this.stringSelector.focusNode.innerText;
                    console.log("ADD SELECTED VALUE");
                    console.log(selectedValue);
                    console.log(displayName);
                    if (!selectedValue) {
                        return;
                    }

              

                    this._addString(selectedValue, displayName);
                } else { // Add string from textbox

                    var enteredValue = this.stringTextbox.value;

                    if (!enteredValue) {
                        return;
                    }

                    this._addString(enteredValue);
                }
            },

            _getStringElements: function () {

                // summary: Gets all DOM elements representing added strings

                return query(".epi-categoryButton", this.valuesContainer);
            },

            _getAddedStrings: function () {

                // summary: Gets the values of all DOM elements representing added strings

                var elements = this._getStringElements();

                var strings = [];

                elements.forEach(function (element, index, array) {
                    strings.push(domAttr.get(element, 'data-value'));
                });

                return strings;
            },

            _addString: function (value, displayName) {

                // summary: Adds a string to the list and updates the property value

                value = value.toString().trim();

                if (!value) {
                    return;
                }

                if (!displayName) {
                    displayName = value;
                }

                this._addStringElement(value, displayName);

                this.stringTextbox.set('value', ""); // Reset textbox value

                this._setValue();
            },

            _addStringElement: function (value, displayName) {

                // summary: Adds a DOM element representing a string in the list

                if (!value) {
                    return;
                }

                value = value.toString().trim();

                if (value === '') {
                    return;
                }

                if (!displayName) {
                    displayName = this._getStringDisplayName(value);
                    //displayName = value;
                }

                // Don't add if it's already added
                if (query("div[data-value=" + value + "]", this.valuesContainer).length !== 0) {
                    return;
                }

                var containerDiv = domConstruct.create('div', { 'class': 'epi-categoryButton' });
                var buttonWrapperDiv = domConstruct.create('div', { 'class': 'dijitInline epi-resourceName' });
                var categoryNameDiv = domConstruct.create('div', { 'class': 'dojoxEllipsis', innerHTML: displayName });

                domConstruct.place(categoryNameDiv, buttonWrapperDiv);

                domConstruct.place(buttonWrapperDiv, containerDiv);

                var removeButtonDiv = domConstruct.create('div', { 'class': 'epi-removeButton', innerHTML: '&nbsp;', title: 'Click to remove' });

                var eventName = removeButtonDiv.onClick ? 'onClick' : 'onclick';

                // Add attributes to make added values easy to find and remove
                domAttr.set(containerDiv, 'data-value', value);
                domAttr.set(removeButtonDiv, 'data-value', value);

                if (!this.readOnly) {
                    this.connect(removeButtonDiv, eventName, lang.hitch(this, this._onRemoveClick));
                    domConstruct.place(removeButtonDiv, buttonWrapperDiv);
                } else {
                    var removeButtonDiv = domConstruct.create('div', { 'class': 'epi-removeButton epi-removeButton--hidden', innerHTML: '&nbsp;', title: 'Click to remove' });
                    this.connect(removeButtonDiv, eventName, lang.hitch(this, this._onRemoveClick));
                    domConstruct.place(removeButtonDiv, buttonWrapperDiv);
                    // domConstruct.place(domConstruct.create("span", { innerHTML: "&nbsp;" }), buttonWrapperDiv);
                }

                domConstruct.place(containerDiv, this.valuesContainer);
            },

            _removeStringElement: function (value) {

                // summary: Removes the DOM element, if any, representing a string in the list

                if (!value) return;

                if (value.toString().trim() === '') {
                    return;
                }

                var matchingValues = query("div[data-value=" + value + "]", this.valuesContainer);

                for (var i = 0; i < matchingValues.length; i++) {
                    domConstruct.destroy(matchingValues[i]);
                }
            },

            _getStringDisplayName: function (text) {

                // summary: Looks up a string value among the selection factory options, returning the corresponding display name if found

                if (!this._hasSelectionFactory) {
                    return text;
                }
                var displayName = text;

                if (typeof text === 'number' || !isNaN(text)) {
                    if (this.selections && this.selections.length > text) {
                        const selection = this.selections[text];
                        displayName = selection.text;
                    }
                }
                else {
                    this.selections.some(function (selection) {
                        if (selection.value === text) {
                            if (selection.text) {
                                displayName = selection.text;
                            }
                            return true; // Break
                        }
                    });
                }
                return displayName;
            }
        });
    });