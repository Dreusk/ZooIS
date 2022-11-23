(function() {
    let isExpanded = new State(false);
    let items = new State(new Array());
    let root = HTMLDivElement;
    let expand_block = HTMLDivElement;
    let link = HTMLAnchorElement;

    function lookup() {
        let xhr = new XMLHttpRequest();
        xhr.open("GET", "Alerts/by_current_user");
        xhr.responseType = "json";
        xhr.onload = (ev) => xhr_load(ev, lookup_success);
        xhr.onerror = xhr_error;
        xhr.send();
    }

    function lookup_success(resp) {
        items.Set(resp["$values"]);
    }

    function click_fn() {
        isExpanded.Swap(reverse);
        
    }

    function key_fn(ev) {
        if (ev.keyCode == 13)
            isExpanded.Swap(reverse)
    }

    function window_fn(ev) {
        if (!root.contains(ev.target))
            isExpanded.Set(false);
    }

    function window_key_fn(ev) {
        if (ev.keyCode == 13 &&
            !root.contains(ev.target))
            isExpanded.Set(false)
        if (ev.keyCode == 27)
            isExpanded.Set(false);
    }
	
	function expand_fn(state) {
		if (state) {
            root.appendChild(expand_block);
            lookup();
        }
        else
            expand_block.remove();
	}

    function mark_as_read(item) {
        let xhr = new XMLHttpRequest();
        xhr.open("PATCH", `/Alerts/${item["guid"]}/mark_as_read`);
        xhr.send();
    }

    function render_body(item) {
        let body = document.createElement("div");
        body.innerHTML = `
        <h4>Подтверждение</h4>
        <div>Вы действительно хотите отметить это оповещение прочитанным?</div>
        <div class="mt-2 w-100 d-flex justify-content-between">
            <button type="button" class="cancel">Отменить</button>
            <button type="button" class="accept">Подтердить</button>
        </div>
        `
        let accept = body.querySelector(".accept");
        accept.onclick = () => mark_as_read(item);
        accept.onkeydown = (ev) => {if (ev.keyCode == 13) mark_as_read(item)}
        return body;
    }

    function item_onclick(item) {
        let body = document.createElement("div");
        new Modal(render_body(item));
        isExpanded.Set(false);
    }

    function item_onkeydown(ev, item) {
        if (ev.keyCode != 13)
            return;
        new Modal(render_body(item));
        isExpanded.Set(false);
    }

    function render_item(item) {
        let li = document.createElement("li");
        li.tabIndex = 0;
        li.onclick = function() {item_onclick(item)};
        li.onkeydown = function(ev) {item_onkeydown(ev, item)};
        li.innerHTML = `
        <div class=${item["level"]["value"].toLowerCase()}>
            <div class=${item["type"]["value"].toLowerCase()}>[${item["type"]["display"]}]</div>
            <div>${item["display"]}</div>
        </div>`;
        return li;
    }

    function render(items) {
        let root = expand_block.querySelector("ul");
        root.textContent = "";
        if (items.length > 0)
            items.forEach(item => {
                root.appendChild(render_item(item));
            });
        else {
            let el = document.createElement("li");
            el.textContent = "Непрочитанные оповещения отсутствуют";
            root.appendChild(el);
        }
        root.appendChild(link);
    }

    function component_did_mount() {
        let script = document.currentScript;
        root = GetRoot(script);
        expand_block = root.querySelector(".expand-block");
        link = expand_block.querySelector("li");
        root.querySelector(".trigger").addEventListener("click", click_fn);
        root.querySelector(".trigger").addEventListener("keydown", key_fn);
        window.addEventListener("click", window_fn)
        window.addEventListener("keydown", window_key_fn)
        isExpanded.Subscribe(expand_fn);
        items.Subscribe(render);
        expand_block.querySelector("ul").textContext = "";
        script.remove();
    }

    component_did_mount();
}).call()