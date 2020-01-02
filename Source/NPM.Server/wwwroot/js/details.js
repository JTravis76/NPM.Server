import Vue from "./vue.esm.browser.min.js";

export default Vue.extend({
    name: "package-detail",
    template: `
<div class="row" v-show="name != null">
    <div class="col-md-8">
        <div class="markdown" v-html="readme"></div>
    </div>
    <div class="col-md-4">
        <div class="form-group">
            <label class="font-weight-bold">Install</label>
            <div class="command">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12.32 9.33" style="height:16px;"><g><line class="st1" x1="7.6" y1="8.9" x2="7.6" y2="6.9" /><rect width="1.9" height="1.9" /><rect x="1.9" y="1.9" width="1.9" height="1.9" /><rect x="3.7" y="3.7" width="1.9" height="1.9" /><rect x="1.9" y="5.6" width="1.9" height="1.9" /><rect y="7.5" width="1.9" height="1.9" /></g></svg> 
                npm i {{name}}
            </div>
        </div>        
        <div class="form-group">
            <label class="font-weight-bold">Description</label>
            <div>
                {{description}}
            </div>
        </div>
    </div>
</div>`,
    data() {
        return {
            readme: null,
            description: null
        }
    },
    props: ['name'],
    watch: {
        name(n, o) {
            if (n !== null) {
                let url = "/" + n;
                ShowSpinner();

                fetch(url, {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json"
                    }
                }).then(reponse => {
                    return reponse.json();
                }).then(data => {
                    this.readme = marked(data.readme);
                    this.description = data.description;
                    ShowSpinner(false);
                });
            }
        }
    },
    updated() {
        let els = document.getElementsByTagName("table");
        if (els.length > 0) {
            for (var i = 0; i < els.length; i++) {
                els[i].classList = "table table-striped";
                els[i].children[0].classList = "thead-dark";
            }
        }
        els = document.getElementsByTagName("pre");
        if (els.length > 0) {
            for (var j = 0; j < els.length; j++) {
                els[j].style.backgroundColor = "#f1f1f1";
                els[j].style.padding = "4px";
            }
        }
    }
});