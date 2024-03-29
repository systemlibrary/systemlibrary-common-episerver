﻿/*jshint esversion: 10 */

window.systemLibraryJsonEditorGlobalTextArea = document.createElement("textarea");

(function () {
    'use strict';
}());

(function ($) {
    if (!$) {
        console.error("Error in $, jQuery not loaded");
    }
    $.fn.jsonEdit = function (options) {
        var renderPlace = this;
        var level = 0;
        var arrayTemplates = {};

        function setLocalStorageData(name, value) {
            try {
                localStorage.setItem(name, JSON.stringify(value));
            } catch (err) {
                console.error("Error in setLocalStorageData()");
                console.error(err);
            }
        }

        function getLocalStorageData(name) {
            return localStorage.getItem(name);
        }

        function initOptions() {
            options = options || {};

            try {
                options["storageDataName"] = options["storageDataName"] || 'systemLibraryCommonEpiserverJsonEditStorageDataName';
                options["expandingLevel"] = (options["expandingLevel"] == null ? -1 : options["expandingLevel"]); // -1:expand all
                options["jsonIsArray"] = options["jsonIsArray"];
                options["value"] = options["value"] || {};
                options["schema"] = options["schema"] || {};
                options["renderFirstLevel"] = options["renderFirstLevel"] || 'false';
                options["autoTrimValues"] = options["autoTrimValues"] || 'true';
                options["identing"] = options["identing"] || 3;
                options["radioNullCaption"] = options["radioNullCaption"] || 'null';
                options["selectNullCaption"] = options["selectNullCaption"] || '';
                options["treeExpandCollapseButton"] = options["treeExpandCollapseButton"] || 'true';

                var currentStoredData = getLocalStorageData(options["storageDataName"]);
                if (currentStoredData) {
                    if (options["value"].data) {
                        var currentValue = JSON.stringify(options["value"].data);
                        var storedLength = currentStoredData.length;
                        if (storedLength) {
                            if (!currentValue || currentValue.length > storedLength + 1 || currentValue.length < storedLength - 1) {
                                if (confirm("Found a previous edit. Click 'OK' to continue from last edit found in your browser or click 'Cancel' to continue with data from server.\n\nReasons you see this:\n You've X'd (or Escape'd) the window instead of clicking Save or Cancel with edits\nSession might've expired while you edited\n\nTip: Unsure? Click 'OK' to view data, then click 'X' on window if youre unhappy with what you see, then click edit again, but this time click 'Cancel'") == true) {
                                    options["value"].data = JSON.parse(currentStoredData);
                                }
                            }
                            else {
                                console.warn("JsonEdit: previous temp data is equal to currently loaded data, continuing as normal...");
                            }
                        }
                    }
                }
                else {
                    console.warn("JsonEdit: no previous temp data stored");
                }
            } catch {
                console.error("Error in initOptions()");
                console.error("JsonEdit: Error reading from localStorage, continue as normal...");
            }
        }

        function initNodeTitle() {
            $('.j-node-title').each((i, node) => {
                $(node).closest('.j-container').each((j, b) => {
                    $(b).find('.j-input-text:first').each((k, c) => {
                        if (c.value && c.value.length > 0) {
                            $(node).text(c.value);
                        }
                    });
                });
            });
        }

        function updateUIOnJsonIsArrayIsFalse() {
            if (jsonIsArray == 'false') {
                document.getElementsByClassName('j-add-array-item')[0].style.display = 'none';
                document.getElementById('collapseExpandAll').style.display = 'none';
                document.getElementsByClassName('jsonEditor-container-menu')[0].style.display = 'none';

                renderPlace.find(".j-object-title-row-ec").click();
            }
        }

        function initWidget() {
            level = 0;
            arrayTemplates = {};

            var widgetContent = renderSchemaNode(options["schema"], "");
            renderPlace.html(widgetContent);

            initValuePaths();
            setValue(options["value"]);
            initEvents();
            validateWidget();
            updateUIOnJsonIsArrayIsFalse();

            if (options["afterWidgetCreated"]) options["afterWidgetCreated"](options["value"], options["schema"]);

            setTimeout(initNodeTitle, 20);
        }

        function validateWidget() {
            $(".j-input").each(function () {
                validateInput($(this));
            });
        }

        function validateInput(elm) {
            try {
                var v_required = fixNU(elm.attr("data-required"), "false"),
                    v_min = elm.attr("data-min"), v_max = elm.attr("data-max");

                v_min = (v_min ? parseFloat(v_min) : null);
                v_max = (v_max ? parseFloat(v_max) : null);

                if (elm.hasClass("j-input-text") || elm.hasClass("j-input-textarea") || elm.hasClass("j-input-select") || elm.hasClass("j-input-number") || elm.hasClass("j-input-date")) {
                    elm.attr("data-is-valid", (v_required == "true" && fixNU(elm.val(), "") == "" ? "false" : "true"));
                    if (elm.attr("data-is-valid") == "false") return;
                    if (v_min) {
                        if (elm.hasClass("j-input-text") || elm.hasClass("j-input-textarea")) {
                            elm.attr("data-is-valid", (elm.val().length < v_min ? "false" : "true"));
                            if (elm.attr("data-is-valid") == "false") return;
                        }
                        if (elm.hasClass("j-input-number")) {
                            elm.attr("data-is-valid", (elm.val() < v_min ? "false" : "true"));
                            if (elm.attr("data-is-valid") == "false") return;
                        }
                    }
                    if (v_max) {
                        if (elm.hasClass("j-input-text") || elm.hasClass("j-input-textarea")) {
                            elm.attr("data-is-valid", (elm.val().length > v_max ? "false" : "true"));
                            if (elm.attr("data-is-valid") == "false") return;
                        }

                        if (elm.hasClass("j-input-number")) {
                            elm.attr("data-is-valid", (elm.val() > v_max ? "false" : "true"));
                            if (elm.attr("data-is-valid") == "false") return;
                        }
                    }
                }
                if (elm.hasClass("j-input-email")) {
                    let regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/; // Regex validation for international number
                    elm.attr("data-is-valid", (regex.test(elm.val())));
                }
                if (elm.hasClass("j-input-tel")) {
                    let regex = /^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$/; // Regex validation for email
                    elm.attr("data-is-valid", (regex.test(elm.val())));
                }
                if (elm.hasClass("j-input-checkbox")) {
                    elm.attr("data-is-valid", (v_required == "true" && elm.prop("checked") == false ? "false" : "true"));
                }

                if (elm.hasClass("j-input-html")) {
                    let v = (elm.next(".j-input-html-div:first").text() == "");
                    elm.next(".j-input-html-div:first").attr("data-is-valid", (v_required == "true" && v ? "false" : "true"));
                }
            }
            catch (err) {
                console.error("Error in validateInput");
                console.error(err);
            }
        }

        function isValid() {
            try {
                var v = renderPlace.find('[data-is-valid="false"]:first').length;
                return (v > 0 ? false : true);
            } catch (err) {
                console.error("Error in isValid() " + err);
            }
        }

        function onRemoveArrayItemClicked(e, ele) {
            e.preventDefault();
            e.stopPropagation();

            if (window.confirm('Sure to delete this item?')) {
                removeArrayItem(ele);
            }

            return true;
        }

        function initEvents() {
            renderPlace.find(".j-ec").off("click").on("click", function (e) { toggleSubTree(this); e.preventDefault(); return true; });
            renderPlace.find(".j-object-title-row-ec").off("click").on("click", function (e) { toggleSubTree(this); e.preventDefault(); return true; });
            // renderPlace.find(".j-title-col").off("click").on("click", function (e) { alert('nice'); e.preventDefault(); return true; });

            renderPlace.find(".j-add-array-item").off("click").on("click", function () {
                addArrayItem($(this), true, null);

                var textareaoptions = {
                    buttons: ['bold', 'italic', 'link', 'mark']
                };

                var richtextareas = $('.j-input-rich-textarea');
                $.each(richtextareas, function (i) {
                    var richtextarea = richtextareas[i];
                    if (richtextarea.classList.length < 3) {
                        $(richtextarea).easyEditor({
                            options: textareaoptions
                        });
                    }
                });
            });
            renderPlace.find(".j-remove-array-item").off("click").on("click", function (e) { onRemoveArrayItemClicked(e, $(this)); });
            renderPlace.find(".j-input-text,.j-input-textarea,.j-input-date,.j-input-number,.j-input-email,.j-input-tel").off("keyup").on("keyup", function () { valueChanged($(this)); });
            renderPlace.find(".j-input-checkbox,.j-input-radio,.j-input-select,.j-input-color,.j-input-date,.j-input-number,.j-input-html").off("change").on("change", function () { valueChanged($(this)); });
            //renderPlace.find(".j-input-html-div").off("keyup").on("keyup", function () { changeInput($(this)); });
            renderPlace.find(".j-input-rich-textarea").on("change", function () { changeInput($(this)); });
            //renderPlace.find(".j-input-html").off("focus").on("focus", function () { $(this).parents("td:first").find(".j-input-html-div:first").focus(); });
        }

        function changeInput(htmlDiv) {
            try {
                var parents = htmlDiv.parents(":first");
                var input = parents.find("input:first");
                if (input) {
                    if (input.length == 0) {
                        input = parents.find("textarea:first");
                    }
                    if (input && input.length > 0) {
                        var oldValue = input.val();
                        input.val(htmlDiv.html());
                        var newValue = input.val();
                        if (oldValue !== newValue) {
                            // class/attribute changed only, not the input value itself
                            valueChanged(input);
                        }
                    }
                }
            }
            catch (err) {
                console.error("Error in changeInput()");
                console.error(err);
            }
        }

        function removeArrayItem(arrItem) {
            try {
                var itemIndex = arrItem.attr("data-index");
                var nodeToRemove = arrItem.parents("table:first");
                var p = nodeToRemove.parents("td:first").attr("data-path");
                eval('options["value"]' + p + "[" + itemIndex + "] = null;");
                //eval('options["value"]' + p + '.splice(' + itemIndex + ',1);');
                nodeToRemove.remove();

                setValue(options["value"]);

                setLocalStorageData(options["storageDataName"], options["value"].data, 14);

                if (options["afterValueChanged"]) options["afterValueChanged"](options["value"], options["schema"]);
            }
            catch (err) {
                console.error("Error in removeArrayItem()");
                console.error(err);
            }
        }

        function addArrayItem(arrayContainer, needInitiations, itemIndex) {
            try {
                var tId = arrayContainer.attr("data-template-id");
                var htmlTemplate = arrayTemplates[tId]["htmlTemplate"];
                var dataTemplate = JSON.parse(JSON.stringify(arrayTemplates[tId]["dataTemplate"]));
                var dataPath = arrayContainer.parents("tr:first").next().find("td:first").attr("data-path");
                if (V(options["value"], dataPath) == undefined || V(options["value"], dataPath) == null) {
                    eval('options["value"]' + dataPath + "=[];");
                }

                if (itemIndex == null) {
                    var arrLen = null;
                    eval('options["value"]' + dataPath + ".push(dataTemplate);");
                    eval('arrLen = options["value"]' + dataPath + ".length;");
                    itemIndex = arrLen - 1;
                }

                htmlTemplate = replaceAll(htmlTemplate, "$index$", itemIndex);
                arrayContainer.parents("tr:first").next().find("td:first").append(htmlTemplate);
                if (needInitiations) {
                    initValuePaths();
                    initEvents();
                }

            }
            catch (err) {
                console.error("Error in addArrayItem");
                console.error(err);
            }
        }

        function valueChanged(changedObject) {
            try {

                ensureDataPath(changedObject.attr("data-path"));

                var p = 'options["value"]' + changedObject.attr("data-path");
                if (changedObject.prop("tagName").toLowerCase() == "input" && changedObject.prop("type").toLowerCase() == "checkbox") {
                    p = p + "=" + (changedObject.prop("checked") == true ? "true" : "false") + ";";
                } else {
                    if (changedObject.prop("type") === "number" || changedObject.prop("type") === "select-one") {
                        p = p + "=" + (options["autoTrimValues"] == "true" ? jsonEscape(changedObject.val()).trim() : jsonEscape(changedObject.val())) + ";";

                        if (p.endsWith('=;')) {
                            p = p.substring(0, p.length - 1);
                            if (changedObject.prop("type") === "number") {
                                p = p + "0;";
                            } else {
                                p = p + "null;";
                            }
                        }
                    } else {
                        p = p + "=`" + (options["autoTrimValues"] == "true" ? htmlDecode(jsonEscape(changedObject.val())).trim() : htmlDecode(jsonEscape(changedObject.val()))) + "`;";
                        if (p.endsWith('=;')) {
                            p = p.substring(0, p.length - 1);
                            p = p + "'';";
                        }
                    }
                }
                try {
                    eval(p);
                }
                catch (ex) {
                    console.error("Error in EVAL in valueChanged()");
                    console.error(ex);
                }

                validateInput(changedObject);

                setLocalStorageData(options["storageDataName"], options["value"].data, 14);

                if (options["afterValueChanged"]) options["afterValueChanged"](options["value"], options["schema"]);
            }
            catch (err) {
                console.error("Error in valueChanged()");
                console.error(err);
            }
        }

        function renderSchemaNode(schemaNode, schemaName, requiredItems) {
            var nodeType = fixNU(schemaNode["type"], "string");
            if (nodeType == "string" || nodeType == "number" || nodeType == "integer" || nodeType == "boolean" || nodeType == "email" || nodeType == "tel" || nodeType == 'textarea') {
                return renderSimpleNode(schemaNode, schemaName, (requiredItems ? requiredItems.includes(schemaName) : false));
            }
            if (nodeType == "array") {
                return renderArrayNode(schemaNode, schemaName);
            }
            if (nodeType == "object") {
                return renderObjectNode(schemaNode, schemaName);
            }
            return "";
        }

        function renderSimpleNode(schemaNode, schemaName, isRequired) {
            var ContainerT = '<table $hover-hint$ class="j-container"><tr class="j-object-value-row">$$$</tr></table>';
            var TitleT = '<td class="j-title-col">$$$</td><td class="j-sep-col"></td>';
            var BodyT = '<td class="j-body-col">$$$</td>';
            var requiredAtt = "", requiredStar = "", inputBody = "";
            var additionalClass = " " + getUISetting(schemaNode, "class", "");
            var nodeType = fixNU(schemaNode["type"], "string");
            var hoverHint = getUISetting(schemaNode, "hoverHint", "");
            var placeholderHint = getUISetting(schemaNode, "placeholderHint", "");
            var displayName = getUISetting(schemaNode, "displayName", "");
            var inlineHint = getUISetting(schemaNode, "inlineHint", "");
            var validationHint = getUISetting(schemaNode, "validationHint", "");
            var dataValueNameAtt = ' data-value-name="' + schemaName + '" ';
            var classAtt = "";

            var min = "1900-01-01";   //min and max value in "date" input type
            var max = "3000-01-01";
            var dateplaceholder = "dd.mm.yyyy"; //if editor is "date", this dateplaceholder is used as date format

            if (hoverHint != "") hoverHint = 'title="' + hoverHint + '"';
            if (placeholderHint != "") placeholderHint = ' placeholder="' + placeholderHint + '" ';
            if (inlineHint != "") inlineHint = '<div class="j-inline-help">' + inlineHint + '</div>';
            if (validationHint != "") validationHint = '<div class="j-validation-help">' + validationHint + '</div>';
            if (isRequired) {
                requiredAtt = ' data-required="true" ';
                requiredStar = '&nbsp;<span class="j-required-star">*</span>';
            }

            if (nodeType == "textarea") {
                classAtt = ' class="j-input j-input-rich-textarea' + additionalClass + '" ';
                inputBody = '<textarea ' + classAtt + dataValueNameAtt + requiredAtt + ' />';
            }
            else if (nodeType == "boolean") {
                classAtt = ' class="j-input j-input-checkbox' + additionalClass + '" ';
                inputBody = '<input type="checkbox" ' + classAtt + dataValueNameAtt + requiredAtt + ' />';
            } else {
                if (fixNU(schemaNode["enum"], "") == "") {
                    var editor = getUISetting(schemaNode, "editor", "text"), htmlEditor = "", minAtt = "", maxAtt = "";
                    if (nodeType == "email") editor = "email";
                    if (nodeType == "tel") editor = "tel";
                    if (nodeType == "number" || nodeType == "integer") editor = "number";
                    if (editor == "html") htmlEditor = '<div class="j-input j-input-html-div" ' + requiredAtt + ' contenteditable></div>';
                    minAtt = fixNU(schemaNode["minLength"], "") + fixNU(schemaNode["minimum"], "");
                    maxAtt = fixNU(schemaNode["maxLength"], "") + fixNU(schemaNode["maximum"], "");
                    if (minAtt != "") minAtt = ' data-min="' + minAtt + '" ';
                    if (maxAtt != "") maxAtt = ' data-max="' + maxAtt + '" ';
                    if (editor == "textarea") {
                        classAtt = ' class="j-input j-input-textarea' + additionalClass + '" ';
                        inputBody = '<textarea ' + classAtt + dataValueNameAtt + requiredAtt + minAtt + maxAtt + '></textarea>';
                    } else {
                        classAtt = ' class="j-input j-input-' + editor + additionalClass + '" ';
                        inputBody = (editor == "color" ? "&nbsp;&nbsp;" : "") + '<input type="' + editor + '" '
                            + (editor == "date" ? "min='" + min + "' max='" + max + "' placeholder='" + dateplaceholder + "' " : "")
                            + classAtt + dataValueNameAtt + placeholderHint + requiredAtt + minAtt + maxAtt + ' />' + htmlEditor;
                    }
                } else {
                    let editor = getUISetting(schemaNode, "editor", "select");
                    classAtt = ' class="j-input j-input-' + editor + additionalClass + '" ';

                    if (editor == "radio") {
                        var nameAtt = ' name="rdo_' + schemaName + '" ';
                        inputBody = '<label><input checked value="' + options["radioNullCaption"] + '" ' + classAtt + ' type="radio" ' + nameAtt
                            + dataValueNameAtt + requiredAtt + ' /><span class="j-input-radio-label">null</span></label>&nbsp;&nbsp;&nbsp;';
                        jQuery.each(schemaNode["enum"], function (index) {
                            inputBody += '<label><input value="' + schemaNode["enum"][index] + '" type="radio" ' + classAtt + nameAtt + dataValueNameAtt + requiredAtt
                                + ' /><span class="j-input-radio-label">' + schemaNode["enum"][index] + '</span></label>&nbsp;&nbsp;&nbsp;';
                        });
                    }

                    if (editor == "select") {
                        if (!isRequired) inputBody = '<option selected="true">' + options["selectNullCaption"] + '</option>';
                        jQuery.each(schemaNode["enum"], function (index) {
                            inputBody += '<option value="' + index + '">' + schemaNode["enum"][index] + '</option>';
                        });
                        inputBody = '<select ' + classAtt + dataValueNameAtt + requiredAtt + '>' + inputBody + "</select>";
                    }
                }
            }

            BodyT = BodyT.replace("$$$", inputBody + validationHint + inlineHint);
            if (typeof (displayName) !== 'undefined' && displayName && displayName.length > 1) {
                TitleT = TitleT.replace("$$$", getSpacer(level) + getEC('') + displayName + requiredStar);
            } else {
                TitleT = TitleT.replace("$$$", getSpacer(level) + getEC('') + getTitle(schemaNode, schemaName) + requiredStar);
            }
            return ContainerT.replace("$$$", TitleT + BodyT).replace("$hover-hint$", hoverHint);
        }

        function renderObjectNode(schemaNode, schemaName) {
            var ContainerT = '<table class="j-container">$$$</table>';
            var TitleT = '<tr class="j-object-title-row j-object-title-row-ec collapsed" tabindex="0"><td class="j-title-col">$$$</td><td class="j-body-col">$inlinehint$</td></tr>';
            var childClass = ((options["expandingLevel"] != -1 && level + 1 > options["expandingLevel"]) ? "j-collapsed" : "");
            if (level > 0) {
                childClass = 'j-collapsed';
            }
            var ecBtn = (childClass == "j-collapsed" ? "e" : "c");
            var properties = Object.keys(schemaNode["properties"]);
            var inlineHint = getUISetting(schemaNode, "inlineHint", "");
            if (inlineHint != "") inlineHint = '<span class="j-inline-help">' + inlineHint + '</span>';
            TitleT = TitleT.replace("$inlinehint$", inlineHint);
            var temp = "";

            var BodyT = '<tr class="j-object-properties ' + childClass + '"><td colspan="3" data-value-name="' + schemaName + '">$$$</td></tr>';
            TitleT = TitleT.replace("$$$", getSpacer(level) + getEC(ecBtn) + getTitle(schemaNode, schemaName));
            TitleT = (options["renderFirstLevel"] == "false" && level == 0 ? "" : TitleT);

            level = level + 1;
            properties.forEach(function (item, index, arr) {
                temp += renderSchemaNode(schemaNode["properties"][item.toString()], item.toString(), schemaNode["required"]);
            });
            level = level - 1;
            return ContainerT.replace("$$$", TitleT + BodyT.replace("$$$", temp));
        }

        function renderArrayNode(schemaNode, schemaName) {
            var ContainerT = '<div>$$$</div>';
            var TitleT = '<div class="j-array-title-row">$$$&nbsp;$ArrTools$&nbsp;$inlinehint$</div>';
            var childClass = ((options["expandingLevel"] != -1 && level + 1 > options["expandingLevel"]) ? "" : "");
            //var ecBtn = (childClass == "j-collapsed" ? "e" : "c");
            var itemTemplateId = schemaName + "_" + level;
            var BodyT = '<tr class="' + childClass + '"><td colspan="3" data-value-name="' + schemaName + '" class="j-array-items">$$$</td></tr>';
            var inlineHint = getUISetting(schemaNode, "inlineHint", "");
            if (inlineHint != "") inlineHint = '<div class="j-inline-help">' + inlineHint + '</div>';
            TitleT = TitleT.replace("$$$", getTitle(schemaNode, schemaName));
            TitleT = TitleT.replace("$ArrTools$", '<button class="j-add-array-item" data-array-loaded="false" data-template-id="' + itemTemplateId + '">&#43;</button>');
            TitleT = TitleT.replace("$inlinehint$", inlineHint);

            var arrType = getArrayType(schemaNode);
            var itemDataTemplate = null, itemContainerT = null;
            var arrSchema = { "title": "", "type": arrType };
            level++;

            if (arrType == "string" || arrType == "number" || arrType == "boolean" || arrType == "email" || arrType == "tel" || arrType == "textarea") {
                if (schemaNode["items"] && schemaNode["items"]["ui"]) arrSchema["ui"] = schemaNode["items"]["ui"];
                if (schemaNode["items"] && schemaNode["items"]["enum"]) arrSchema["enum"] = schemaNode["items"]["enum"];
                var space = "";
                if (arrSchema["title"] && arrSchema["title"].length > 0) {
                    space = " ";
                }
                arrSchema["title"] = arrSchema["title"] + space + '[$index$] <button class="j-remove-array-item" tabindex="-1" data-index="$index$">&#10006;</button>';
                itemContainerT = renderSimpleNode(arrSchema, "$index$");
            }

            if (arrType.startsWith("#")) {
                var r = "['" + replaceAll(arrType.replace('#/', ""), '/', "']['") + "']";
                var space = "";
                if (arrSchema["title"] && arrSchema["title"].length > 0) {
                    space = " ";
                }

                arrSchema = JSON.parse(JSON.stringify(V(options["schema"], r)));
                arrSchema["title"] = fixNU(arrSchema["title"], "") + '[$index$] <span class="j-node-title"></span> <button class="j-remove-array-item" tabindex="-1" data-index="$index$">&#10006;</button>';
                itemContainerT = renderSchemaNode(arrSchema, "$index$");
            }
            level--;

            arrayTemplates[itemTemplateId] = { "htmlTemplate": itemContainerT, "dataTemplate": itemDataTemplate };
            return ContainerT.replace("$$$", TitleT + BodyT.replace("$$$", ""));
        }

        function getTitle(schemaNode, schemaName) {
            return '<label>' + fixNU(schemaNode["title"], schemaName) + "</label>";
        }

        function getIdBasedDataPath(dataPath) {
            var n = replaceAll(dataPath, '][', '_');
            n = replaceAll(n, '[', '');
            n = replaceAll(n, ']', '');
            n = replaceAll(n, '"', '');
            n = replaceAll(n, "'", '');
            return renderPlace.attr("id") + "_" + n;
        }

        function initValuePaths() {
            renderPlace.find("[data-value-name]").each(function () {
                var dp = generatePath($(this));
                $(this).attr("data-path", dp);
                if (dp) {

                    $(this).attr("id", getIdBasedDataPath(dp));
                    $(this).parents("table:first").find("label:first").attr("for", $(this).attr("id"));
                }
            });
        }

        function generatePath(o) {
            var p = [];
            o.parents("[data-value-name]").each(function () {
                p.push("['" + $(this).attr("data-value-name") + "']");
            });
            var r = p.reverse().join(".");
            r = replaceAll(r, ".[", "[");
            r = replaceAll(r, "['']", "");
            r = r + "['" + o.attr("data-value-name") + "']";
            return r;
        }

        function setValue(v) {
            try {
                addArrayItemsToTheDOM();
                renderPlace.find("input[data-path],select[data-path],textarea[data-path]").each(function () {

                    if ($(this).prop("tagName").toLowerCase() == "input" && $(this).prop("type").toLowerCase() == "checkbox") {
                        $(this).prop("checked", V(v, $(this).attr("data-path")) == true ? true : false);
                    } else if ($(this).prop("tagName").toLowerCase() == "input" && $(this).prop("type").toLowerCase() == "radio") {
                        //$('[data-path="' + $(this).attr("data-path") + '"]').prop("checked", false);
                        $('[data-path="' + $(this).attr("data-path") + '"][value="' + V(v, $(this).attr("data-path")) + '"]').prop("checked", true);
                    } else {
                        if ($(this).prop("type") === "textarea") {
                            var _temp = V(v, $(this).attr("data-path"));
                            $(this).val(_temp);
                        }
                        else if (options["autoTrimValues"] == "true") {
                            var _temp = V(v, $(this).attr("data-path"));

                            if (_temp && _temp.trim) _temp = _temp.trim();

                            $(this).val(_temp);
                        } else {
                            $(this).val(V(v, $(this).attr("data-path")));
                        }
                        if ($(this).hasClass("j-input-html")) {
                            $(this).parents(":first").find(".j-input-html-div:first").html($(this).val());
                        }
                    }
                });
                options["value"] = v;
            }
            catch (err) {
                console.error("Error in setValue()");
                console.error(err);
            }
        }

        function addArrayItemsToTheDOM() {
            var arrayNodes = renderPlace.find('[data-array-loaded="false"]');
            if (arrayNodes.length == 0) {
                initValuePaths();
                return;
            }
            arrayNodes.each(function () {
                var addArrayItemBtn = $(this);
                var dataPath = addArrayItemBtn.parents("tr:first").next("tr").find("td:first").attr("data-path");
                if (dataPath === undefined) {
                    var o = addArrayItemBtn.parents("tr:first").next("tr").find("td:first");
                    o.attr("data-path", generatePath(o));
                    dataPath = addArrayItemBtn.parents("tr:first").next("tr").find("td:first").attr("data-path");
                }
                var arr = null;
                eval('arr = options["value"]' + dataPath + ';');
                if (arr) {
                    arr.forEach(function (item, index, arr) {
                        addArrayItem(addArrayItemBtn, false, index);
                    });
                }
                addArrayItemBtn.attr("data-array-loaded", "true");
            });

            addArrayItemsToTheDOM();
        }

        function getValue() {
            return options["value"];
        }

        function getSchema() {
            return options["schema"];
        }

        function getUISetting(schemaNode, name, ifNU) {
            return (schemaNode["ui"] && schemaNode["ui"][name] ? schemaNode["ui"][name] : ifNU);
        }

        function getArrayType(schemaNode) {
            var type = (schemaNode["items"] != undefined && schemaNode["items"]["type"] != undefined ? schemaNode["items"]["type"] : "");
            if (type == "") {
                type = (schemaNode["items"] != undefined && schemaNode["items"]["$ref"] != undefined ? schemaNode["items"]["$ref"] : "string");
            }
            return type;
        }

        function ensureDataPath(dataPath) {
            try {
                var pathParts = replaceAll(dataPath, "][", "].[").split('.');
                var pathCursor = "";
                pathParts.forEach(function (item, index, arr) {
                    pathCursor = pathCursor + item.toString();
                    if (V(options["value"], pathCursor) === undefined || V(options["value"], pathCursor) === null) {
                        var phrase = 'options["value"]' + pathCursor + '={};';
                        eval(phrase);
                    }
                });
            }
            catch (err) {
                console.error("Error in ensureDataPath()");
                console.error(err);
            }
        }

        function V(o, p) {
            try {
                return eval("o" + p);
            } catch (e) {
                return null;
            }
        }

        function htmlDecode(str) {
            if (!str || str.length < 4) return str;
            systemLibraryJsonEditorGlobalTextArea.innerHTML = str;
            return systemLibraryJsonEditorGlobalTextArea.value.toString().replaceAll('"', "'");
        }

        function jsonEscape(str) {
            return str.replace(/\n/g, "\\n").replace(/\r/g, "\\r").replace(/\t/g, "\\t");
        }

        function getEC(ECType) {
            if (options["treeExpandCollapseButton"] != "true") return "";
            if (ECType == "e") return '<div style="display:inline-block; width: 22px;" class="j-ec">&#8595;</div>&nbsp;';
            if (ECType == "c") return '<div style="display:inline-block; width: 22px;" class="j-ec">&#8593;</div>&nbsp;';
            return '';
        }

        function toggleSubTree(ele) {
            var state = $(ele).text().trim();

            if (state.startsWith('↑')) {
                collapseByEcRowClick(ele);
                if (ele.classList.contains('expanded')) {
                    ele.classList.remove('expanded');
                }
                if (!ele.classList.contains('collapsed')) {
                    ele.classList.add('collapsed');
                }
            }
            else if (state.startsWith('↓')) {
                expandByEcRowClick(ele);
                if (ele.classList.contains('collapsed')) {
                    ele.classList.remove('collapsed');
                }
                if (!ele.classList.contains('expanded')) {
                    ele.classList.add('expanded');
                }
            }
            else {
                if (state == '↑') { collapseByECButton(ele); } else if (state == '↓') { expandByECButton(ele); }
            }
        }

        function expandByECButton(btn) {
            $(btn).text("↑");
            $(btn).parents("tr:first").next("tr").removeClass("j-collapsed");
        }

        function collapseByECButton(btn) {
            $(btn).text("↓");
            $(btn).parents("tr:first").next("tr").addClass("j-collapsed");
        }

        function collapseByEcRowClick(ele) {
            let btn = $(ele).find('.j-ec');
            collapseByECButton(btn);
        }
        function expandByEcRowClick(ele) {
            let btn = $(ele).find('.j-ec');
            expandByECButton(btn);
        }

        function getSpacer(number) {
            var spaces = "";
            var n = number + (options["renderFirstLevel"] == "false" ? -1 : 0);
            for (i = 1; i < n * options["identing"]; i++) {
                spaces += "&nbsp;";
            }
            return '<span class="j-spacer">' + spaces + '</span>';
        }

        function fixNU(o, v) {
            return o || v;
        }

        function replaceAll(source, find, replace) {
            var str = source;
            return str.replace(new RegExp(find.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'g'), replace);
        }

        initOptions();

        initWidget();

        var textareaoptions = {
            buttons: ['bold', 'italic', 'link', 'mark']
        };

        $('.j-input-rich-textarea').easyEditor({
            options: textareaoptions
        });

        var output = {
            "isValid": function () { return isValid(); },
            "getSchema": function () { return getSchema(); },
            "getValue": function () { return getValue(); },
            "setValue": function (v) { return setValue(v); }
        };

        return output;
    };
}(jQuery));
