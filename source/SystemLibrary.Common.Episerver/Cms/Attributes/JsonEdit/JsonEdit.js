try {

    define([
        "dojo/_base/declare",
        "dojo/_base/lang",

        "dojo/on",

        "dijit/_CssStateMixin",
        'dijit/_WidgetBase',
        "dijit/_Widget",
        "dijit/_TemplatedMixin",
        "dijit/_WidgetsInTemplateMixin",

        "epi/epi",

        'dojo/text!./Html'
    ],
        function (
            declare,
            lang,

            on,

            _CssStateMixin,
            _WidgetBase,
            _Widget,
            _TemplatedMixin,
            _WidgetsInTemplateMixin,

            epi,

            template
        ) {
            return declare("systemLibrary.Common.Episerver.JsonEdit", [
                _Widget,
                _TemplatedMixin], {
                templateString: template,

                is: function (data) {
                    if (typeof (data) === 'undefined' || data === null || data === "" || (data.length && data.length === 0)) {
                        return false;
                    }
                    if (data === " " || data === "  " || data === "    ") {
                        return false;
                    }
                    return true;
                },

                isEqual: function (value1, value2) {
                    if (!(!!value1)) {
                        value1 = null;
                    }
                    if (!(!!(value2))) {
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
                },

                setCount: function (count) {
                    if (count > 1) {
                        this.jsonTextCount.innerHTML = "Items: " + count;
                    } else {
                        this.jsonTextCount.innerHTML = "";
                    }
                },

                setError: function (message) {
                    this.jsonTextErrorMessage.innerHTML = message;
                },

                _onEditJsonClick: function (jsonEditTitle, jsonEditSortByPropertyName1, jsonEditSortByPropertyName2, jsonEditProperties, jsonEditorUrl, jsonIsArray) {
                    try {
                        if (!this.is(jsonEditTitle)) {
                            jsonEditTitle = 'Editor';
                        }

                        let jsonEditValue = this.jsonTextArea.value;
                        if (!this.is(jsonEditValue)) {
                            if (jsonIsArray == 'false') {
                                jsonEditValue = '{}';
                            } else {
                                jsonEditValue = '[]';
                            }
                        }

                        if (!jsonEditValue.startsWith('[') && !jsonEditValue.startsWith('{')) {
                            console.warn(jsonEditValue);
                            throw "Error: Json value is not an array nor an object, must start with either { or [";
                        }

                        let jsonEditObject = JSON.parse(jsonEditValue);

                        if (!Array.isArray(jsonEditObject) && typeof (jsonEditObject) === 'object') {
                            jsonEditObject = [jsonEditObject];
                        }

                        if (!Array.isArray(jsonEditObject)) {
                            console.warn(jsonEditValue);
                            throw "Error: Json Value is not an array nor an object";
                        }

                        if (!this.is(jsonEditProperties)) {
                            jsonEditProperties = '';
                        }

                        const jsonEditPropertiesObject = JSON.parse('{' + jsonEditProperties + '}');

                        if (!this.is(jsonEditPropertiesObject) || !this.is(jsonEditPropertiesObject.required)) {
                            throw "Error: Fields from backend was not loaded properly, try refresh";
                        }

                        if (Array.isArray(jsonEditPropertiesObject)) {
                            console.warn(jsonEditProperties);
                            throw "Error: schema properties is an array, it should be an object with 'required' and 'properties' variables";
                        }

                        if (!this.is(jsonEditSortByPropertyName1)) {
                            jsonEditSortByPropertyName1 = null;
                        }
                        if (!this.is(jsonEditSortByPropertyName2)) {
                            jsonEditSortByPropertyName2 = null;
                        }

                        const width = (screen.availWidth / 2) + 60;
                        const height = (screen.availHeight / 1.2) + 33;

                        const left = (screen.availWidth - width) / 2;

                        const d = new Date();
                        const skipCacheParam = 'skipCache=' + d.getHours().toString() + d.getMinutes().toString() + d.getMilliseconds().toString();

                        const windowFeatures = "left=" + left + ",top=166,width=" + width + ",height=" + height + ",status=no,menubar=no,directories=no,titlebar=no,location=no";
                        const w = window.open(jsonEditorUrl + '?' + skipCacheParam, "Editor", windowFeatures);

                        if (!w) {
                            alert("Error: could not open the window for unknown reason. Try disabling ad-blockers? Try disabling other blocking extensions? Try in normal chrome, not inkognito or vice versa?");
                            return;
                        }
                        w.jsonEditTitle = jsonEditTitle;
                        w.jsonEditValue = jsonEditObject;
                        w.jsonIsArray = jsonIsArray;
                        w.jsonEditPropertiesObject = jsonEditPropertiesObject;
                        w.jsonEditSortByPropertyName1 = jsonEditSortByPropertyName1;
                        w.jsonEditSortByPropertyName2 = jsonEditSortByPropertyName2;

                        w.onSave = lang.hitch(this, function (newJson) {
                            this.onFocus();

                            let jsonText = JSON.stringify(newJson);

                            this._setValueAttr(jsonText);       //This should be "setValue()"?

                            this.onChange(jsonText);            //OnChange is sufficient, why setValueAttr?

                            this.isShowingEditorWindow = false;

                            return true;
                        });

                        if (!w.focus) {
                            w.focus();
                        }

                        this.isShowingEditorWindow = true;

                    } catch (e) {
                        this.setError(e.toString());
                        console.error(e);
                    }
                },

                //always invoked on initial load by Epi, value is true if 'readonly' attribute has been added to the property
                _setReadOnlyAttr: function (value) {
                    this._set("readOnly", value);
                },

                isValid: function () {
                    return true;
                },

                //always invoked on initial load by Epi, value is current value from the database
                _setValueAttr: function (value) {
                    this._setValue(value, true);

                    if (value && value.length) {
                        let jsonObject = JSON.parse(value);
                        if (jsonObject !== null) {
                            if (Array.isArray(jsonObject)) {
                                this.setCount(jsonObject.length);
                            }
                        }
                    }
                },

                _setValue: function (value, initialLoad) {
                    if (!this._started) {
                        return;
                    }

                    try {
                        if (typeof (value) === 'undefined') {
                            value = null;
                        }

                        if (this.isEqual(this.value, value)) {
                            return;
                        }

                        this._set('value', value);

                        if (this.jsonTextArea.value !== value) {
                            this.jsonTextArea.value = value;
                        }

                        if (initialLoad) {
                            return;
                        }
                        if (this._started && this.isValid()) {
                            this.onChange(this.value);
                        }
                    }
                    catch (e) {
                        console.error(e);
                    }
                },

                _onTextAreaChanged: function (value) {
                    this._setValue(value);
                },

                postCreate: function () {
                    try {
                        let jsonEditTitle = this.jsonEditTitle;
                        let jsonIsArray = this.jsonIsArray;
                        let jsonEditSortByPropertyName1 = this.jsonEditSortByPropertyName1;
                        let jsonEditSortByPropertyName2 = this.jsonEditSortByPropertyName2;
                        let jsonEditProperties = this.jsonEditProperties;
                        let jsonEditorUrl = this.jsonEditorUrl;

                        const selections = this.selections;
                        if (this.is(selections) && selections.forEach) {
                            selections.forEach(selection => {
                                let text = selection.text;
                                if (this.is(text) && text.startsWith("ERROR: ")) {
                                    console.error(text);
                                    this.setError(text);
                                }
                            });
                        }

                        this._bindEvents(this,
                            () => this._onEditJsonClick(jsonEditTitle, jsonEditSortByPropertyName1, jsonEditSortByPropertyName2, jsonEditProperties, jsonEditorUrl, jsonIsArray),
                            (v) => this._onTextAreaChanged(v));
                    }
                    catch (e) {
                        console.error(e);
                    }
                },

                _bindEvents: function (myself, jsonEditOnClick, textAreaOnChange) {
                    on(myself.editButton, "click", function (e) {
                        jsonEditOnClick();
                        e.preventDefault();
                    });

                    on(myself.jsonTextArea, "keyup", function (e) {
                        textAreaOnChange(e.target.value);
                    });

                    on(myself.jsonTextErrorMessage, 'onset', function (e) { });

                    on(myself.jsonTextCount, 'onset', function (e) { });
                },
            }
            );
        });
}
catch {
    console.error("JsonEdit.js crashed during loading");
}