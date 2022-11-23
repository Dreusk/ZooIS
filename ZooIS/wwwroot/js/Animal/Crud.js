"use strict";
(function () {
    let root = HTMLDivElement;

    function render_image(ev) {
        Array.from(root.querySelectorAll("img")).forEach(el =>
            el.remove());
        let input = ev.target;
        Array.from(input.files).forEach(file => {
            let img = document.createElement("img");
            img.src = URL.createObjectURL(file);
            img.classList.add("photo");
            input.parentElement.insertBefore(img, input);
        });
    }
    function component_did_mount() {
        let script = document.currentScript;
        root = document.querySelector("main");
        root.querySelector("#Photo").onchange = render_image;
        script.remove();
    }
    component_did_mount();
}).call(this);
