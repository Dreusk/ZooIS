function render_item(item) {
    let el = document.createElement("div");
    el.innerHTML = `
        <a href="/Reports/Show/${item["id"]}">
            ${item["display"]}
        </a>
    `
    return el;
}