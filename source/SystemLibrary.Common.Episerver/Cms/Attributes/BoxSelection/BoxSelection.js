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
        const moduleFullName = "systemLibraryCommonEpiserverBoxSelectionWidget";
        const dojoAttachPointName = "systemLibraryCommonEpiserverBoxSelection";

        function getStylesheetLink(id, path) {
            var link = document.createElement('link');
            link.id = id;
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.media = 'all';
            link.href = path;
            return link;
        }

        function getCssClassName(data) {
            if (typeof (data) === 'undefined' || !data || data === '') {
                return null;
            }
            if (data.includes(".gif") || data.includes(".png") || data.includes(".jpg") ||
                data.includes("/") || data.includes("\\")) {
                return null;
            }
            if (data.includes('!') || data.includes('(') || data.includes(',') || data.includes('#')) {
                return null;
            }

            return data.toString().toLowerCase().replaceAll(' ', '-');
        }

        function getItemClass(text, value, additional) {
            let textCssName = getCssClassName(text);
            let valueCssName = getCssClassName(value);
            let additionalCssName = getCssClassName(additional);

            let classes = dojoAttachPointName + '-item';

            if (textCssName !== null && textCssName.length > 1) {
                classes += ' ' + dojoAttachPointName + '-item--' + textCssName + ' background-color--' + textCssName;
            }
            if (valueCssName !== null && valueCssName.length > 1) {
                classes += ' ' + dojoAttachPointName + '-item--' + valueCssName + ' background-color--' + valueCssName;
            }
            if (additionalCssName !== null && additionalCssName.length > 1) {
                classes += ' ' + dojoAttachPointName + '-item--' + additionalCssName + ' background-color--' + additionalCssName;
            }

            return classes;
        }

        function getInlineCssBackgroundColorValue(data) {
            if (typeof (data) === 'undefined' || !data || data === '') {
                return null;
            }

            let txt = data.toString();

            if (txt.includes("/") || txt.includes("\\")) {
                return null;
            }

            if (txt.includes(".gif") || txt.includes(".jpg") || txt.includes(".png")) {
                return null;
            }

            if (txt.includes('#')) {
                return txt;
            }
            if (txt.includes('(') && txt.includes(')') && txt.includes(',')) {
                if (txt.includes('rgb')) {
                    return txt;
                } else {
                    return "rgba" + txt;
                }
            }
            if (txt.includes(',') && txt.length >= 5 && txt.length <= 11) {
                if (txt.includes('rgb')) {
                    return txt;
                } else {
                    return "rgba(" + txt + ")";
                }
            }
            return null;
        }

        function getItemStyle(text, value, additional) {
            let textBackgroundCss = getInlineCssBackgroundColorValue(text);
            let valueBackgroundCss = getInlineCssBackgroundColorValue(value);
            let additionalBackgroundCss = getInlineCssBackgroundColorValue(additional);

            let styles = '';

            if (textBackgroundCss !== null) {
                styles += 'background-color:' + textBackgroundCss + ';';
            }
            if (valueBackgroundCss !== null) {
                styles += 'background-color:' + valueBackgroundCss + ';';
            }
            if (additionalBackgroundCss !== null) {
                styles += 'background-color:' + additionalBackgroundCss + ';';
            }

            let foundImage = false;
            if (typeof (additional) !== 'undefined' && additional && additional !== "" && additional.length > 1) {
                let temp = additional.toLowerCase();
                if (temp.includes('.jpg') || temp.includes('.gif') || temp.includes('.png')) {
                    if (!temp.includes('#') && !temp.includes(',')) {
                        foundImage = true;
                        styles = styles + ' background-image: url(' + additional + ');';
                    }
                }
            }

            if (foundImage === false) {
                if (typeof (value) !== 'undefined' && value !== null && value.length > 3) {
                    let temp = value.toLowerCase();
                    if (temp.includes('.jpg') || temp.includes('.gif') || temp.includes('.png')) {
                        if (!temp.includes('#') && !temp.includes(',')) {
                            styles = styles + ' background-image: url(' + value + ');';
                        }
                    }
                }
            }

            return styles;
        }

        function isReallyEqual(value1, value2) {
            if (typeof (value1) === 'undefined') {
                value1 = null;
            }
            if (typeof (value2) === 'undefined') {
                value2 = null;
            }
            if (value1 === value2) {
                return true;
            }
            if (epi.areEqual(value1, value2)) {
                return true;
            }
            if (value1 !== null && value2 !== null) {
                return value1.toString() === value2.toString();
            }
            return false;
        }

        return declare(moduleFullName, [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin], {
            templateString: "<div class='dijitInline'><ul data-dojo-attach-point='" + dojoAttachPointName + "' class='" + dojoAttachPointName + "'></ul></div>",

            intermediateChanges: false,
            value: null,

            postCreate: function () {
                this._loadCssFile();
                this._initWidgetProperties();
                this._bindEvents(this);
                this.inherited(arguments);
            },

            //always invoked on initial load by Epi, value is true if 'readonly' attribute has been added to the property
            _setReadOnlyAttr: function (value) {
                this._set("readOnly", value);
            },

            //always invoked on initial load by Epi, value is current value from the database
            _setValueAttr: function (value) {
                this._setValue(value, true);
            },

            //Commented out: never invoked it seems
            // _setIntermediateChangesAttr: function (value) {
            //     this._set("intermediateChanges", value);
            // },

            isValid: function () {
                return !this.required ||
                    (lang.isArray(this.value) && this.value.length > 0 && this.value.join() !== "") ||
                    (typeof (this.value) !== 'undefined' && this.value !== "" && this.value.length > 0)
            },

            _setValue: function (value, initialLoad) {
                try {
                    if (!this._started) {
                        return;
                    }

                    if (typeof (value) === 'undefined') {
                        value = null;
                    }

                    if (this.isMultiSelect) {
                        let selected = this.value;

                        if (selected === null) {
                            if (value !== null && Array.isArray(value)) {
                                selected = value;
                            }
                            else {
                                selected = [];
                                selected.push(value);
                            }
                        } else {
                            if (Array.isArray(value)) {
                                console.warn("Value is an array from an onClick event from a box, this should never happen");
                                return;
                            } else {
                                if (selected.includes(value)) {
                                    if (this.allowUnselection) {
                                        selected = selected.filter(e => e !== value)
                                    } else {
                                        if (selected.length === 1) {
                                            console.warn("allowUnselection is false: cannot unselect the value leaving the list empty");
                                            return;
                                        }
                                        selected = selected.filter(e => e !== value)
                                    }
                                } else {
                                    selected.push(value);
                                }
                            }
                        }
                        this._set('value', selected);

                    } else {
                        if (initialLoad) {
                            this._set('value', value);
                        }
                        else {
                            if (this.allowUnselection !== true) {
                                if (isReallyEqual(this.value, value)) {
                                    console.warn("allowUnselection is false: cannot unselect the value");
                                    return;
                                } else {
                                    this._set('value', value);
                                }
                            } else {
                                if (isReallyEqual(this.value, value)) {
                                    this._set('value', null);
                                } else {
                                    this._set('value', value);
                                }
                            }
                        }
                    }

                    this._selectBoxes();

                    if (initialLoad) {
                        return;
                    }

                    if (this._started && this.validate()) {
                        //invoke built-in onChange method to trigger epi events
                        this.onChange(this.value);
                    }
                }
                catch (e) {
                    console.error(e);
                }
            },

            _selectBoxes: function () {
                try {
                    let colors = this.systemLibraryCommonEpiserverBoxSelection.getElementsByTagName('a');

                    if (!colors || !colors.length) {
                        return;
                    }

                    let selected = this.value;

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
                        let additional = color.getAttribute('data-additional');

                        let classes = getItemClass(text, value, additional);
                        let styles = getItemStyle(text, value, additional);

                        if (this.isMultiSelect) {
                            if (selected.includes(value.toString())) {
                                classes = classes + ' ' + dojoAttachPointName + '--item-selected';
                            } else {
                                //console.warn("BoxSelection: multiselect deselecting a value");
                            }
                        } else {
                            if (isReallyEqual(value, selected)) {
                                classes = classes + ' ' + dojoAttachPointName + '--item-selected';
                            }
                        }

                        div.setAttribute('class', classes);
                        div.setAttribute('style', styles);

                        color.appendChild(div);
                    }
                }
                catch (e) {
                    console.error(e);
                    console.error("Error failed in mark chosen color in list");
                }
            },

            _bindEvents: function (myself) {
                on(query(this.systemLibraryCommonEpiserverBoxSelection).query("a"), "click", function (e) {
                    myself._setValue(e.currentTarget.getAttribute("data-value"));
                    e.preventDefault();
                });
            },

            _loadCssFile: function () {
                try {
                    var cssPrefixId = 'systemLibraryCommonEpiserverBoxSelectionCss';
                    if (!document.getElementById(cssPrefixId + '1')) {
                        var head = document.getElementsByTagName('body')[0];

                        var css1 = getStylesheetLink(cssPrefixId + '1', '/SystemLibrary/Common/Episerver/UiHint/BoxSelection/Style');

                        head.appendChild(css1);
                    }
                }
                catch (e) {
                    console.error(e);
                }
            },

            _initWidgetProperties: function () {
                try {
                    const list = this.systemLibraryCommonEpiserverBoxSelection;
                    const colors = this.selections;

                    colors.forEach(color => {
                        let text = color.text;
                        let value = color.value;
                        let additional = null;
                        if (text.startsWith("ERROR:")) {
                            console.error(text);
                        }

                        if (typeof (value) !== 'undefined' && value && value.toString().includes('__d_')) {
                            let tmp = value.split('__d_');
                            value = tmp[0];
                            if (tmp.length > 1) {
                                additional = tmp[1];
                            }
                        }

                        let classes = getItemClass(text, value, additional);

                        let styles = getItemStyle(text, value, additional);

                        let div = document.createElement('div');
                        div.setAttribute('class', classes);
                        div.setAttribute('style', styles);

                        let textLowered = text.toString().toLowerCase();
                        if (textLowered !== 'unset' && textLowered !== 'none' && textLowered !== 'no') {
                            if (textLowered !== 'yes' && textLowered !== 'checked' && textLowered !== 'enabled') {
                                div.innerText = color.text;
                            }
                        }

                        if (text === '') {
                            text = " ";
                        }

                        let a = document.createElement('a');
                        a.href = '#';
                        a.title = text;
                        a.setAttribute('data-additional', additional);
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
