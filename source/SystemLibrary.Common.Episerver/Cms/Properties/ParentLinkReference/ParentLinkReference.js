﻿try {

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
            return declare("systemLibrary.Common.Episerver.ParentLinkReference", [
                _Widget,
                _TemplatedMixin], {
                templateString: template,
                postCreate: function () {
                    var current = this;
                    let parentLinkReferenceId = this.parentLinkReferenceId;

                    setTimeout(function () {
                        try {
                            var parent = current.parent;
                            if (parent && parent !== null) {
                                var contentModel = parent.contentModel;
                                var id = contentModel['epi-icontent_parentlink'];
                                if (parentLinkReferenceId && parentLinkReferenceId > 4) {
                                    id = parentLinkReferenceId;
                                }
                                var path = "/EPiServer/CMS/?language=no#context=epi.cms.contentdata:///";

                                var fullUrl = path + id;
                                var urlLink = '<a target="_blank" style="font-size:12px;position: relative;top: 2px;text-decoration: underline;color: #0037ff;" href="' + fullUrl + '">Parent: ' + id + '</a>';

                                current.domNode.innerHTML = '<div>' + urlLink + '</div>';

                                var li = current.domNode.parentElement;

                                if (this.allPropertiesShowPropertiesAsColumns) {
                                    if (li) {
                                        li.style.display = 'flex';
                                    }
                                }

                            } else {
                                console.warn("Current viewed object has no parent found");
                            }
                        }
                        catch (e) {
                            console.error("Error getting parent link");
                            console.error(e);
                        }
                    }, 200);
                }
            }
            );
        });
}
catch {
    console.error("ParentLinkReference.js crashed on load");
}