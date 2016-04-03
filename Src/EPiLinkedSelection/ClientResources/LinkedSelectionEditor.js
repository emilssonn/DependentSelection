define([
    "dojo/_base/declare",
    "dojo/_base/connect",
    "dojo/_base/lang",
    "dojo/_base/json",
    "dojo/request/notify",
    "dojox/mvc/equals",

    "epi-cms/contentediting/editors/SelectionEditor",
    "epi/shell/store/JsonRest",
    "epi/shell/_ContextMixin"
],
function (
    declare,
    connect,
    lang,
    json,
    notify,
    equals,

    SelectionEditor,
    JsonRest,
    _ContextMixin
) {
    return declare([SelectionEditor, _ContextMixin], {
        eventHandle: null,
        notifyHandle: null,
        restStore: null,
        saved: null,
        postMixInProperties: function () {
            this.eventHandle = connect.subscribe("widgetBlur", this, this._update);

            this.restStore = new JsonRest({
                target: this.storeUrl
            });

            /**
             * If we should use ContentData on the server. If so we need to wait for the save to complete before doing request to get new selections.
             * The following code checks if a save has been made and the modified property was one we depend on. If so we update the last saved date.
             */
            if (this.useContentData === true && typeof this.contentDataStoreUrl === "string") {
                this.notifyHandle = notify("load", lang.hitch(this, function (data) {
                    var url = data.url.toLowerCase();
                    if (data.options.method === "POST" && url === this.contentDataStoreUrl) {
                        var dataObj = json.fromJson(data.data);

                        for (var prop in this.dependsOn) {
                            if (this.dependsOn.hasOwnProperty(prop) === true) {
                                var shouldUpdate = dataObj.properties.some(lang.hitch(this, function (obj) {
                                    return obj.successful === true && obj.name === prop;
                                }));
                                if (shouldUpdate === true) {
                                    this.saved = new Date();
                                    break;
                                }
                            }
                        }
                    }
                }));
            }

            this.inherited(arguments);
        },
        destroy: function () {
            if (this.eventHandle !== null) {
                connect.unsubscribe(this.eventHandle);
            }
            if (this.notifyHandle !== null) {
                this.notifyHandle.remove();
            }
            this.inherited(arguments);
        },
        _update: function (data, sender) {
            // If we should make this property read only and set the value to null.
            if (!data.value && this.readOnlyWhen.indexOf(data.name) > -1) {
                this.set("readOnly", true);
                this._setValueAttr(null, undefined);
            } else {
                for (var prop in this.dependsOn) {
                    // If a property we depend on has been changed.
                    if (this.dependsOn.hasOwnProperty(prop) === true && prop === data.name && equals(this.dependsOn[prop], data.value) !== true) {
                        this.dependsOn[prop] = data.value;
                        if (this.useContentData === true) {
                            // Make the property read only while we wait for the save to complete.
                            this.set("readOnly", true);

                            // Wait for the save to complete
                            var intervalId = window.setInterval(lang.hitch(this, function () {
                                if (this.saved !== null && this.saved < new Date()) {
                                    window.clearInterval(intervalId);
                                    this.saved = null;
                                    this.restStore.query({ complexReference: this._currentContext.id }).then(lang.hitch(this, function (items) {
                                        this._updateEditor(items);
                                    }));
                                }

                            }), 20);
                        } else {
                            this.restStore.add({ values: this.dependsOn }).then(lang.hitch(this, function (items) {
                                this._updateEditor(items);
                            }));
                        }
                        break;
                    }
                }
            }
        },
        _updateEditor: function (items) {
            this.set("readOnly", false);
            this.set("selections", items);
            this.set("value", null);
            this._setSelectionsAttr(items);
            this._setValueAttr(null, undefined);
        }
    });
});