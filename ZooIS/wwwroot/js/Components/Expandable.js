(function() {
    let isExpanded = new State(false);
    let root = HTMLElement;
    let expand_block = HTMLElement;

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
    }
	
	function expand_fn(state) {
		if (state)
            root.appendChild(expand_block);
        else
            expand_block.remove();
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
        script.remove();
    }

    component_did_mount();
}).call()