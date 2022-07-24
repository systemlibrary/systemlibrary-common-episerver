define([
    "dojo/_base/array",
    "dojo/query",
    "dojo/on",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/dom-construct",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",

    "epi/epi",
    "epi/shell/widget/_ValueRequiredMixin"
],
    function (
        array,
        query,
        on,
        declare,
        lang,
        domConstruct,

        _CssStateMixin,
        _Widget,
        _TemplatedMixin,
        _WidgetsInTemplateMixin,

        epi,
        _ValueRequiredMixin
    ) {
        const moduleFullName = "SystemLibraryCommonEpiserverSelectionPickerWidget";
        const dojoAttachPointName = "SystemLibraryCommonEpiserverSelectionPicker";

        function getStylesheetLink(id, path) {
            var link = document.createElement('link');
            link.id = id;
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.media = 'all';
            link.href = path;
            return link;
        }

        function cleanInput(data) {
            if (typeof (data) === 'undefined' || !data || data === '') {
                return '';
            }
            return data.toString().toLowerCase().replace('#', '');
        }

        function getItemClass(text, value) {
            text = cleanInput(text);
            value = cleanInput(value);

            //Supports both class and inline styling, RGB/className can be in either 'value' or the 'text' variable of a selection
            let classes = dojoAttachPointName + '-item ' +
                dojoAttachPointName + '-item--' + text + ' ' +
                dojoAttachPointName + '-item--' + value +
                ' background-color--' + value +
                ' background-color--' + text;

            return classes;
        }

        function getItemStyle(text, value) {
            if (text && text.toString().includes('#')) {
                return 'background-color:' + text + ';';
            }

            if (value && value.toString().includes('#')) {
                return 'background-color:' + value + ';';
            }
            return '';
        }

        return declare(moduleFullName, [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin], {
            templateString: "<div class='dijitInline'><ul data-dojo-attach-point='" + dojoAttachPointName + "' class='" + dojoAttachPointName + "'></ul></div>",

            intermediateChanges: false,
            value: null,
            isMultiSelect: false,

            onChange: function (value) {
            },

            postCreate: function () {
                this._loadCssFile();
                this._initColors();
                //this.inherited(arguments);
                this._bindEvents(this);
            },

            isValid: function () {
                return !this.required ||
                    (lang.isArray(this.value) && this.value.length > 0 && this.value.join() !== "") ||
                    (typeof (this.value) !== 'undefined' && this.value !== "" && this.value.length > 0)
            },

            _setValueAttr: function (value) {
                //setValueAttr occurs on load, once
                if (!value) {
                    return;
                }

                this._setValue(value, true);
            },

            _setValue: function (value, updateTextbox) {
                try {
                    if (this._started && epi.areEqual(this.value, value)) {
                        return;
                    }

                    if (this.isMultiSelect) {
                        let selected = this.value;

                        if (selected === null) {
                            if (Array.isArray(value)) {
                                selected = value;
                            }
                            else {
                                selected = [];
                                selected.push(value);
                            }
                        } else {
                            if (Array.isArray(value)) {
                                console.warn("Value is an array from an onClick event from a box, this should never happen");
                            } else {
                                if (selected.includes(value)) {
                                    selected = selected.filter(e => e !== value)
                                } else {
                                    selected.push(value);
                                }
                            }
                        }
                        this._set('value', selected);
                    } else {
                        this._set('value', value);
                    }

                    if (updateTextbox) {
                        this._selectBoxes();
                    }

                    if (this._started && this.validate()) {
                        if (this.value !== null) {
                            this.onChange(this.value);
                        }
                    }
                }
                catch (e) {
                    console.error(e);
                }
            },

            _setReadOnlyAttr: function (value) {
                //this._set("readOnly", value);
            },

            _setIntermediateChangesAttr: function (value) {
                this._set("intermediateChanges", value);
            },

            _selectBoxes: function () {
                try {
                    let colors = this.SystemLibraryCommonEpiserverSelectionPicker.getElementsByTagName('a');

                    if (!colors || !colors.length) {
                        return;
                    }

                    let selected = this.value;

                    if (selected === null) {
                        return;
                    }

                    for (var i = 0; i < colors.length; i++) {
                        color = colors[i];
                        if (!color) {
                            continue;
                        }

                        let div = color.getElementsByTagName('div')[0];
                        if (!div) {
                            continue;
                        }

                        let value = color.getAttribute('data-value');
                        let text = color.getAttribute('data-text');
                        let img = color.getAttribute('data-img');

                        let classes = getItemClass(text, value);
                        let styles = getItemStyle(text, value);

                        if (img !== null && img !== "" && img.length > 3 &&
                            (img.includes('.jpg') || img.includes('.gif') || img.includes('.png'))) {
                            styles = styles + ' background-image: url(' + img + ');';
                        }

                        if (this.isMultiSelect) {
                            if (selected.includes(value.toString())) {
                                classes = classes + ' ' + dojoAttachPointName + '--item-selected';
                            } else {
                                console.warn("SelectionPicker: multiselect deselecting a value");
                            }
                        } else {
                            if (selected && value.toString() === selected.toString()) {
                                classes = classes + ' ' + dojoAttachPointName + '--item-selected';
                            }
                        }

                        div.setAttribute('class', classes);
                        div.setAttribute('style', styles);

                        color.appendChild(div);
                    }
                }
                catch {
                    console.warn("Error failed in mark chosen color in list");
                }
            },

            _bindEvents: function (myself) {
                on(query(this.SystemLibraryCommonEpiserverSelectionPicker).query("a"), "click", function (e) {
                    try {
                        myself._setValue(e.currentTarget.getAttribute("data-value"), true);

                        e.preventDefault();
                    }
                    catch (e) {
                        console.error(e);
                    }
                });

            },

            _loadCssFile: function () {
                try {
                    var cssPrefixId = 'systemLibraryCommonEpiserverSelectionPickerCss';
                    if (!document.getElementById(cssPrefixId + '1')) {
                        //var head = document.getElementsByTagName('head')[0];
                        var head = document.getElementsByTagName('body')[0];

                        var css1 = getStylesheetLink(cssPrefixId + '1', '/SystemLibrary/Common/Episerver/UiHint/SelectionPicker/Style');

                        head.appendChild(css1);
                    }
                }
                catch (e) {
                    console.error(e);
                }
            },

            _initColors: function () {
                try {
                    const list = this.SystemLibraryCommonEpiserverSelectionPicker;
                    const colors = this.selections;

                    if (this.metadata && this.metadata.additionalValues &&
                        this.metadata.additionalValues["multiselect"] === true) {
                        this.isMultiSelect = true;
                    }

                    colors.forEach(color => {
                        let text = color.text;
                        let value = color.value;
                        let img = null;
                        if (text.startsWith("ERROR:")) {
                            console.error(text);
                        }

                        if (typeof (value) !== 'undefined' && value && value.toString().includes('___')) {
                            let tmp = value.split('___');
                            value = tmp[0];
                            if (tmp.length > 1) {
                                img = tmp[1];
                            }
                        }

                        let classes = getItemClass(text, value);
                        let styles = getItemStyle(text, value);

                        let div = document.createElement('div');
                        div.setAttribute('class', classes);
                        div.setAttribute('style', styles);

                        let textLowered = text.toString().toLowerCase();
                        if (textLowered !== 'unset') {
                            div.innerText = color.text;
                        }

                        if (text === '') {
                            text = " ";
                        }

                        let a = document.createElement('a');
                        a.href = '#';
                        a.title = text;
                        a.setAttribute('data-img', img);
                        a.setAttribute('data-text', text);
                        a.setAttribute('data-value', value);

                        a.appendChild(div);

                        let li = document.createElement('li');
                        li.appendChild(a);

                        list.appendChild(li);
                    });
                }
                catch (e) {
                    console.error(e);
                }
            },
        });
    });
