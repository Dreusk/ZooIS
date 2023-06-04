(function() {
    let root = HTMLElement;
    let list = HTMLElement;
    let RenderedItemsCount = 0;

    function render_items(items) {
        if (RenderedItemsCount >= items.length)
            return;
        items.slice(RenderedItemsCount).forEach(item => {
            let card = document.createElement("div");
            card.innerHTML = `
            <a class="text-inherit" href="/Users/Show/${item["id"]}">
                <div class="card">
                    <div>
                        <span class="text-muted">Имя пользователя:</span>
                        <span>${item["display"]}</span>
                    </div>
                    <div>
                        <span class="text-muted">Роли:</span>
                        <span>${item["roles"]["$values"].map(role => role["display"]).join(', ')}</span>
                    </div>
                </div>
            </a>`
            list.appendChild(card);
        });
    }

    function component_did_mount() {
        let script = document.currentScript;
        root = document.querySelector("main");
        list = root.querySelector(".list");

        window.Infinite = new State([]);
        window.Infinite.Subscribe(render_items);

        script.remove();
    }

    component_did_mount();
}).call(this);
