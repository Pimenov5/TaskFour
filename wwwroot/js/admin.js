export async function resetUsersTable() {
    const response = await fetch("api/getusers", {
        method: "GET",
        headers: { "Accept": "application/json", "Content-Type": "application/json" }
    });

    if (response.status !== 200) {
        const error = await response.json();
        alert(response.status + ": " + error);
        return;
    }

    const json = await response.json();

    var i = 0;
    document.querySelector("tbody")?.remove();
    const tbody = document.createElement("tbody");
    tbody.id = "usersTbody";

    json.users.forEach(user => {
        const tr = document.createElement("tr");

        const input = document.createElement("input");
        input.class = "form-check-input";
        input.type = "checkbox";
        input.id = "userCheckbox" + i++;
        input.name = user.id;

        const label = document.createElement("label");
        label.class = "form-check-inline";
        label.append(input);

        const div = document.createElement("div");
        div.class = "form-check-inline";
        div.append(label);

        var td = document.createElement("td");
        td.append(div);
        tr.append(td);

        td = document.createElement("td");
        td.append(user.name);
        tr.append(td);

        td = document.createElement("td");
        td.append(user.email);
        tr.append(td);

        td = document.createElement("td");
        td.append(user.status);
        tr.append(td);

        td = document.createElement("td");
        td.append(user.lastSignIn);
        tr.append(td);

        tbody.append(tr);
    });

    document.getElementById("allUsersCheckbox").name = i;

    const table = document.getElementById("usersTable");
    table.append(tbody);
}

export function allUsersCheckboxOnClick() {
    const allCheckbox = document.getElementById("allUsersCheckbox");
    const count = allCheckbox.name;

    for (var i = 0; i < count; i++) {
        const checkbox = document.getElementById("userCheckbox" + i);
        checkbox.checked = allCheckbox.checked;
    }
}

export async function blockUsersButtonOnClick() {
    await setUsersStatus(2);
}

export async function unblockUsersButtonOnClick() {
    await setUsersStatus(1);
}

export async function deleteUsersButtonOnClick() {
    await deleteUsers(null);
}

export async function cleanUsersButtonOnClick() {
    await deleteUsers(true);
}

export async function signOutButtonOnClick() {
    const response = await fetch("api/signout", {
        method: "POST",
        headers: { "Accept": "application?json", "Content-Type": "application/json" }
    });

    if (await isValidResponse(response, 200))
        window.location.href = response.url;
}

function getCheckedIds() {
    const ids = [];
    const count = document.getElementById("allUsersCheckbox").name;
    for (var i = 0; i < count; i++) {
        const checkbox = document.getElementById("userCheckbox" + i);
        if (checkbox.checked)
            ids.push(checkbox.name);
    }

    return ids;
}

import { isValidResponse } from "/js/functions.js";

async function deleteUsers(onlyNotVerified) {
    const ids = getCheckedIds();

    const response = await fetch("api/deleteusers", {
        method: "DELETE",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            onlyNotVerified: onlyNotVerified,
            ids: ids
        })
    });

    if (await isValidResponse(response, 200))
        window.location.href = "/admin.html";
}

async function setUsersStatus(status) {
    const ids = getCheckedIds();

    const response = await fetch("api/setusersstatus", {
        method: "PATCH",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            status: status,
            ids: ids
        })
    });

    if (await isValidResponse(response, 200))
        window.location.href = "/admin.html";
}