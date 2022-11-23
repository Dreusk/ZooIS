(function() {
    let root = HTMLElement;

    function redirect(report) {
        console.log(report);
    }

    function component_did_mount() {
        root = document.body;
        Array.from(root.querySelectorAll()).forEach(el => {
            let report = el.id;
            el.onclick = () => redirect(report);
            el.onkeydown = (ev) => { if (ev.keyCode == 13) redirect(report) };
        })
    }

    component_did_mount();
}).call(this);