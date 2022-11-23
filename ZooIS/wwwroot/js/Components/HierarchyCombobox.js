(function() {
	//Flags
	let isSearchable = false;
	let isMultiselect = false;
	let isInstant = false;
	
	//Params
	let Url = "";
	let SearchParams = new State({});
	let Dependencies = {};
	let MaxCount = null;
	let default_placeholder = "";
	let disabled_placeholder = "";

	//Local vars
	let Value = new State([]);
	let isExpanded = new State(false);
	let isActive = new State(false);
	let isBlocked = new State(false);
	let root = HTMLElement;
	let expand_block = HTMLElement;
	let top_list = null;
	let placeholder = "";
	let items = new State([]);

	let last_search = null;
	
	//Methods
	function click_fn(ev) {
		if (!isBlocked.Get() && ev.target != root.querySelector(".clear"))
			isExpanded.Swap(reverse);
	}
	
	function window_fn(ev) {
		if (!root.contains(ev.target))
			isExpanded.Set(false);
		}
	
	function window_key_fn(ev) {
		if (ev.keyCode == 13) {
			if (ev.target == root.querySelector(".search-wrap"))
				isExpanded.Swap(reverse);
			else if (!root.contains(ev.target))
				isExpanded.Set(false);
		}
		if (ev.keyCode == 27)
			isExpanded.Set(false);
	}
	
	function search(SearchParams) {
		if (!Url || SearchParams["q"] === last_search)
			return;
		let Query = new URLSearchParams(SearchParams).toString();
		let xhr = new XMLHttpRequest();
		xhr.open("GET", `${Url}?${Query}`);
		xhr.responseType = "json";
		xhr.onload = (ev) => xhr_load(ev, search_success);
		xhr.onerror = xhr_error;
		xhr.send();
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
		items.Set(format_resp(resp));
	}

	function active_fn(isActive) {
		let el = root.querySelector(".search-wrap");
		if (isActive)
			el.classList.add("active");
		else
			el.classList.remove("active");
	}

	function expand_fn(isExpanded) {
		if (isExpanded) {
			rerender(expand_block);
			root.appendChild(expand_block);
		}
		else
			expand_block.remove();
	}

	function block_fn(isBlocked) {
		let input = root.querySelector(".search-wrap");
		if (isBlocked) {
			isExpanded.Set(false);
			input.tabIndex = -1;
			input.querySelector(".clear").tabIndex = -1;
			input.setAttribute("disabled", true);
			placeholder.textContent = disabled_placeholder;
		}
		else {
			input.tabIndex = 0;
			input.querySelector(".clear").tabIndex = 0;
			input.setAttribute("disabled", false);
			placeholder.textContent = default_placeholder;
		}
	}
	
	function clear_fn() {
		Value.Set([]);
	}

	function isChildSelected(item) {
		return item["items"].some(item =>
		Value.Get().map((item) => item["value"]).includes(item["id"]) ||
		item["items"] &&
		isChildSelected(item));
	}

	function isChildSearched(item) {
		return item["items"].some(item =>
		item["isSearched"] ||
		item["items"] &&
		isChildSearched(item));
	}

	function push_value(Value, item) {
		if (isMultiselect) {
			if (MaxCount && Value.length == MaxCount)
			Value.pop();
			Value.push(item);
		}
		else
			Value = [item];
		return Value;
	}

	function remove_value(Value, item) {
		Value.splice(Value.map((item) => item["value"]).indexOf(item["value"]), 1);
		return Value;
	}

	function SetValue(item) {
		let value = {value:   item["id"] ?? item["value"],
			 		 display: item["display"]};
		if (isInstant) {
			let SearchParams = new URLSearchParams(window.location.search);
			SearchParams.set("report", value["value"]);
			window.location.search = SearchParams.toString();
			return;
		}
		if (!Value.Get().map((item) => item["value"]).includes(value["value"]))
			Value.Swap((Value) => push_value(Value, value));
		else
			Value.Swap((Value) => remove_value(Value, value));
		if (!isMultiselect)
			setTimeout(() => isExpanded.Set(false), 10);
	}

	function render_item(item, level) {
		let isExpanded = new State(false);
		let items = HTMLUListElement;
		
		let div = document.createElement("div");
		div.id = "id" + item["id"];
		let li = document.createElement("li");
		let pads = document.createElement("div");
		pads.classList.add("pad");
		for (i = 0; i <= level; i++) {
			let pad = document.createElement("div");
			pad.classList.add(i == level ? item["items"].length == 0 ? "empty" : "collapsed" : "none")
			if (i == level && item["items"].length > 0) {
				let img = document.createElement("img");
				img.src = "/static/chevron-right.svg";
				pad.appendChild(img);
				pad.tabIndex = 0;
			}
			pads.onclick = () => isExpanded.Swap(reverse);
			pads.onkeydown = (ev) => { if (ev.keyCode == 13) isExpanded.Swap(reverse) };
			pads.appendChild(pad);
		}
		li.appendChild(pads);
		let ctn = document.createElement("div");
		ctn.textContent = item["display"];
		ctn.classList.add("display");
		ctn.tabIndex = 0;
		ctn.onclick = () => SetValue(item);
		ctn.onkeydown = (ev) => {
			if (ev.keyCode == 13)
				SetValue(item)};
		if (Value.Get().map((item) => item["value"]).includes(item["id"]))
		ctn.classList.add("selected");
		li.appendChild(ctn);
		div.appendChild(li);
		if (item["items"].length > 0) {
			items = render_items(item["items"], level+1)
			isExpanded.Subscribe((isExpanded) => {
				li.querySelector("img").src = isExpanded ? "/static/chevron-down.svg" : "/static/chevron-right.svg";
				if (isExpanded) {
					rerender(items);
					div.appendChild(items);
				}
				else
					items.remove();
			});
			if (isChildSelected(item) || isChildSearched(item))
				isExpanded.Set(true);
		}
		return div;
	}

	function rerender(items) {
		Array.from(items.querySelectorAll(".selected")).forEach(el =>
			el.classList.remove("selected"));
		Value.Get().forEach(item => {
			let el = items.querySelector(`#id${item["value"]}`);
			if (el) el.querySelector(".display").classList.add("selected")
		});
	}

	function render_items(items, level) {
		let ul = HTMLUListElement;
		if (level == 0) {
			ul = expand_block.querySelector("ul");
			ul.textContent = "";
		}
		else
			ul = document.createElement("ul");
		items.forEach(item => ul.appendChild(render_item(item, level)));
		return ul;
	}

	function RenderValue(Items) {
		root.querySelector("select").textContent = "";
		Array.from(root.querySelectorAll(".selected")).forEach(el =>
			el.classList.remove("selected"));
		if (top_list)
			Array.from(top_list.children).forEach(el =>
				el.remove());
		if (Items.length != 0) {
			Items.forEach(item => {
				if (root.querySelector(`#id${item["value"]} .display`))
				root.querySelector(`#id${item["value"]} .display`).classList.add("selected");

				let option = document.createElement("option")
				option.value = item["value"];
				option.setAttribute("selected", true);
				root.querySelector("select").appendChild(option);

				if (isMultiselect) {
					let display = document.createElement("li");
					display.innerHTML = `
						<div class="display">
							${item["display"]}
						</div>
					`;
					top_list.appendChild(display);
				}
			})
			if (!isMultiselect)
				if (root.querySelector(".placeholder")) {
					let display = document.createElement("div");
					display.classList.add("display");
					display.textContent = Items.at(0)["display"];
					init_display(display);
					root.querySelector(".placeholder").replaceWith(display);
				}
				else
					root.querySelector(".search-wrap .display").textContent = Items.at(0)["display"];
		}
		else if (!isMultiselect && root.querySelector(".search-wrap .display"))
			root.querySelector(".search-wrap .display").replaceWith(placeholder);
	}
	
	function EvalDependancy(id, value) {
		if (Object.entries(Dependencies).some(([k, v]) => jQuery.isEmptyObject(v.Get())))
			isBlocked.Set(true);
		else
			isBlocked.Set(false);
		SearchParams.Swap(SearchParams => {
			SearchParams[id] = value;
			return SearchParams;
		})
	}

	function set_search_query(params) {
		params["q"] = root.querySelector(".search-input input").value;
		return params;
	}

	function GetItems(items) {
		Array.from(root.querySelector(".expand-block ul").children).forEach(el => {
			let item = {id:      el.id,
						display: el.textContent,
						items:   []};
			items.push(item);
		})
		return items;
	}

	function init_display(el) {
		if (el == null)
			return;
		el.onclick = click_fn;
		el.onmouseover = () => isActive.Set(true);
		el.onmouseout = () => isActive.Set(false);
	}

	function component_did_mount() {
		let script = document.currentScript;
		let params = JSON.parse(script.textContent);

		isSearchable = params["flags"]["isSearchable"];
		isMultiselect = params["flags"]["isMultiselectable"];
		isInstant = params["flags"]["isInstant"]
		disabled_placeholder = params["disabledPlaceholder"];
		MaxCount = params["maxCount"];
		Url = params["url"];
		SearchParams.Set(params["searchParams"]);
		SearchParams.Swap((params) => {
			delete params["$id"];
			return params;
		});

		// For now works only for select;
		params["dependencies"]["$values"].forEach(id => {
			Dependencies[id] = new State(null);
			let el = document.querySelector(`main #${id}`);
			switch (el.tagName) {
				case "SELECT":
					Dependencies[id].Set(Array.from(el.children).map(item => item.value))
					let observer = new MutationObserver((ev) => Dependencies[id].Set(Array.from(ev[0]["target"].children).map(item => item.value)));
					observer.observe(el, {childList: true});
					break;
			}
			Dependencies[id].Subscribe((value) => EvalDependancy(id, value))
		});

		SearchParams.Subscribe(search);

		root = GetRoot(script);
		items.Swap(GetItems);
		window.addEventListener("click", window_fn);
		window.addEventListener("keydown", window_key_fn);
		root.querySelector(".clear").onclick = clear_fn;
		root.querySelector(".clear").onkeydown = (ev) => { if (ev.keyCode == 13) clear_fn() };
		(function(el) {
			el.onfocus = () => isActive.Set(true);
			el.onblur = () => isActive.Set(false);
		}).call(this, root.querySelector(".search-wrap"));
		init_display(root.querySelector(".search-wrap .display"));
		init_display(root.querySelector(".search-wrap .placeholder"));
		if (isSearchable) {
			root.querySelector(".search-input .send").onclick = () => SearchParams.Swap(set_search_query); 
			root.querySelector(".search-input .send").onkeydown = (ev) => { if (ev.keyCode == 13) SearchParams.Swap(set_search_query) };
			root.querySelector(".search-input input").onkeydown = (ev) => {
				if (ev.keyCode == 13) SearchParams.Swap(set_search_query) };
		}
			if (isMultiselect)
			top_list = root.querySelector("ul");
		expand_block = root.querySelector(".expand-block");
		placeholder = root.querySelector(".placeholder");
		default_placeholder = placeholder.textContent;
		if (isMultiselect) {
			let values = Array.from(root.querySelector("select").children);
			let display = Array.from(root.querySelector("ul").children);
			for (i = 0; i < values.length; i++) {
				let value = {value:   values[i].value,
							 display: display[i].textContent.trim()};
				Value.Swap((Value) => push_value(Value, value));
			}
		}
		else if (root.querySelector("select").children.length > 0) {
			Value.Set([{value:   root.querySelector("select").value,
						display: root.querySelector(".display").textContent}])
			root.querySelector(".display").remove();
		};

		isActive.Subscribe(active_fn)
		isExpanded.Subscribe(expand_fn);
		isBlocked.Subscribe(block_fn)
		Value.Subscribe(RenderValue);
		items.Subscribe((items) => render_items(items, 0));

		script.remove();
	}
	
	component_did_mount();
}).call();

window.addEventListener("keydown", (ev) => { if (ev.keyCode == 13) ev.preventDefault()})