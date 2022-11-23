(function () {
    let root = HTMLElement;

    function SlideCards() {
        let Cards = root.querySelectorAll(".card");
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

    function SetPaging() {
        let paging = root.querySelector(".paging");
        let input = paging.querySelector("input");

        paging.children[0].onclick = () => {
            input.value = 1;
            root.querySelector("form").submit();
        };
        paging.children[0].onkeydown = (ev) => { if (ev.keyCode == 13) {
            input.value = 1;
            root.querySelector("form").submit();
        }};

        paging.children[1].onclick = () => {
            input.value = parseInt(input.value) - 1;
            root.querySelector("form").submit();
        };
        paging.children[1].onkeydown = (ev) => { if (ev.keyCode == 13) {
            input.value = parseInt(input.value) - 1;
            root.querySelector("form").submit();
        }};

        paging.children[3].onclick = () => {
            input.value = parseInt(input.value) + 1;
            root.querySelector("form").submit();
        };
        paging.children[3].onkeydown = (ev) => { if (ev.keyCode == 13) {
            input.value = parseInt(input.value) + 1;
            root.querySelector("form").submit();
        }};

        paging.children[2].onchange = () => root.querySelector("form").submit();
        paging.children[2].onkeydown = (ev) => { if (ev.keyCode == 13) root.querySelector("form").submit() };
    }

    function component_did_mount() {
        let script = document.currentScript;
        root = document.querySelector("main");
        SlideCards();
        SetPaging();
        script.remove();
    }

    component_did_mount();
}).call()