define([
    "dojo/_base/declare",
    "dojo/on",
    "dojo/_base/lang",

    "dijit/_Widget",
    "dijit/_TemplatedMixin",

    "epi/shell/widget/_ValueRequiredMixin",

    'dojo/text!./Html'
],
    function (
        declare,
        on,
        lang,

        _Widget,
        _TemplatedMixin,

        _ValueRequiredMixin,

        html
    ) {

        function is(data) {
            if (typeof (data) === 'undefined' || data === null || data === "" || (data.length && data.length === 0)) {
                return false;
            }
            if (data === " " || data === "  " || data === "    ") {
                return false;
            }
            return true;
        }

        function valuesAreEqual(value1, value2) {
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
        }

        const attachPointName = "someComponent";
        const moduleName = attachPointName + "Module";

        return declare(moduleName, [
            _Widget,
            _TemplatedMixin,
            _ValueRequiredMixin], {
            templateString: html,

            _setReadOnlyAttr: function (value) {
                this._set("readOnly", value);
            },

            _setValueAttr: function (value) {
                this._set('value', value);
                this.Textarea.value = value;
                this._setCounterValue(value);
            },

            _setTextareaValue: function (value) {
                if (!this._started) {
                    return;
                }

                if (valuesAreEqual(this.value, value)) {
                    return;
                }

                this._set('value', value);

                this.onChange(value);
            },

            _setCounterValue: function (value) {
                this.Counter.innerHTML = "";
                if (value && value.length > 1) {
                    try {
                        let jsonObject = JSON.parse(value);
                        if (jsonObject !== null) {
                            if (Array.isArray(jsonObject)) {
                                this.Counter.innerHTML = "Items: " + jsonObject.length;
                            }
                        }
                    } catch {
                    }
                }
            },

            _SetFeedbackValue: function (message) {
                console.error(message);
                this.Feedback.innerHTML = message;
            },

            _onOpenJsonEditorClick: function (jsonEditTitle, jsonEditSortByPropertyName1, jsonEditSortByPropertyName2, jsonEditProperties, jsonEditorUrl, jsonIsArray) {
                try {
                    if (!is(jsonEditTitle)) {
                        jsonEditTitle = 'Editor';
                    }

                    let jsonEditValue = this.Textarea.value;
                    if (!is(jsonEditValue)) {
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

                    if (!is(jsonEditProperties)) {
                        jsonEditProperties = '';
                    }

                    const jsonEditPropertiesObject = JSON.parse('{' + jsonEditProperties + '}');

                    if (!is(jsonEditPropertiesObject) || !is(jsonEditPropertiesObject.required)) {
                        throw "Error: Fields from backend was not loaded properly, try refresh";
                    }

                    if (Array.isArray(jsonEditPropertiesObject)) {
                        console.warn(jsonEditProperties);
                        throw "Error: schema properties is an array, it should be an object with 'required' and 'properties' variables";
                    }

                    if (!is(jsonEditSortByPropertyName1)) {
                        jsonEditSortByPropertyName1 = null;
                    }
                    if (!is(jsonEditSortByPropertyName2)) {
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

                        this._setTextareaValue(jsonText);

                        this._setCounterValue(jsonText);

                        this.Textarea.value = jsonText;

                        this.onChange(jsonText);

                        this.isShowingEditorWindow = false;

                        return true;
                    });

                    if (!w.focus) {
                        w.focus();
                    }

                    this.isShowingEditorWindow = true;

                } catch (e) {
                    this._SetFeedbackValue(e.toString());
                }
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
                    if (is(selections) && selections.forEach) {
                        selections.forEach(selection => {
                            let text = selection.text;
                            if (text && text.startsWith("ERROR: ")) {
                                this._SetFeedbackValue(text);
                            }
                        });
                    }
                    this._bindEvents(this,
                        (value) => this._setTextareaValue(value),
                        () => this._onOpenJsonEditorClick(jsonEditTitle, jsonEditSortByPropertyName1, jsonEditSortByPropertyName2, jsonEditProperties, jsonEditorUrl, jsonIsArray));
                }
                catch (err) {
                    console.error(err);
                }
            },

            _bindEvents: function (self, onKeyUp, onOpenJsonEditor) {
                on(self.Textarea, "keyup", function (e) {
                    onKeyUp(e.target.value);
                });

                on(self.Button, "click", function (e) {
                    onOpenJsonEditor();
                })
            },
        }
        );
    });