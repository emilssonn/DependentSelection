define([
    // Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojox/mvc/equals",
    "dojo/aspect",
    "dojo/_base/array",
    "dojo/dom-construct",

    // EPi
    "epi/dependency",
    "epi-cms/contentediting/editors/CheckBoxListEditor",
    "epi/shell/store/JsonRest"
],
function (
    // Dojo
    declare,
    lang,
    equals,
    aspect,
    array,
    domConstruct,

    // EPi
    dependency,
    CheckBoxListEditor,
    JsonRest
) {
    return declare([CheckBoxListEditor], {
        _onItemChangedHandle: null,
        _restStore: null,

        postMixInProperties: function () {
            this._restStore = new JsonRest({
                target: this.storeUrl
            });

            var registry = dependency.resolve("epi.storeregistry");
            var contentDataStore = registry.get("epi.cms.contentdata");
            this._onItemChangedHandle = aspect.after(contentDataStore, "onItemChanged", lang.hitch(this, this._update));

            this.inherited(arguments);
        },

        destroy: function () {
            if (this._onItemChangedHandle !== null) {
                this._onItemChangedHandle.remove();
            }
            this.inherited(arguments);
        },

        _update: function (x, data) {
            for (var prop in this.linkedTo) {
                if (this.linkedTo.hasOwnProperty(prop) === true && data[1].properties.hasOwnProperty(prop) === true && equals(data[1].properties[prop], this.linkedTo[prop]) !== true) {
                    this.set("readOnly", true);
                    this.linkedTo[prop] = data[1].properties[prop];
                    this._restStore.query({ complexReference: data[0] }).then(lang.hitch(this, function (items) {
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

            var checkbox;
            while (checkbox = this._checkboxes.pop()) {
                checkbox.destroyRecursive();
            }

            var labelClickHandle;
            while (labelClickHandle = this._labelClickHandles.pop()) {
                labelClickHandle.remove();
            }

            domConstruct.empty(this.domNode);

            this.set("selections", items);
            this.set("value", null);
            this._setValueAttr(null);
            if (this.required) {
                this.set("state", "Error");
            }

            array.forEach(this.selections, this._addCheckBoxForItem, this);
        }
    });
});