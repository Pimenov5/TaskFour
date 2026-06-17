export async function signInButtonOnClick() {
    const email = document.getElementById("signInEmailInput").value.trim();
    const password = document.getElementById("signInPasswordInput").value.trim();
    if (email == null || email === "" || password == null || password === "") {
        alert("Email and password cannot be emty");
        return;
    }

    const response = await fetch("api/signin", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            email: email,
            password: password
        })
    });

    if (response.status === 200) {
        window.location.href = response.url;
    }
    else {
        const error = await response.json();
        alert(response.status + ": " + error);
    }
}

export async function signUpButtonOnClick() {
    const name = document.getElementById("signUpNameInput").value.trim();
    const email = document.getElementById("signUpEmailInput").value.trim();
    const password = document.getElementById("signUpPasswordInput").value.trim();
    const repeatPassword = document.getElementById("signUpRepeatPasswordInput").value.trim();
    if (name == null || name === "" || email == null || email === "" || password == null || password === "" || repeatPassword == null || repeatPassword === "") {
        alert("Name, email and both passwords cannot be emty");
        return;
    }
    
    const response = await fetch("api/signup", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            name: name,
            email: email,
            password: password,
            repeatPassword: repeatPassword
        })
    });
    
    if (response.status === 200) {
        const href = await response.json();
        const element = document.getElementById("signUpVerifyA");
        element.href = href;
        element.style.visibility = "visible";
    }
    else {
        const error = await response.json();
        alert(response.status + ": " + error);
    }
}