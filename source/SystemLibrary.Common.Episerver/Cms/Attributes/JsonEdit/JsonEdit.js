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
                return true;
            },

            isEqual: function (value1, value2) {
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
            },

            _onEditJsonClick: function (jsonEditorUrl) {
                try {
                    const json = this.jsonTextArea.value;

                    const width = screen.availWidth - 30;
                    const height = screen.availHeight - 30;

                    const d = new Date();
                    const skipCacheParam = 'skipCache=' + d.getHours().toString() + d.getMinutes().toString() + d.getMilliseconds().toString();

                    const windowFeatures = "left=100,top=100,width=" + width + ",height=" + height;
                    const w = window.open(jsonEditorUrl + '?' + skipCacheParam, "Editor", windowFeatures);

                    if (!w) {
                        alert("Error: could not open the window for unknown reason. Try disabling ad-blockers? Try disabling other blocking extensions? Try in normal chrome, not inkognito or vice versa?");
                    }

                    w.json = json;

                    w.onSave = lang.hitch(this, function (newJson) {
                        this.onFocus();

                        let jsonText = JSON.stringify(newJson);

                        this._setValueAttr(jsonText);       //This should be "setValue()"?

                        this.onChange(jsonText);            //OnChange is sufficient, why setValueAttr?

                        this.isShowingEditorWindow = false;
                    });

                    if (!w.focus) {
                        w.focus();
                    }

                    this.isShowingEditorWindow = true;
                } catch (e) {
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

                    this.jsonTextArea.value = value;

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

            _onTextAreaChanged: function (value) {
                this._setValue(value);
            },

            postCreate: function () {
                try {
                    let typeName = this.typeName;
                    let jsonSchema = this.jsonSchema;
                    let jsonEditorUrl = this.jsonEditorUrl;

                    console.log("VALUES FROM C#");
                    console.log(typeName);
                    console.log(jsonSchema);
                    console.log(jsonEditorUrl);

                    this._bindEvents(this, () => this._onEditJsonClick(jsonEditorUrl), () => this._onTextAreaChanged);
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

                on(myself.jsonTextArea, "onchange", function (e) {
                    textAreaOnChange();
                    e.preventDefault();
                });
            },
        }
        );
    });
