(function() {
    //Params
    let Url = "";
    //Flags
    let isDisabled = new State(false);

    //Local
    let root = HTMLElement;

    function disable_button(isDisabled) {
        if (isDisabled)
            root.setAttribute("disabled", "true");
        else
            root.removeAttribute("disabled");
    }

    function get() {
        isDisabled.Set(true);
        let xhr = new XMLHttpRequest();
        xhr.open("GET", Url);
        xhr.responseType = "blob";
        xhr.onload = (ev) => xhr_load(ev, get_success);
		xhr.onerror = xhr_error;
        xhr.send();
    }

    function get_success(resp, xhr) {
        isDisabled.Set(false);
        let name = xhr.getResponseHeader("content-disposition");
        name = name.match(/filename\*=.*/)[0];
        name = name.match(/(?<='').*/)[0];
        name = decodeURIComponent(name);
        
        let link = document.createElement("a");
        link.href = URL.createObjectURL(resp);
        link.download = "Отчет";
        link.click();
        link.remove();
    }

    function component_did_mount() {
        let script = document.currentScript;
        root = GetRoot(script);
        let params = JSON.parse(script.textContent);
        Url = params["url"];
        isDisabled.Set(params["flags"]["isDisabled"]);
        isDisabled.Subscribe(disable_button);

        (function(el) {
            el.onclick = get;
        }).call(this, root)

        script.remove();
    }

    component_did_mount()
}).call(this);