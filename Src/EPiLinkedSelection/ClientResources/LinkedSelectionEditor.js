define([
    "dojo/_base/declare",
    "dojo/_base/connect",

    "epi-cms/contentediting/editors/SelectionEditor",
    "epi/shell/store/JsonRest",
    "epi/shell/_ContextMixin"
],
function (
    declare,
    connect,

    SelectionEditor,
    JsonRest,
    _ContextMixin
) {
    return declare([SelectionEditor, _ContextMixin], {
        eventHandle: null,
        restStore: null,

        postMixInProperties: function () {
            if (this.dependsOn) {
                this.eventHandle = connect.subscribe("widgetBlur", this, this._update);
            }

            this.restStore = new JsonRest({
                target: this.storeUrl
            });

            this.inherited(arguments);
        },
        _update: function (data, sender) {
            // Empty string?
            if (!data.value && this.readOnlyWhen.indexOf(data.name) > -1) {
                this.set("readOnly", true)
                this.set("value", null);

                this._setDisplay(null);
                this._updateSelection();
                this._handleOnChange(null, undefined);
                return;
            }

            if (this.dependsOn.indexOf(data.name) > -1) {
                var that = this;

                var currentContextID = this._currentContext.id.split("_");
                var contentReference = {
                    contentID: currentContextID[0],
                    versionID: currentContextID[1]
                }

                this.restStore.query(contentReference).then(function (items) {
                    that.set("readOnly", false)
                    that.set("value", null);

                    that.set("selections", items);
                    that._setSelectionsAttr(items);

                    that._setDisplay(null);
                    that._updateSelection();
                    that._handleOnChange(null, undefined);
                });
            }
        },
        destroy: function () {
            if (this.eventHandle) {
                connect.unsubscribe(this.eventHandle);
            }
            this.inherited(arguments);
        }
    });
});