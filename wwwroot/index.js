async function GetTODOList() {
    await fetch("/list-get")
        .then(data => data.json())
        .then(jsonData => BuildTODOTable(jsonData));
}

function BuildTODOTable(todoJSON) {
    const table = document.getElementsByTagName("tbody")[0];

    let listCount = 0;
    for (const todo of todoJSON) {
        listCount++;

        const tr = document.createElement("tr");
        const tdId = document.createElement("td");
        const tdTitle = document.createElement("td");
        const tdDescription = document.createElement("td");
        const tdDate = document.createElement("td");
        const tdEdit = document.createElement("td");

        tdId.textContent = listCount; // if is is removed
        tdTitle.textContent = todo.title;
        tdDescription.textContent = todo.description;
        tdDate.textContent = todo.date;

        const { buttonEdit, buttonRemove } = CreateEditButtons(todo);
        tdEdit.append(buttonEdit, buttonRemove);

        tr.append(tdId, tdTitle, tdDescription, tdDate, tdEdit);
        table.appendChild(tr);
    }
}

function CreateEditButtons(todoItem) {
    const buttonEdit = document.createElement("button");
    buttonEdit.textContent = "Edit";
    buttonEdit.dataset.itemId = todoItem.id;

    buttonEdit.addEventListener("click", async (ev) => {
        BuildForm("edit", todoItem);
    })

    const buttonRemove = document.createElement("button");
    buttonRemove.textContent = "Remove";
    buttonRemove.dataset.itemId = todoItem.id;

    buttonRemove.addEventListener("click", async (ev) => {
        SubmitRemove(todoItem.id);
    })

    return { buttonEdit, buttonRemove }
}

function BuildForm(type, todoItem = null) {

    const modal = CreateModal();

    const form = document.createElement("form");
    form.setAttribute("action", "/");

    let path, method, legendText;

    switch (type) {
        case "edit": path = `/list-edit/${todoItem.id}`; method = "PUT"; legendText = "Edit Item"; break;
        case "add": path = `/list-add`; method = "POST"; legendText = "Add Item"; break;
    }

    if (!path) {
        return;
    }

    form.addEventListener("submit", async (ev) => {
        ev.preventDefault();

        const body = new FormData(form);
        
        await fetch(path, {
            method,
            body
        });
    });

    const legend = document.createElement("legend");
    legend.textContent = legendText;
    form.appendChild(legend);

    const inputId = document.createElement("input");
    inputId.setAttribute("type", "hidden");
    inputId.setAttribute("name", "id");

    const labelTitle = document.createElement("label");
    labelTitle.setAttribute("for", "inputTitle");
    labelTitle.textContent = "Title:";
    const inputTitle = document.createElement("input");
    inputTitle.setAttribute("type", "text");
    inputTitle.setAttribute("id", "inputTitle");
    inputTitle.setAttribute("name", "title");

    const labelDescription = document.createElement("label");
    labelDescription.setAttribute("for", "inputDescription");
    labelDescription.textContent = "Description:";
    const inputDescription = document.createElement("textarea");
    inputDescription.setAttribute("row", "4");
    inputDescription.setAttribute("id", "inputDescription");
    inputDescription.setAttribute("name", "description");
    inputDescription.style.resize = "none";

    const labelDate = document.createElement("label");
    labelDate.setAttribute("for", "inputDate");
    labelDate.textContent = "Date:";
    const inputDate = document.createElement("input");
    inputDate.setAttribute("type", "text");
    inputDate.setAttribute("id", "inputDate");
    inputDate.setAttribute("name", "date");

    if (todoItem) {
        inputId.value = todoItem.id;
        inputTitle.value = todoItem.title;
        inputDescription.value = todoItem.description;
        inputDate.value = todoItem.date;
    }

    const inputSubmit = document.createElement("input");
    inputSubmit.setAttribute("type", "submit");

    form.append(inputId, labelTitle, inputTitle, labelDescription, inputDescription, labelDate, inputDate, inputSubmit);

    modal.appendChild(form);

    document.body.appendChild(modal);
}

function SubmitRemove(itemId) {
    const modal = CreateModal();

    const messageBlock = document.createElement("div");
    messageBlock.classList.add("message");

    const messageText = document.createElement("div");

    const buttonOk = document.createElement("button");
    buttonOk.textContent = "Ok";

    buttonOk.addEventListener("click", async (ev) => {
        await fetch(`/list-remove/${itemId}`, {
            method: "DELETE"
        });
    })    

    const buttonCancel = document.createElement("button");
    buttonCancel.textContent = "Cancel";

    messageBlock.append(messageText, buttonOk, buttonCancel);

    modal.appendChild(messageBlock);

    document.body.appendChild(modal);
}

function CreateModal() {
    const modal = document.createElement("div");
    modal.classList.add("modal");

    modal.addEventListener("mousedown", (ev) => {
        if (ev.target == modal) {
            modal.parentNode.removeChild(modal);
        }
    });

    return modal;
}