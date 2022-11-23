(function() {
    let root = HTMLElement;
    let input = HTMLInputElement;
    let checkbox = HTMLElement;
    let isChecked = new State(false);

    function SetState(state) {
        if (state) {
            input.setAttribute("checked", "checked");
            checkbox.classList.add("checked");
        }
        else {
            input.removeAttribute("checked");
            checkbox.classList.remove("checked")
        }
    }

    function on_click() {
        isChecked.Swap(reverse);
    }

    function on_keydown(ev) {
        if (ev.keyCode == 13)
            isChecked.Swap(reverse);
    }

    function component_did_mount() {
        let script = document.currentScript;
        root = GetRoot(script);
        input = root.querySelector("input");
        checkbox = root.querySelector(".checkbox");
        checkbox.addEventListener("click", on_click);
        checkbox.addEventListener("keydown", on_keydown);
        isChecked.Set(input.checked);
        isChecked.Subscribe(SetState);
        script.remove();
    }

    component_did_mount();
}).call();