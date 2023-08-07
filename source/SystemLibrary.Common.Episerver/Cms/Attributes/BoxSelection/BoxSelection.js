try {

    define([
        "dojo/query",
        "dojo/on",
        "dojo/_base/declare",
        "dojo/_base/lang",

        "dijit/_CssStateMixin",
        "dijit/_Widget",
        "dijit/_TemplatedMixin",
        "dijit/_WidgetsInTemplateMixin",

        "epi/epi",
        "epi/shell/widget/_ValueRequiredMixin"
    ],
        function (
            query,
            on,
            declare,
            lang,

            _CssStateMixin,
            _Widget,
            _TemplatedMixin,
            _WidgetsInTemplateMixin,

            epi,
            _ValueRequiredMixin
        ) {
            const moduleFullName = "systemLibraryCommonEpiserverBoxSelectionWidget";
            const dojoAttachPointName = "systemLibraryCommonEpiserverBoxSelection";

            function is(data) {
                if (typeof (data) === 'undefined' || data === null || data === "" || data == -1 || (data.length && data.length === 0)) {
                    return false;
                }
                return true;
            }

            function isImageUrl(url) {
                if (!is(url)) {
                    return false;
                }

                url = url.toString();

                if (url.startsWith("~/") && url.includes('.') && url[url.length - 1] !== '.') {
                    return true;
                }

                if (url.includes(".svg") || url.includes(".gif") || url.includes(".png") || url.includes(".jpg")) {
                    return url.includes("/") || url.includes("\\");
                }
                return false;
            }

            function isHex(data) {
                if (!is(data)) {
                    return false;
                }
                data = data.toString();

                return data.length && data.length <= 7 && data.startsWith('#') && !data.includes(' ');
            }

            function isRgbColor(data) {
                if (!is(data)) {
                    return false;
                }

                data = data.toString();

                if (data.length && data.length > 22) {
                    return false;
                }

                if (data.includes('.') || data.includes('/') || data.includes('\\')) {
                    return false;
                }

                if (data.includes('(') && data.includes(')') && data.includes(',')) {
                    let temp = data.toLowerCase();
                    return temp.includes('rgb');
                }
                return false;
            }

            function isColorValue(data) {
                if (!is(data)) {
                    return false;
                }
                if (data.length) {
                    if (data.length > 16 || data.length < 5) {
                        return false;
                    }
                }

                data = data.toString();

                if (data.includes('.') || data.includes('/') || data.includes('\\')) {
                    return false;
                }
                if (data.includes(',')) {
                    return !(/[a-zæøåÆØÅ!?.]+$/i.test(data));
                }
                return false;
            }

            function getStylesheetLink(id, path) {
                var link = document.createElement('link');
                link.id = id;
                link.rel = 'stylesheet';
                link.type = 'text/css';
                link.media = 'all';
                link.href = path;
                return link;
            }

            function getCssName(data) {
                if (!is(data) || isImageUrl(data) || isHex(data) || isColorValue(data) || isRgbColor(data)) {
                    return null;
                }

                data = data.toString();

                if (data.includes('[') || data.includes('(') || data.includes('#')) {
                    return null;
                }

                return data.toString().toLowerCase().replaceAll(' ', '-').replaceAll('!', '-').replaceAll(',', '-').replaceAll('.', '-');
            }

            function appendCssClass(data) {
                var cssName = getCssName(data);
                if (cssName !== null) {
                    return ' ' + dojoAttachPointName + '-item--' + cssName + ' background-color--' + cssName;
                }
                return '';
            }

            function getBackgroundColorValue(data) {

                if (!is(data)) {
                    return null;
                }

                if (isImageUrl(data)) {
                    return null;
                }

                if (isHex(data)) {
                    return data;
                }

                if (isRgbColor(data)) {
                    return data;
                }

                data = data.toString();

                if (isColorValue(data)) {
                    if (data.includes('(')) {
                        return "rgba" + data;
                    }
                    return "rgba(" + data + ")";
                }

                return null;
            }

            function getContainerCssClass(text, value, additional) {
                let css = dojoAttachPointName + '-item';

                css += appendCssClass(text);
                css += appendCssClass(value);
                css += appendCssClass(additional);

                if (is(text) && (isImageUrl(value) || isImageUrl(additional))) {
                    css += ' systemLibraryCommonEpiserverBoxSelection--item-image';
                }

                return css;
            }

            function appendContainerInlineBackground(data) {
                if (is(data)) {
                    // data contains comma, and each "word" is either Hex, or Rgb, or "no css class" (dash, underscores), then it is 
                    // built in colors, hex or rgb values
                    data = data.toString();
                    if (data.includes('-') && !data.includes("/")) {
                        let colors = data.split('-');
                        if (colors.length === 2) {
                            let bottom = getBackgroundColorValue(colors[0].replace(' ', ''));
                            if (bottom === null) {
                                bottom = colors[0].replace(' ', '');
                            }
                            let top = getBackgroundColorValue(colors[1].replace(' ', ''));
                            if (top === null) {
                                top = colors[1].replace(' ', '');
                            }
                            return 'background-image: linear-gradient(to top left, ' + bottom + ' 48%, ' + top + ' 51%); text-shadow: 1.25px 1.25px #FFF;';
                        }
                    }
                }

                let backgroundValue = getBackgroundColorValue(data);
                if (backgroundValue !== null) {
                    return 'background-color:' + backgroundValue;
                }
                return "";
            }

            function getContainerInlineStyle(text, value, additional) {
                let inline = '';

                inline += appendContainerInlineBackground(text);
                inline += appendContainerInlineBackground(value);
                inline += appendContainerInlineBackground(additional);

                let foundImage = false;

                if (is(additional)) {
                    let temp = additional.toLowerCase();
                    if (isImageUrl(temp)) {
                        foundImage = true;

                        if (additional.startsWith('~')) {
                            additional = additional.substring(1);
                        }

                        let size = '36%';
                        if (!text || text === '') {
                            size = '48%';
                        }

                        inline += ' background-image:url(' + additional + ');background-size: ' + size + ';';

                        if (is(text)) {
                            inline += 'background-position-y: 2px;align-items:end;';
                        }
                    }
                }

                if (foundImage === false) {
                    if (is(value)) {
                        value = value.toString();
                        let temp = value.toLowerCase();

                        if (value.startsWith('~')) {
                            value = value.substring(1);
                        }

                        if (isImageUrl(temp)) {
                            inline += ' background-image:url(' + value + ');';
                        }
                    }
                }

                //NOTE: Not supporting 'EnumText()' in backend with an image url per now, it can only be "display name" or "blank"

                return inline;
            }

            function isEqual(value1, value2) {
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
                    if (!this.isMultiSelect) {
                        this.value = '';
                    }
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
                    if (!this.required) {
                        return true;
                    }

                    if (typeof (this.value) === 'undefined') return false;

                    return true;

                    //                    return (lang.isArray(this.value) && this.value.length && this.value.length > 0 && this.value.join() !== "") ||
                    //                      (this.value !== "" && this.value.length && this.value.length > 0)
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
                                    console.warn("BoxSelection: value is an array from an onClick event from a box, this should never happen");
                                    return;
                                } else {
                                    if (selected.includes(value)) {
                                        if (this.allowUnselection) {
                                            selected = selected.filter(e => e !== value)
                                        } else {
                                            if (selected.length === 1) {
                                                console.warn("BoxSelection: allowUnselection is false, cannot unselect the value leaving the list empty");
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
                                    if (isEqual(this.value, value)) {
                                        console.warn("BoxSelection: allowUnselection is false, cannot unselect the value");
                                        return;
                                    } else {
                                        this._set('value', value);
                                    }
                                } else {
                                    if (isEqual(this.value, value)) {
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
                        console.error("BoxSelection: _setValue");
                        console.error(e);
                    }
                },

                _selectBoxes: function () {
                    try {
                        let boxes = this.systemLibraryCommonEpiserverBoxSelection.getElementsByTagName('a');

                        if (!is(boxes)) {
                            console.warn("BoxSelection: no items returned from the backend to select from");
                            return;
                        }

                        let selected = this.value;

                        for (var i = 0; i < boxes.length; i++) {
                            let box = boxes[i];
                            if (!is(box)) {
                                continue;
                            }
                            let div = box.getElementsByTagName('div')[0];
                            if (!is(div)) {
                                console.warning("Warn: no div to select, dijit rendering changed?");
                                continue;
                            }

                            let value = box.getAttribute('data-value');
                            let text = box.getAttribute('data-text');
                            let additional = box.getAttribute('data-additional');

                            let css = getContainerCssClass(text, value, additional);
                            let inline = getContainerInlineStyle(text, value, additional);

                            if (this.isMultiSelect) {
                                if (selected.includes(value.toString())) {
                                    css = css + ' ' + dojoAttachPointName + '--item-selected';
                                } else {
                                    //console.warn("BoxSelection: multiselect deselecting a value");
                                }
                            } else {
                                if (isEqual(value, selected)) {
                                    css = css + ' ' + dojoAttachPointName + '--item-selected';
                                } else {
                                    // Initial selection is null, the "value" from html is ofc empty as the attribute exists
                                    // so check if html is "" and current value is null, then its a "match"
                                    if (value === "" && selected === null) {
                                        css = css + ' ' + dojoAttachPointName + '--item-selected';
                                    }
                                    else {
                                        // Initial selection is 0, when type is an Enum in backend
                                        // without specifying a default, if theres a 0 in the Enum, then that is selected
                                        if (value == 0 && selected === null) {
                                            css = css + ' ' + dojoAttachPointName + '--item-selected';
                                        }
                                    }
                                }
                            }

                            div.setAttribute('class', css);
                            div.setAttribute('style', inline);

                            box.appendChild(div);
                        }
                    }
                    catch (e) {
                        console.error(e);
                        console.error("BoxSelection: Error marking clicked box in the list");
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

                            var css1 = getStylesheetLink(cssPrefixId + '1', '/SystemLibrary/CommonEpiserverCms/BoxSelection/Style');

                            head.appendChild(css1);
                        }
                    }
                    catch (e) {
                        console.error("BoxSelection: loadCssFile");
                        console.error(e);
                    }
                },

                _initWidgetProperties: function () {
                    try {
                        const list = this.systemLibraryCommonEpiserverBoxSelection;
                        const boxes = this.selections;

                        if (!is(boxes)) {
                            console.warn("BoxSelection: No boxes from the backend");
                            return;
                        }
                        boxes.forEach(box => {
                            let text = box.text;
                            let value = box.value;
                            let additional = null;

                            if (is(text)) {
                                text = text.toString();
                            }

                            if (is(text) && text.startsWith("ERROR: ")) {
                                console.error(text);
                            }
                            if (!is(text)) {
                                text = "";
                            }
                            if (!is(value)) {
                                value = "";
                            }

                            if (value != "-1" && value != "0") {
                                if (value.includes('__d_')) {
                                    let tmp = value.split('__d_');
                                    value = tmp[0];
                                    if (tmp.length > 1) {
                                        additional = tmp[1];
                                    }
                                }
                                else if (isImageUrl(value)) {
                                    additional = value;
                                }
                            }

                            let css = getContainerCssClass(text, value, additional);

                            let inline = getContainerInlineStyle(text, value, additional);

                            let div = document.createElement('div');
                            div.setAttribute('class', css);
                            div.setAttribute('style', inline);

                            let textLowered = text.toString().toLowerCase();
                            if (textLowered !== 'unset' && textLowered !== 'none' && textLowered !== 'no') {
                                if (textLowered !== 'yes' && textLowered !== 'checked' && textLowered !== 'enabled') {
                                    div.innerHTML = '<span>' + box.text + '</span>';
                                }
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
                        console.error("BoxSelection: _initWidgetProperties");
                        console.error(e);
                    }
                },
            });
        });
}
catch {
    console.error("BoxSelection.js crashed on load");
}