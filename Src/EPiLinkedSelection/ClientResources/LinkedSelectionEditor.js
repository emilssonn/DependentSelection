define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojox/mvc/equals",
    "dojo/aspect",
    "dojo/topic",

    "epi/dependency",
    "epi-cms/contentediting/editors/SelectionEditor",
    "epi/shell/store/JsonRest",
    "epi/shell/_ContextMixin"
],
function (
    declare,
    lang,
    equals,
    aspect,
    topic,

    dependency,
    SelectionEditor,
    JsonRest,
    _ContextMixin
) {
    return declare([SelectionEditor, _ContextMixin], {
        onItemChangedHandle: null,
        restStore: null,

        postMixInProperties: function () {
            this.restStore = new JsonRest({
                target: this.storeUrl
            });

            var registry = dependency.resolve("epi.storeregistry");
            var contentDataStore = registry.get("epi.cms.contentdata");
            this.onItemChangedHandle = aspect.after(contentDataStore, "onItemChanged", lang.hitch(this, this._update));

            this.inherited(arguments);
        },
        destroy: function () {
            if (this.onItemChangedHandle !== null) {
                this.onItemChangedHandle.remove();
            }
            this.inherited(arguments);
        },
        _update: function (a, data, sender) {
            for (var prop in this.dependsOn) {

                if (this.dependsOn.hasOwnProperty(prop) === true && data[1].properties.hasOwnProperty(prop) === true && equals(data[1].properties[prop], this.dependsOn[prop]) !== true) {
                    console.log("dependsOn", this.dependsOn[prop], "properties", data[1].properties[prop])
                    console.log("changed")
                    this.set("readOnly", true);
                    this.dependsOn[prop] = data[1].properties[prop];
                    this.restStore.query({ complexReference: this._currentContext.id }).then(lang.hitch(this, function (items) {
                        this._updateEditor(items);
                    }));
                    break;
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