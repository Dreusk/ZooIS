(function() {
    let root = HTMLElement;

    function mark_as_read(id) {
        let xhr = new XMLHttpRequest();
        xhr.open("PATCH", `/Alerts/${item["guid"]}/mark_as_read`);
        xhr.send();
        xhr.onload = () => read_success(id);
    }

    function read_success(id) {
        let item = root.querySelector(`#id${id}`)
        item.querySelector("td:has(button)").remove();
        item.classList.add("text-muted");
    }

    function component_did_mount(){
        let script = document.currentScript;
        root = document.body.querySelector(".main");
        Array.from(root.querySelectorAll("tbody tr")).forEach(item => {
            
        });
        script.remove();
    }

component_did_mount();
}).call();