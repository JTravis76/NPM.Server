import Vue from "./vue.esm.browser.min.js";
import PackageDetail from "./details.js";

export default Vue.extend({
    name: "search-component",
    components: {
        PackageDetail
    },
    template: `
<div>
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <button class="btn btn-outline-success" type="button" id="btn-search" @click="OnSearch">
            <svg width="1em" height="1em" viewBox="0 0 16 16" class="bi bi-search" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
              <path fill-rule="evenodd" d="M10.442 10.442a1 1 0 0 1 1.415 0l3.85 3.85a1 1 0 0 1-1.414 1.415l-3.85-3.85a1 1 0 0 1 0-1.415z"/>
              <path fill-rule="evenodd" d="M6.5 12a5.5 5.5 0 1 0 0-11 5.5 5.5 0 0 0 0 11zM13 6.5a6.5 6.5 0 1 1-13 0 6.5 6.5 0 0 1 13 0z"/>
            </svg>
        </button>
      </div>
        <input id="txt-search" class="form-control" value="" placeholder="Search..." v-on:keyup.enter="OnSearch" aria-label="" aria-describedby="btn-search" />
    </div>
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
            if (e.type === "click" || (e.type === "keyup" && e.keyCode === 13)) {
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