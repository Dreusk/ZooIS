(function() {
    //Params
    let Url = "";
    let SearchParams = new URLSearchParams();
    //Local
    let root = HTMLElement;
    let list = HTMLUListElement;
    let msg = null;
    let isLoading = new State(false);
    let isSearch = new State(true);
    let currentPage = 1;

    function on_window_scroll() {
        if (!isSearch.Get())
            return;
        window.removeEventListener("scroll", on_window_scroll);
        setTimeout(() => window.addEventListener("scroll", on_window_scroll), 300);

        if (window.innerHeight + window.scrollY > document.documentElement.scrollHeight * 0.90)
            search();
    }

    function scroll_to_top() {
        let duration = 2;
        let step = 10;
        
        let nextPosition = window.scrollY;
        let tick = 0;
        let tickMax = nextPosition / step;

        while (tick < tickMax) {
            console.log(nextPosition);
            setTimeout(() => window.scrollTo(0, nextPosition), tick * 1000);
            tick += 1;
            nextPosition -= step;
        }
    }

    function search() {
        if (!Url)
			return;
        SearchParams.set("page", currentPage);
        let Query = SearchParams.toString();
        let xhr = new XMLHttpRequest();
        xhr.open("GET", `${Url}?${Query}`);
        xhr.responseType = "json";
		xhr.onload = (ev) => xhr_load(ev, search_success);
		xhr.onerror = xhr_error;
		xhr.send();
        isLoading.Set(true);
    }

    function format_resp(resp) {
		if (resp == null)
			return [];
		let items = resp["$values"];
		items.forEach(item => item["items"] = format_resp(item["items"]));
		return items;
	}

	function search_success(resp, xhr) {
		last_search = (new URL(xhr.responseURL)).searchParams.get("q");
        currentPage++;
		load_items(format_resp(resp));
	}

    function load_items(new_items) {
        window.Infinite.Swap(items => {
            new_items.forEach(item => {
                items.push(item)})
            return items;});
        isLoading.Set(false);
        if (new_items.length > 0)
            isSearch.Set(true);
        else
            render_message("empty result");
    }

    function render_message(state) {
        if (msg != null)
            msg.remove();
        msg = document.createElement("div");
        msg.classList.add("msg");
        switch (state) {
            case "loading":
                isSearch.Set(false);
                msg.innerHTML = `
                    <img src="/static/loading.svg" class="loading" title="Загрузка" />
                    <div class="loading">Загрузка</div>
                `;
                break;
            case "empty result":
                msg.innerHTML =  window.Infinite.Get().length == 0 ? "Нет данных" : "Усё";
                break;
            case null:
                msg = null;
                return;
            default:
                msg.textContent = state;
        }
        root.appendChild(msg);
    }

    function component_did_mount() {
        let script = document.currentScript;
        let params = JSON.parse(script.textContent);
        SearchParams = new URLSearchParams(window.location.search);
        Object.entries(params["searchParams"]).forEach(([param, value]) => {
            if (param == '$id')
                return;
            SearchParams.set(param, value);
        });
        root = GetRoot(script);
        Url = params["url"];
        search();
        isLoading.Subscribe((isLoading) => {
            if (isLoading)
                render_message("loading");
            else
                render_message(null);
        })
        window.addEventListener("scroll", on_window_scroll);
        script.remove();
    }

    component_did_mount();
}).call(this);
