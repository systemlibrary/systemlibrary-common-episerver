try {

    define([
        "dojo/_base/declare",
        "dijit/_Widget",
        "dijit/_TemplatedMixin",
        'dojo/text!./Html'
    ],
        function (
            declare,
            _Widget,
            _TemplatedMixin,
            template
        ) {
            return declare("systemLibrary.Common.Episerver.Message", [
                _Widget,
                _TemplatedMixin], {
                templateString: template,

                isToggleable: false,

                postMixInProperties: function () {
                    this.tooltip = '';
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
                        var cssPrefixId = 'systemLibraryCommonEpiserverMessageStyle';
                        if (!document.getElementById(cssPrefixId)) {
                            var head = document.getElementsByTagName('body')[0];

                            var css = this._getStylesheetLink(cssPrefixId, '/SystemLibrary/Common/Episerver/Cms/Message/Style');

                            head.appendChild(css);
                        }
                    }
                    catch (e) {
                        console.error(e);
                    }
                },

                onToggleDescription: function (tooltipNode, hide) {
                    try {
                        if (tooltipNode && tooltipNode.firstChild) {
                            var textContainer = tooltipNode.firstChild; //parentElement

                            var aHtml = '<i class="fa fa-chevron-down" aria-hidden="true"></i>';
                            if (typeof (hide) !== 'undefined' && hide !== null) {
                                if (hide === true) {
                                    textContainer.classList.add('systemLibraryCommonEpiserverMessageCollapsed');
                                } else {
                                    textContainer.clazssList.remove('systemLibraryCommonEpiserverMessageCollapsed');
                                }
                            } else {
                                textContainer.classList.toggle('systemLibraryCommonEpiserverMessageCollapsed');
                                if (!textContainer.classList.contains('systemLibraryCommonEpiserverMessageCollapsed')) {
                                    aHtml = '<i class="fa fa-chevron-up" aria-hidden="true"></i>';
                                }
                            }

                            var aCollection = tooltipNode.getElementsByTagName('a');
                            if (aCollection && aCollection.length && aCollection.length > 0) {
                                var a = aCollection[0];
                                if (a.innerHTML.toString().includes('<i class')) {
                                    a.innerHTML = aHtml;
                                }
                            }
                        }
                    }
                    catch {
                        console.warn("Error occured toggling the description, please refresh website");
                    }
                },
                postCreate: function () {


                    function is(data) {
                        if (typeof (data) === 'undefined' || data === null || data === "" || (data.length && data.length === 0)) {
                            return false;
                        }
                        return true;
                    }

                    function addToggleButton(self) {
                        var a = document.createElement('a');

                        a.href = "#";
                        a.className = 'chevron-toggle';
                        a.innerHTML = '<i class="fa fa-chevron-down" aria-hidden="true"></i>';
                        a.style = "cursor:pointer; position: absolute;top:0;right:0;margin-right:3px;margin-top:0px;width:22px;height:22px;text-align: center;font-size:17px;";

                        a.onclick = function () {
                            self.onToggleDescription(self.domNode);
                        };

                        self.domNode.appendChild(a);
                    }

                    function setDescription(self, tooltip) {
                        self.domNode.innerHTML = '<div>' + tooltip + '</div>';
                    }

                    try {

                        this._loadCssFile();

                        if (is(this.metadata) && is(this.metadata.settings) && is(this.metadata.settings.tooltip)) {
                            setDescription(this, this.metadata.settings.tooltip);

                            var tooltip = this.metadata.settings.tooltip;
                            var countLines = (tooltip.match(/<br>/g) || []).length;
                            countLines += (tooltip.match(/<br\/>/g) || []).length;
                            countLines += (tooltip.match(/<br \/>/g) || []).length;
                            countLines += (tooltip.match(/<\/p>/g) || []).length;

                            if (tooltip.length > 255 || countLines > 2) {
                                this.isToggleable = true;

                                addToggleButton(this);
                            }
                        }
                    }
                    catch (e) {
                        console.error(e);
                    }
                },
                startup: function () {
                    function is(data) {
                        if (typeof (data) === 'undefined' || data === null || data === "" || (data.length && data.length === 0)) {
                            return false;
                        }
                        return true;
                    }

                    function addInfoIcon(label, self, canToggle) {
                        var a = document.createElement('a');

                        a.href = "#";
                        a.innerHTML = '<i style="font-size: 16px;" class="fa fa-info-circle" aria-hidden="true"></i>';
                        if (canToggle === true) {
                            a.onclick = function () {
                                self.onToggleDescription(self.domNode);
                            };
                            a.style = 'cursor: pointer; position: absolute; margin-left: 8px;margin-top: -1px;';
                        } else {
                            a.style = 'cursor: default; position: absolute; margin-left: 8px;margin-top: -1px;';
                        }

                        label.appendChild(a);
                    }

                    try {
                        if (is(this.domNode.previousElementSibling)) {
                            var label = this.domNode.previousElementSibling;
                            label.innerHTML = '<span style="color: TextColor;">' + label.innerHTML + '</span>';

                            if (this.isToggleable) {
                                this.onToggleDescription(this.domNode, true, label);
                            }
                            addInfoIcon(label, this, this.isToggleable);
                        }
                    }
                    catch (e) {
                        console.error(e);
                    }
                }
            }
            );
        });
}
catch {
    console.error("Message.js crashed on load");
}