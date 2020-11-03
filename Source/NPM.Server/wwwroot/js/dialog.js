export default {
    install: function (vue, name) {
        if (name === void 0) { name = "$dialog"; }
        function Show(dlg, opt) {
            if (dlg === null || dlg === undefined)
                throw new Error("Dialog is not defined.");
            if (!dlg["showModal"]) {
                console.warn("[WARN] your browser doesn't support native dialog api. Using fallback api.");
                dlg.remove = function () {
                    var body = document.getElementsByTagName("body")[0];
                    body.removeChild(dlg);
                };
                dlg.show = function () {
                    dlg.setAttribute("open", "");
                    dlg.style.width = "500px";
                    dlg.style.display = "block";
                    dlg.style.position = "absolute";
                    dlg.style.margin = "auto";
                    dlg.style.background = "white";
                    dlg.style.color = "black";
                    dlg.style.border = "solid";
                    dlg.style.borderWidth = "2px";
                    dlg.style.borderColor = "black";
                    dlg.style.padding = "1em";
                    dlg.style.top = "50%";
                    dlg.style.left = "50%";
                    dlg.style.transform = "translate(-50%, -50%)";
                };
                dlg.showModal = function () {
                    dlg.show();
                    var body = document.getElementsByTagName("body")[0];
                    var div = document.createElement("div");
                    div.classList.add("dialog-backdrop");
                    div.style.position = "fixed";
                    div.style.top = "0px";
                    div.style.left = "0px";
                    div.style.right = "0px";
                    div.style.bottom = "0px";
                    div.style.background = "rgba(0,0,0,0.2)";
                    div.style.zIndex = "50";
                    body.appendChild(div);
                };
                dlg.close = function (v) {
                    dlg.removeAttribute("open");
                    dlg.style.display = "none";
                    var body = document.getElementsByTagName("body")[0];
                    var div = document.querySelector(".dialog-backdrop");
                    body.removeChild(div);
                    dlg.returnValue = v.toString();
                };
                dlg.open = dlg.hasAttribute("open");
            }
            if (opt.backdrop)
                dlg.showModal();
            else
                dlg.show();
        }
        var dialog = function (options) {
            var defaultOpt = {
                message: "",
                backdrop: true,
                title: "Message",
                type: "ok"
            };
            if (typeof options === "string") {
                defaultOpt.message = options;
            }
            else {
                defaultOpt = Object.assign(defaultOpt, options);
            }
            return new Promise(function (resolve, reject) {
                return new (vue.extend({
                    template: "\n<dialog style=\"min-width:400px;z-index:2000;\">\n    <div style=\"font-weight: bold;font-size:large;border-bottom:1px solid lightgray;margin-bottom:8px;\">{{title}}</div>\n    <div class=\"alert\" v-html=\"message\"></div>\n    <div class=\"pull-right\">\n        <template v-if=\"type === 'ok'\">\n            <button type=\"button\" class=\"btn btn-default\" @click=\"Response(true)\" data-testid=\"btn-dialog-ok\"><span class=\"fa fa-ok\"></span>Ok</button>\n        </template>\n        <template v-if=\"type === 'yesno'\">\n            <button type=\"button\" class=\"btn btn-success\" @click=\"Response(true)\" data-testid=\"btn-dialog-yes\"><span class=\"fa fa-thumbs-up\"></span>Yes</button>\n            <button type=\"button\" class=\"btn btn-danger\" @click=\"Response(false)\" data-testid=\"btn-dialog-no\"><span class=\"fa fa-thumbs-down\"></span>No</button>\n        </template>\n    </div>\n</dialog>",
                    data: function () {
                        return {
                            dlg: null,
                            message: defaultOpt.message,
                            title: defaultOpt.title,
                            type: defaultOpt.type
                        };
                    },
                    methods: {
                        Response: function (result) {
                            var dlg = this.dlg;
                            dlg.close("");
                            dlg.remove();
                            if (result)
                                resolve(dlg.returnValue);
                            else
                                reject();
                        }
                    },
                    beforeMount: function () {
                        this.dlg = document.querySelector("dialog");
                        if (this.dlg) {
                            Show(this.dlg, defaultOpt);
                            return;
                        }
                        var container = document.body;
                        container.appendChild(this.$el);
                    },
                    mounted: function () {
                        var _this = this;
                        this.$nextTick(function () {
                            _this.dlg = document.querySelector("dialog");
                            Show(_this.dlg, defaultOpt);
                        });
                    }
                }))({
                    el: document.createElement("dialog")
                });
            });
        };
        Object.defineProperty(vue.prototype, name, { value: dialog });
    }
};