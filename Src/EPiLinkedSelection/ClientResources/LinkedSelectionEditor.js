define([
    "dojo/_base/declare",
    "dojo/_base/connect",

    "epi-cms/contentediting/editors/SelectionEditor",
    "epi/shell/store/JsonRest"
],
function (
    declare,
    connect,

    SelectionEditor,
    JsonRest
) {
    return declare([SelectionEditor], {
        eventHandle: null,
        restStore: null,
        constructor: function (params) {
            this.inherited(arguments, [params]);
        },
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

            for (var prop in this.dependsOn) {
                if (this.dependsOn.hasOwnProperty(prop) === true && prop === data.name) {
                    this.dependsOn[prop] = data.value;
                    var that = this;
                    this.restStore.query(this.dependsOn).then(function (items) {
                        that.set("readOnly", false)
                        that.set("selections", items);
                        that.set("value", null);

                        that._setSelectionsAttr(items);
                        that._setDisplay(null);
                        that._updateSelection();
                        that._handleOnChange(null, undefined);
                    });
                    break;
                }
            }
        },
        destroy: function () {
            this.inherited(arguments);
            if (this.eventHandle) {
                connect.unsubscribe(this.eventHandle);
            }
        }
    });
});