define([
    "dojo/_base/declare",
    "dojo/_base/connect",
    "dojo/request",

    "epi-cms/contentediting/editors/SelectionEditor"
],
function (
    declare,
    connect,
    request,

    SelectionEditor
) {
    return declare([SelectionEditor], {
        constructor: function (params) {
            this.inherited(arguments, [params]);
        },
        _handleOnChange: function (value, priorityChange) {
            this.inherited(arguments, [value, priorityChange]);
        },
        destroy: function () {
            this.inherited(arguments);
        },
        _update: function (value) {

        }
    });
});