(function () {
    let root = HTMLElement;
    let collection = HTMLElement;
    let RenderedCardsCount = 0;

    function SlideCard(Cards) {
        let Delay = 0;
        let Duration = 0.5
        Cards.forEach(el => {
            Delay += 0.2;
            el.style.animationDelay = Delay + "s";
            el.style.animationDuration = Duration + "s";
            setTimeout(reset, (Delay + Duration - 0.1) * 1000, el)
        });
    }

    function reset(el) {
        el.classList.remove("animated");
    }

    function render_card(Animals) {
        if (RenderedCardsCount >= Animals.length)
            return;
        let Cards = [];
        Animals.slice(RenderedCardsCount).forEach(Animal => {
            let card = document.createElement("a");
            card.classList.add("text-inherit");
            card.href = `/Animals/Show/${Animal["id"]}`;
            card.innerHTML =
                `<div class="card animal animated text-center">
                    <img class="photo" src="${Animal["picturePath"] ?? "/static/NoPhoto.png"}" title="${Animal["display"]}" />
                    <div class="font-weight-bold">${Animal["display"]}</div>
                    <div class="text-muted">${Animal["species"]["display"]}</div>
                </div>`;
            collection.appendChild(card);
            Cards.push(card.querySelector(".card"));
        })
        RenderedCardsCount = Animals.length;
        SlideCard(Cards);
    }

    function component_did_mount() {
        let script = document.currentScript;
        root = document.querySelector("main");
        collection = root.querySelector(".collection");

        window.Infinite = new State([]);
        window.Infinite.Subscribe(render_card);

        script.remove();
    }

    component_did_mount();
}).call(this);
