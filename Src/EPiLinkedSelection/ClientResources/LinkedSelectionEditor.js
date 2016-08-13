define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojox/mvc/equals",
    "dojo/aspect",
    "dojo/topic",

    "epi/dependency",
    "epi-cms/contentediting/editors/SelectionEditor",
    "epi/shell/store/JsonRest"
],
function (
    declare,
    lang,
    equals,
    aspect,
    topic,

    dependency,
    SelectionEditor,
    JsonRest
) {
    return declare([SelectionEditor], {
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

        _update: function (x, data) {
            for (var prop in this.linkedTo) {
                if (this.linkedTo.hasOwnProperty(prop) === true && data[1].properties.hasOwnProperty(prop) === true && equals(data[1].properties[prop], this.linkedTo[prop]) !== true) {
                    this.set("readOnly", true);
                    this.linkedTo[prop] = data[1].properties[prop];
                    this.restStore.query({ complexReference: data[0] }).then(lang.hitch(this, function (items) {
                        this._updateEditor(items);
                    }));
                    break;
                }
            }
        },

        _updateEditor: function (items) {
            if (this.readOnlyOnEmpty !== true || (this.readOnlyOnEmpty === true && items.length > 0)) {
                this.set("readOnly", false);
            }
            this.set("selections", items);
            this.set("value", null);
            this._setSelectionsAttr(items);
            this._setValueAttr(null, undefined);
        }
    });
});