(function() {
    let isExpanded = new State(false);
	let items = new State(new Array());
    let root = HTMLElement;
    let expand_block = HTMLElement;

	/**
	 * Looks up for roles of current user.
	 */
	function lookup() {
		xhr = new XMLHttpRequest();
		xhr.open("GET", "/Roles/by_current_user")
        xhr.responseType = "json";
        xhr.onload = (ev) => xhr_load(ev, lookup_success);
        xhr.onerror = xhr_error;
		xhr.send();
        expand_block.querySelector("ul").textContent = "";
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

    function render_items(items) {
        let root = expand_block.querySelector("ul");
        root.textContent = "";
        if (items.length > 0)
            items.forEach(role => {
                let el = document.createElement("li");
                el.textContent = role["display"];
                root.appendChild(el);
            })
        else {
            let el = document.createElement("li");
            el.textContent = "Роли отсутствуют";
            root.appendChild(el)
        }
    }

    function component_did_mount() {
        let script = document.currentScript;
        root = GetRoot(script);
        expand_block = root.querySelector(".expand-block");
        root.querySelector(".trigger").addEventListener("click", click_fn);
        root.querySelector(".trigger").addEventListener("keydown", key_fn);
        window.addEventListener("click", window_fn)
        window.addEventListener("keydown", window_key_fn)
        isExpanded.Subscribe(expand_fn);
        items.Subscribe(render_items);
        script.remove();
    }

    component_did_mount();
}).call()