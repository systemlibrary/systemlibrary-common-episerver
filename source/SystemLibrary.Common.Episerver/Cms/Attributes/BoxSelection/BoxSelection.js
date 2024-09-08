try {

    define([
        "dojo/query",
        "dojo/on",
        "dojo/_base/declare",

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
                if (typeof (data) === 'undefined' || data === null || data === "" || data == -9999 || (data.length && data.length === 0)) {
                    return false;
                }
                return true;
            }

            function getAdditionalValue(value) {
                if (value?.toString().includes('__d_')) {
                    return value.toString().split('__d_')[1];
                }
                return null;
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

                if (data.length && data.length > 23) {
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

            function getInitialValue(storeAsEnum, isMultiSelect, value) {
                if (typeof value === "undefined") {
                    value = null;
                }
                if (value === null) {
                    if (storeAsEnum) {
                        if (isMultiSelect) {
                            value = new Array();
                        } else {
                            value = 0;
                        }
                    } else {
                        if (isMultiSelect) {
                            value = new Array();
                        } else {
                            value = null;
                        }
                    }
                }
                // obj.additionalValue = getAdditionalValue(value);
                return value;
            }

            function getCssName(data) {
                if (!is(data) || isImageUrl(data) || isHex(data) || isColorValue(data) || isRgbColor(data)) {
                    return null;
                }

                data = data.toString();

                if (data.includes('"') || data.includes('<') || data.includes('[') || data.includes('(') || data.includes('#')) {
                    return null;
                }

                return data.toString().toLowerCase()
                    .replaceAll(':', '')
                    .replaceAll('?', '')
                    .replaceAll('&', '')
                    .replaceAll(' ', '-')
                    .replaceAll('!', '-')
                    .replaceAll(',', '-')
                    .replaceAll('.', '-');
            }

            function appendCssClass(data) {
                var cssName = getCssName(data);
                if (cssName !== null) {
                    return ' '
                        + dojoAttachPointName + '-item--' + cssName
                        + ' background-color--' + cssName
                        + ((isNaN(cssName) && cssName?.length > 1) ? ' ' + cssName : '');
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

                if (value !== text) {
                    css += appendCssClass(value);
                }

                if (additional !== value && additional !== text) {
                    css += appendCssClass(additional);
                }

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
                    if (data.includes("--") && !data.includes("/")) {
                        let colors = data.split("--");
                        if (colors.length === 2) {
                            let bottom = getBackgroundColorValue(colors[0].replace(' ', ''));
                            if (bottom === null) {
                                bottom = colors[0].replace(' ', '');
                            }
                            let top = getBackgroundColorValue(colors[1].replace(' ', ''));
                            if (top === null) {
                                top = colors[1].replace(' ', '');
                            }
                            return 'background-image: linear-gradient(to top left, ' + top + ' 48%, ' + bottom + ' 51%); text-shadow: 2px 1px 5px white, 0 0px 6px white, 0 0 12px white;';
                        }
                    }
                }

                let backgroundValue = getBackgroundColorValue(data);
                if (backgroundValue) {
                    backgroundValue = backgroundValue.toString();
                    if (backgroundValue.startsWith('#')) {
                        if (backgroundValue.length === 7) {
                            if (backgroundValue[1] <= 5 && backgroundValue[3] <= 5 && backgroundValue[5] <= 5) {
                                return 'background-color:' + backgroundValue + ";color:white;";
                            }
                        }
                        else if (backgroundValue.includes('#000')) {
                            return 'background-color:' + backgroundValue + ";color:white;";
                        }
                    }

                    if (backgroundValue.startsWith('rgb')) {
                        if (backgroundValue.length <= 13) {
                            return 'background-color:' + backgroundValue + ";text-shadow: 2px 1px 5px white, 0 0px 6px white, 0 0 12px white;";
                        }
                        if (backgroundValue.startsWith('rgba')) {
                            if (backgroundValue.length <= 14) {
                                return 'background-color:' + backgroundValue + ";text-shadow: 2px 1px 5px white, 0 0px 6px white, 0 0 12px white;";
                            }
                        }
                    }

                    if (backgroundValue.startsWith('(')) {
                        if (backgroundValue.length <= 11) {
                            return 'background-color:' + backgroundValue + ";text-shadow: 2px 1px 5px white, 0 0px 6px white, 0 0 12px white;";
                        }
                    }
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
                        let temp = value.toString().toLowerCase();

                        if (isImageUrl(temp)) {
                            if (value.startsWith('~')) {
                                value = value.substring(1);
                            }
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
                    try {
                        this._loadCssFile();
                        this.inherited(arguments);
                        this._initWidgetProperties();
                        this.value = getInitialValue(this.storeAsEnum, this.isMultiSelect, null);
                        this._bindEvents(this);

                        if (this.value === 0) {
                            this._selectBoxes();
                        } else {
                            console.warn("value is not 0, do not init select boxes");
                        }
                    }
                    catch (err) {
                        console.error(err);
                    }
                },

                //always invoked on initial load by Epi, value is true if 'readonly' attribute has been added to the property
                _setReadOnlyAttr: function (value) {
                    this._set("readOnly", value);
                },

                //always invoked on initial load by Epi, value is current value from the database
                _setValueAttr: function (v) {
                    v = getInitialValue(this.storeAsEnum, this.isMultiSelect, v);
                    this._set('value', v);
                    this._selectBoxes();
                },

                isValid: function () {
                    if (!this.required) {
                        return true;
                    }

                    if (typeof (this.value) === 'undefined' || this.value === null) return false;

                    return true;
                },

                _setValue: function (v) {
                    try {
                        if (!this._started) {
                            return;
                        }

                        if (typeof (v) === 'undefined') {
                            v = null;
                        }
                        if (this.storeAsEnum && (v === null || v === '')) {
                            v = 0;
                        }

                        if (this.isMultiSelect) {
                            let storedValue = this.value;
                            let isSelected = storedValue.toString().includes(v.toString());
                            if (isSelected) {
                                if (!this.allowUnselection) {
                                    if (storedValue.length === 1) {
                                        console.warn("BoxSelection: allowUnselection is false, cannot unselect the value leaving the list empty");
                                        return;
                                    }
                                }
                                storedValue = storedValue.filter(boxValue => {
                                    return boxValue?.toString() !== v?.toString()
                                });
                            } else {
                                if (this.storeAsEnum) {
                                    storedValue.push(Number(v));
                                } else {
                                    storedValue.push(v);
                                }
                            }
                            this._set('value', storedValue);
                        } else {
                            if (this.storeAsEnum) {
                                if (isEqual(this.value, v)) {
                                    if (this.allowUnselection) {
                                        this._set('value', 0);
                                    } else {
                                        console.warn("AllowUnselection: false, cannot deselect...");
                                    }
                                } else {
                                    this._set('value', Number(v));
                                }
                            } else {
                                if (isEqual(this.value, v)) {
                                    if (this.allowUnselection) {
                                        this._set('value', null);
                                    } else {
                                        console.warn("AllowUnselection: false, cannot deselect...");
                                    }
                                }
                                else {
                                    this._set('value', v);
                                }
                            }
                        }
                    }
                    catch (e) {
                        console.error("BoxSelection: _setValue: ");
                        console.error(e);
                        console.log(v);
                    }
                    this._selectBoxes();
                    if (this._started && this.validate()) {
                        //invoke built-in onChange method to trigger epi events
                        this.onChange(this.value);
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
                                if (selected && selected.toString().includes(value.toString())) {
                                    css = css + ' ' + dojoAttachPointName + '--item-selected';
                                } else {
                                    // deselecting/not selected item, continue...
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
                                        else {
                                            // deselecting/not selected item, continue...
                                        }
                                    }
                                }
                            }

                            div.setAttribute('class', css);
                            div.setAttribute('style', inline);
                            // box.appendChild(div);
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

                        if (!list) {
                            console.warn("'systemLibraryCommonEpiserverBoxSelection' is not existing");
                            return;
                        }

                        if (!is(boxes)) {
                            console.warn("BoxSelection: No boxes from the backend");
                            return;
                        }

                        boxes.forEach(box => {
                            let text = box.text;
                            let value = box.value;
                            let additional = null;
                            if (typeof (value) === "undefined") {
                                value = null;
                            }

                            if (is(text) || text === 0) {
                                text = text.toString();
                            }

                            if (is(text) && text.startsWith("ERROR: ")) {
                                console.error(text);
                            }

                            if (!is(text)) {
                                text = "";
                            }

                            if (!is(value) && value !== 0) {
                                if (this.storeAsEnum) {
                                    value = 0;
                                } else {
                                    value = "";
                                }
                            }

                            if (is(value)) {
                                if (value?.toString().includes('__d_')) {
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
                                    if (box.text.length > 32) {
                                        div.innerHTML = '<span class="small">' + box.text + '</span>';
                                    } else {
                                        div.innerHTML = '<span>' + box.text + '</span>';
                                    }

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