import Vue from "./vue.esm.browser.min.js";
import PackageDetail from "./details.js";

export default Vue.extend({
    name: "search-component",
    components: {
        PackageDetail
    },
    template: `
<div>
    <input id="txt-search" class="form-control" value="" placeholder="Search..." v-on:keyup.enter="OnSearch" />
    <p></p>
    <div class="list-group list-group-flush">
        <template v-for="d in searchResult">
            <button type="button" class="list-group-item list-group-item-action" @click="Goto(d.name)" :title="d.description">{{d.name}} {{d.version}}</button>
        </template>
    </div>
    <package-detail :name="packageName"></package-detail>
</div>`,
    data() {
        return {
            searchResult: [],
            packageName: null
        }
    },
    methods: {
        OnSearch(e) {
            if (e.keyCode === 13) {
                let vm = this;
                let txt = document.getElementById("txt-search");

                ShowSpinner();
                fetch("/search", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(txt.value)
                }).then(reponse => {
                    return reponse.json();
                }).then(data => {
                    vm.searchResult = data;
                    vm.packageName = null;
                    ShowSpinner(false);
                });
            }
        },
        Goto(name) {
            let txt = document.getElementById("txt-search");
            txt.value = "";
            this.searchResult = [];
            this.packageName = name;
        }
    }
});