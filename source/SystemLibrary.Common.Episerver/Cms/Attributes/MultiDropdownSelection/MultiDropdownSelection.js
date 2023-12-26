//CREDS TO: https://tedgustaf.com/blog/2016/custom-editor-for-string-list-properties-in-episerver/
define([
    "dojo/_base/declare",
    "dojo/aspect",
    "dojo/dom-construct",
    "dojo/dom-attr",
    "dojo/dom-style",
    "dojo/_base/lang",
    "dojo/query",

    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",

    'dojo/text!./Html'
],

    function (
        declare,
        aspect,
        domConstruct,
        domAttr,
        domStyle,
        lang,
        query,

        _Widget,
        _TemplatedMixin,
        _WidgetsInTemplateMixin,

        template
    ) {
        return declare("systemLibrary.Common.Episerver.MultiSelectDropdown", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin], {

            templateString: template,

            filteredSelections: [],

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

            isSelectionStorable: function (item) {
                let canStoreItem = false;
                let filterText = item?.text?.toLowerCase();
                let filterValue = item?.value?.toLowerCase();

                if (!filterText && !filterValue) {
                    return canStoreItem;
                }

                for (var j = 0; j < this.multiDropdownStoreOptions?.length; j++) {
                    var matchText = this.multiDropdownStoreOptions[j]?.text?.toLowerCase();
                    var matchValue = this.multiDropdownStoreOptions[j]?.value?.toLowerCase();

                    canStoreItem = matchText === filterText || matchValue === filterValue;

                    if (canStoreItem) {
                        break;
                    }
                }

                return canStoreItem;
            },

            getDisplayNameOfSelectedValue: function (filteredIndex) {
                try {
                    let item = this.multiDropdownStoreOptions[Number(filteredIndex)];

                    return item.text;
                } catch (err) {
                    console.error(err);
                }
                return filteredIndex;
            },

            getSelectionIndexStorable: function (filteredIndex) {
                try {
                    let item = this.selections[Number(filteredIndex)];
                    let value = item?.value;
                    if (value) {
                        let index = filteredIndex;
                        for (var i = 0; i < this.multiDropdownStoreOptions?.length; i++) {
                            var matchText = this.multiDropdownStoreOptions[i]?.text?.toLowerCase();
                            var matchValue = this.multiDropdownStoreOptions[i]?.value?.toLowerCase();

                            canStoreItem = matchText === value || matchValue === value;

                            if (canStoreItem) {
                                index = i;
                                break;
                            }
                        }
                        return index;
                    }
                } catch (err) {
                    console.error(err);
                }
                return filteredIndex;
            },

            postCreate: function () {
                this.inherited(arguments);

                this._loadCssFile();

                try {
                    if (this.multiDropdownSelectionSaveString) {
                        // Display textbox when there are no select options to select from
                        domStyle.set(this.stringSelector.domNode, 'display', 'none');
                    } else {
                        if (this.selections?.length) {
                            for (let i = 0; i < this.selections.length; i++) {
                                let item = this.selections[i];

                                if (this.multiDropdownSelectionDoFilter) {
                                    let canStoreItem = this.isSelectionStorable(item);

                                    this.filteredSelections.push(item);

                                    if (canStoreItem) {
                                        this.stringSelector.addOption({
                                            disabled: false,
                                            label: (item.text && item.text !== '') ? item.text : '&nbsp',
                                            selected: i === 0 && item.text !== '' && item.text.length > 0,      // First item is always "addable", unless it is empty string ""
                                            value: item.value?.toString()
                                        });
                                    }
                                } else {
                                    this.stringSelector.addOption({
                                        disabled: false,
                                        label: (item.text && item.text !== '') ? item.text : '&nbsp',
                                        selected: i === 0 && item.text !== '' && item.text.length > 0,      // First item is always "addable", unless it is empty string ""
                                        value: item.value?.toString()
                                    });
                                }
                            }
                        }

                        // Display dropdown when we have a list of selections to select from
                        domStyle.set(this.stringTextbox.domNode, 'display', 'none');
                    }

                    this.stringSelector.setDisabled(this.readOnly);
                    this.stringTextbox.setDisabled(this.readOnly);
                    this.addButton.setDisabled(false);
                }
                catch (err) {
                    console.error(err);
                }
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

                        var css1 = this._getStylesheetLink(cssPrefixId + '1', '/SystemLibrary/CommonEpiserverCms/MultiDropdownSelection/Style');

                        head.appendChild(css1);
                    }
                }
                catch (e) {
                    console.error(e);
                }
            },

            _setValue: function () {
                var value = this._getAddedStrings();

                if (!value || value === '' || value.length === 0) {
                    value = null;
                }
                this.set("value", value);

                this._setHelpTextVisibility();

                this.onChange(value);
            },

            _refreshStringElements: function (strings) {
                if (strings === undefined || strings === null || strings === '') {
                    return;
                }

                var that = this;

                if (strings.forEach) {
                    strings.forEach(function (text, index, array) {
                        if (strings.indexOf(text) === -1) {
                            that._removeStringElement(text);
                        }
                    });

                    // Add an element for each string in the list

                    if (that.filteredSelections.length > 0) {

                        strings.forEach(function (text, index, array) {
                            var displayName = that._getStringDisplayName(text);

                            if (!isNaN(displayName)) {
                                displayName = that.getDisplayNameOfSelectedValue(displayName);
                            }
                            that._addStringElement(text, displayName);
                        });

                    } else {
                        strings.forEach(function (text, index, array) {
                            var displayName = that._getStringDisplayName(text);
                            that._addStringElement(text, displayName);
                        });
                    }
                }
                else {
                    for (var i = 0; i < strings.length; i++) {
                        if (strings.indexOf(strings[i]) === -1) {
                            that._removeStringElement(strings[i]);
                        }
                    }

                    for (var i = 0; i < strings.length; i++) {
                        var displayName = that._getStringDisplayName(strings[i]);

                        that._addStringElement(strings[i], displayName);
                    }
                }

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
                    //e.target.blur();

                    this._addString(e.target.value.toString().trim());
                }
            },

            _selectedStringChanged: function (value) {
                if (value) {
                    // value is is in list, disable button, else enable it
                    this.addButton.setDisabled(false);
                }
            },

            _onRemoveClick: function (e) {
                var stringValue = domAttr.get(e.srcElement, "data-value")?.toString()?.trim();

                if (stringValue) {
                    this._removeStringElement(stringValue);
                }

                this._setValue();
            },

            _onAddButtonClick: function () {
                if (this.selections?.length > 0) {
                    // Add item from dropdown
                    var selectedValue = this.stringSelector.value;

                    var displayName = this.stringSelector.focusNode.innerText;

                    if (isNaN(selectedValue)) {
                        if (!this.multiDropdownSelectionSaveString) {
                            if (this.selections && this.selections.length) {
                                var index = -1;
                                for (var i = 0; i < this.selections.length; i++) {
                                    if (this.selections[i].value === selectedValue) {
                                        index = i;
                                    }
                                }

                                if (index >= 0) {
                                    selectedValue = index.toString();
                                }
                            }
                        }
                    }


                    this._addString(selectedValue, displayName);
                } else {
                    // Add string from textbox
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

                value = value?.toString()?.trim();

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

                if (typeof (value) === 'undefined' || value === null) {
                    return;
                }

                value = value?.toString()?.trim();

                if (value.indexOf(',') > -1) {
                    value = value.replaceAll(',', '&#44;');
                }

                if (this.filteredSelections.length > 0) {
                    value = this.getSelectionIndexStorable(value);
                }

                if (!displayName) {
                    displayName = this._getStringDisplayName(value);
                }

                // Don't add if it's already added
                if (query("div[data-value=" + value + "]", this.valuesContainer).length !== 0) {
                    return;
                }

                displayName = displayName.toString();

                var expiredClass = "";

                if (displayName.includes("Expired: ")) {
                    expiredClass = " epi-resourceName--expired";
                }

                var containerDiv = domConstruct.create('div', { 'class': 'epi-categoryButton' });
                var buttonWrapperDiv = domConstruct.create('div', { 'class': 'dijitInline epi-resourceName' + expiredClass, 'title': displayName });
                var categoryNameDiv = domConstruct.create('div', { 'class': 'dojoxEllipsis', innerHTML: displayName, 'title': displayName });


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
                var displayName = text?.toString();

                if (typeof text === 'number' || !isNaN(text)) {
                    if (this.selections && this.selections.length > text) {
                        const selection = this.selections[text];

                        displayName = selection.text;
                        if (!displayName && selection.value) {
                            displayName = selection.value;
                        }
                    }
                }
                else {
                    this.selections.some(function (selection) {
                        if (selection.value === text) {
                            displayName = selection.text;
                            if (!displayName && selection.value) {
                                displayName = selection.value;
                            }
                            return true; // Break
                        }
                    });
                }

                if (displayName === null) {
                    return "";
                }
                return displayName.toString();
            }
        });
    });