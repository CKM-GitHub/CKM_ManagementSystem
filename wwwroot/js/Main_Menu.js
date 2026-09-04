document.addEventListener("DOMContentLoaded", function () {

    const mainMenuContainer =
        document.getElementById("mainMenuContainer");

    if (mainMenuContainer) {
        fetch("/MainMenu/Index")
            .then(response => response.text())
            .then(html => {
                mainMenuContainer.innerHTML = html;
            });
    }
    const profileContainer =
        document.getElementById("profileContainer");

    if (profileContainer) {
        fetch("/MainMenu/Profile")
            .then(response => response.text())
            .then(html => {
                profileContainer.innerHTML = html;
            });
    }

});