(function() {
    let isActive = new State(false);
    let root = HTMLElement;
    let input = HTMLInputElement;
    
    function activate_fn(isActive) {
        if (isActive)
            root.querySelector(".search").classList.add("active");
        else
            root.querySelector(".search").classList.remove("active");
    }
    
    function clear_fn() {
        input.value = null;
        let ev = new Event("change");
        input.dispatchEvent(ev);
    }
    
    function component_did_mount() {
        let script = document.currentScript;
        root = GetRoot(script);
        
        (function(el) {
            input = el;
            el.onfocus = () => isActive.Set(true);
            el.onblur = () => isActive.Set(false);
            el.onmouseover = () => isActive.Set(true);
            el.onmouseout = () => isActive.Set(false);
        }).call(this, root.querySelector(".search input"));
        
        (function(el) {
            el.onclick = () => clear_fn();
            el.onkeydown = (ev) => { if (ev.keyCode == 13) clear_fn() };
        }).call(this, root.querySelector(".search .clear"));
        
        isActive.Subscribe(activate_fn);
        script.remove();
    }
    
    component_did_mount();
    }).call();