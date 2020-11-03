import Vue from "./vue.esm.browser.min.js";
import Search from "./search.js";
import Dialog from "./dialog.js";
Vue.use(Dialog);
new Vue({
    components: {
        Search
    },
    data: {
        version: ""
    },
    methods: {
        Reset() {
            //cheating here a bit, instead of using a $ref
            this.$children[0].packageName = null;
            this.$children[0].searchResult = null;
            document.getElementById("txt-search").value = "";
        },
        RefreshDB() {
            fetch("/indexdb", { method: "PUT" })
                .then(resp => { return resp.text() })
                .then(d => {
                    this.$dialog({
                        title: "Message",
                        message: d,
                        type: "ok"
                    }).then(() => {
                        return;
                    }).catch(() => {
                        return;
                    });
                });
        }
    },
    created() {
        let vm = this;
        fetch("/aboutserver", {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        }).then(resp => {
            return resp.json();
        }).then(d => {
            vm.version = d.version;
        });
    }
}).$mount("#app");