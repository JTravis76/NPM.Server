import Vue from "./vue.esm.browser.min.js";
import Search from "./search.js";

new Vue({
    components: {
        Search
    },
    data: {
        version: ""
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