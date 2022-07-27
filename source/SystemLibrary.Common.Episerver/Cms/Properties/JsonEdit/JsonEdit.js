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
        return declare("systemLibrary.Common.Episerver.JsonEdit", [
            _Widget,
            _TemplatedMixin], {
            templateString: "<div class='dijitInline' style='position:relative;font-size:14px;line-height:1.36;color:#414141;border:1px solid #b5bcc7;padding:1em;min-width:585px;max-width: 1024px;background-color:MessagePropertyBackgroundColor'></div>",

            postCreate: function () {
                function is(data) {
                    if (typeof (data) === 'undefined' || data === null || data === "" || (data.length && data.length === 0)) {
                        return false;
                    }
                    return true;
                }

                try {
                    if (is(this.metadata) && is(this.metadata.settings) && is(this.metadata.settings.tooltip)) {
                    }
                }
                catch (e) {
                    console.error(e);
                }
            },
        }
        );
    });
