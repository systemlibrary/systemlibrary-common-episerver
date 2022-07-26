define([
    "dojo/_base/declare",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
],
    function (
        declare,
        _Widget,
        _TemplatedMixin
    ) {
        return declare("systemLibrary.Common.Episerver.Message", [
            _Widget,
            _TemplatedMixin], {
            templateString: "<div class='dijitInline' style='position:relative;font-size:14px;line-height:1.36;color:#414141;border:1px solid #b5bcc7;padding:1em;min-width:585px;max-width: 1024px;background-color:MessagePropertyBackgroundColor'></div>",

            isToggleable: false,

            postMixInProperties: function () {
                this.tooltip = '';
            },

            onToggleDescription: function (tooltipNode, hide) {
                try {
                    if (tooltipNode && tooltipNode.firstChild) {
                        var textContainer = tooltipNode.firstChild;
                        var aHtml = '<i class="fa fa-arrow-down" aria-hidden="true"></i>';
                        if (typeof (hide) !== 'undefined' && hide !== null) {
                            if (hide === true) {
                                textContainer.classList.add('dijitHidden');
                            }
                        } else {
                            textContainer.classList.toggle('dijitHidden');
                            if (!textContainer.classList.contains('dijitHidden')) {
                                aHtml = '<i class="fa fa-arrow-up" aria-hidden="true"></i>';
                            }
                        }

                        var aCollection = tooltipNode.getElementsByTagName('a');
                        if (aCollection && aCollection.length && aCollection.length > 0) {
                            var a = aCollection[0];
                            a.innerHTML = aHtml;
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
                    a.innerHTML = '<i class="fa fa-arrow-down" aria-hidden="true"></i>';
                    a.style = "cursor:pointer; position: absolute;top:0;right:0;margin-right:4px;margin-top:4px;width:24px;height:24px;text-align: center;font-size:17px;";

                    a.onclick = function () {
                        self.onToggleDescription(self.domNode);
                    };

                    self.domNode.appendChild(a);
                }

                function setDescription(self, tooltip) {
                    self.domNode.innerHTML = '<div>' + tooltip + '</div>';
                }

                try {
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

                try {
                    if (is(this.domNode.previousElementSibling)) {
                        var label = this.domNode.previousElementSibling;
                        label.innerHTML = '<i style="padding-right: 2px;" class="fa fa-info-circle" aria-hidden="true"></i><span style="color: MessagePropertyBackgroundColorDarkened;">' + label.innerHTML + "</span>";
                        label.style = 'border-bottom:1px solid #b5bcc7;';

                        if (this.isToggleable) {
                            this.onToggleDescription(this.domNode, true);
                        }
                    }
                }
                catch (e) {
                    console.error(e);
                }
            }
        }
        );
    });
