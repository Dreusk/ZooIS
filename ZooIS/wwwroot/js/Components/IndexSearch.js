(function() {
let isActive = new State(false);
let isExpanded = new State(false);
let root = HTMLElement;
let expand_block = HTMLElement;

function window_click_fn(ev) {
    if (!root.contains(ev.target))
        isExpanded.Set(false);
}

function window_keydown_fn(ev) {
    if ((ev.keyCode == 13 &&
        !root.contains(ev.target)) ||
        ev.keyCode == 27)
            isExpanded.Set(false);
}

function activate_fn(isActive) {
    if (isActive)
        root.querySelector(".search").classList.add("active");
    else
        root.querySelector(".search").classList.remove("active");
}

function expand_fn(isExpanded) {
	if (isExpanded)
        expand_block.style["display"] = "block"
    else
        expand_block.style["display"] = "none"
}

function clear_fn() {
    window.location.search = "";
}

function component_did_mount() {
    let script = document.currentScript;
    root = GetRoot(script).querySelector(".search-wrap");
    window.addEventListener("click", window_click_fn);
    window.addEventListener("keydown", window_keydown_fn);
    
    (function(el) {
        el.onfocus = () => isActive.Set(true);
        el.onmouseover = () => isActive.Set(true);
        el.onmouseout = () => isActive.Set(false);
        el.onblur = () => isActive.Set(false);
        el.onkeydown = (ev) => { if (ev.keyCode == 13) document.querySelector("form#search").submit()};
    }).call(null, root.querySelector(".search input"));
    
    (function(el) {
        el.onclick = () => clear_fn();
        el.onkeydown = (ev) => { if (ev.keyCode == 13) clear_fn() };
    }).call(null, root.querySelector(".search .clear"));
    
    (function(el) {
        el.onclick = () => isExpanded.Swap(reverse);
        el.onkeydown = (ev) => { if (ev.keyCode == 13) isExpanded.Swap(reverse) };
    }).call(null, root.querySelector(".search .expand"));
    
    expand_block = root.querySelector(".search-form");
    isActive.Subscribe(activate_fn);
	isExpanded.Subscribe(expand_fn);
    script.remove();
}

component_did_mount();
}).call();
