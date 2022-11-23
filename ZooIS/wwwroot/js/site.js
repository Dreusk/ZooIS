class State {
    #Value = new Object();
    #Subscriptions = new Array();

    constructor(value){
        this.#Value = value;
    }

    Subscribe(fn) {
        this.#Subscriptions.push(fn);
		fn(this.#Value);
    }
	
	#RenderState() {
		this.#Subscriptions.forEach(sub => {
            sub(this.#Value);
        })
	}

    /** 
    * Add this state to global state as a `key` under window.state object.
    */
    Globalize(key) {
        window.state[key] = this.#Value;
    }

    /**
     * Changes the value of this state to `value` and re-runs the subscribed functions.
     */
    Set(value) {
		if (this.#Value == value)
			return;
        this.#Value = value;
		this.#RenderState();
    }

    /**
     * Changes the value of this state by calling `fn` on it and re-runs the subscribed functions.
     */
     Swap(fn) {
		let tmp = fn(structuredClone(this.#Value));
		if (this.#Value == tmp)
			return;
        this.#Value = tmp;
		this.#RenderState();
    }

    /**
     * Returns value of this state.
     */
    Get() {
        return this.#Value;
    }
}

function reverse(bool){
    bool = !bool;
    return bool;
}

function GetRoot(script) {
    let root = script.nextElementSibling;
    while (root.tagName == "SCRIPT")
        root = root.nextElementSibling;
    return root;
}

class Modal {
    #modal = HTMLDivElement;
    #body = HTMLElement;
    #isShown = new State(false);
    #disabled_els = new Array();

    #change_state(isShown) {
        if (isShown) {
            Array.from(document.body.children).forEach(el => {
                el.querySelectorAll("*[tabIndex]").forEach (el => {
                    el.setAttribute("tabIndex", -1);
                    this.#disabled_els.push(el);
                });
                el.querySelectorAll("a").forEach (el => {
                    el.setAttribute("tabIndex", -1);
                    this.#disabled_els.push(el);
                });
                el.querySelectorAll("button").forEach (el => {
                    el.setAttribute("tabIndex", -1);
                    this.#disabled_els.push(el);
                });
            });
        }
        else {
            this.#disabled_els.forEach(el => {
                el.setAttribute("tabIndex", 0);
            });
            this.#modal.remove();
        }
    }

    constructor(body) {
        this.#isShown.Set(true);
        this.#isShown.Subscribe((state) => this.#change_state(state));
        this.#render_modal(body);
    }

    #click_close_fn(ev) {
        if (!this.#body.contains(ev.target)) {
            this.#isShown.Set(false);
        }
    }

    #render_body(body) {
        this.#body = document.createElement("div");
        this.#body.classList.add("modal-body");
        this.#body.appendChild(this.#render_cancel());
        console.log(body);
        this.#body.appendChild(body);
        body.querySelectorAll("button").forEach(btn => {
            btn.addEventListener("click", () => this.#cancel_fn());
            btn.addEventListener("keydown", (ev) => {if (ev.keyCode == 13) this.#cancel_fn()});
        })
        return this.#body;
    }
    
    #cancel_fn() {
        this.#isShown.Set(false);
    }

    #render_cancel() {
        let el = document.createElement("div");
        el.classList.add("d-flex", "justify-content-end");
        let btn = document.createElement("button");
        btn.classList.add("cancel");
        btn.onclick = () => this.#cancel_fn();
        btn.onkeydown = (ev) => {if (ev.keyCode == 13) this.#cancel_fn()} ;
        btn.textContent = "X";
        el.appendChild(btn);
        return el;
    }

    #render_modal(body) {
        this.#modal = document.createElement("div");
        this.#modal.classList.add("modal");
        this.#modal.onclick = (ev) => this.#click_close_fn(ev);
        this.#modal.appendChild(this.#render_body(body));
        document.body.appendChild(this.#modal);
    }
}

///Xhr utils
function xhr_load(ev, success_fn) {
    if (ev.target.status != 200 ||
        ev.target.responseURL.includes("AccessDenied"))
        xhr_fail(ev.target);
    else
        success_fn(ev.target.response, ev.target);
}

function xhr_error(ev) {
    let error = document.createElement("div");
    let msg = "Проблема сети";
    error.classList.add("flash");
    error.classList.add("error");
    error.textContent = msg;
    let timer = setTimeout(() => error.remove(), 4000);
    error.onclick = () => {
        error.remove();
        clearTimeout(timer); }
    document.body.appendChild(error);
}

function xhr_fail(xhr) {
    let error = document.createElement("div");
    let msg = "";
    switch (xhr.status) {
        case 404:
            msg = "Ресурс не найден"
            break;
        case 500:
            msg = "Внутренняя ошибка сервера";
            break;
        case 200:
            msg = "Недостаточно прав";
            break;
        default:
            msg = "Неизвестная ошибка";
    }
    error.classList.add("flash");
    error.classList.add("error");
    error.textContent = msg;
    let timer = setTimeout(() => error.remove(), 4000);
    error.onclick = () => {
        error.remove();
        clearTimeout(timer); }
    document.body.appendChild(error);
}

(function() {
    function PreventEnter(ev) {
        if (ev.keyCode == 13 && ev.target.tagName == "INPUT ")
            ev.preventDefault();
    }

    function reset_state() {
        window.state = {};
    }

    function component_did_mount() {
        let script = document.currentScript;
        window.state = {};
        window.addEventListener("hashchange", reset_state)
        window.addEventListener("keydown", PreventEnter);
        script.remove();
    }

    component_did_mount();
}).call()