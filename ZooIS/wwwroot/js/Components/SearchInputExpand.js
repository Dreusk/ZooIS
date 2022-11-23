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
        root.querySelector(".search").classList.add("remove");
}

function expand_fn(isExpanded) {
	if (isExpanded)
        root.appendChild(expand_block);
    else
        expand_block.remove();
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
        el.onblur = () => isActive.Set(false);
    }).call(root.querySelector(".search input"));
    
    (function(el) {
        el.onclick = () => clear_fn;
        el.onkeydown = (ev) => { if (ev.keyCode == 13) clear_fn };
    }).call(root.querySelector(".search .clear"));
    
    (function(el) {
        el.onclick = () => isExpanded.Swap(reverse);
        el.onkeydown = (ev) => { if (ev.keyCode == 13) isExpanded.Swap(reverse) };
    }).call(root.querySelector(".search .expand"));
    
    expand_block = root.querySelector(".search-form");
    isActive.Subscribe(activate_fn);
	isExpanded.Subscribe(expand_fn);
    script.remove();
}

component_did_mount();
}).call();