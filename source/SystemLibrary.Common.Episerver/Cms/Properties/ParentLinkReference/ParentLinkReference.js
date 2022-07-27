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

                setTimeout(function () {
                    try {
                        var parent = current.parent;
                        if (parent && parent !== null) {
                            var contentModel = parent.contentModel;
                            var id = contentModel['epi-icontent_parentlink'];
                            var path = "/EPiServer/CMS/?language=no#context=epi.cms.contentdata:///";

                            var fullUrl = path + id;
                            var urlLink = '<a target="_blank" style="top: 2px; text-decoration: underline; color: #212121;" href="' + fullUrl + '">Content Id: ' + id + '</a>';

                            current.domNode.innerHTML = '<div style="font-size: 14px;">' + urlLink + '</div>';

                            var li = current.domNode.parentElement;
                            if (li) {
                                li.style.display = 'flex';
                            }
                        } else {
                            console.log("Current viewed object has no parent found");
                        }
                    }
                    catch (e) {
                        console.log("Error getting parent link");
                        console.log(e);
                    }
                }, 200);
            }
        }
        );
    });
