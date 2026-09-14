document.addEventListener("DOMContentLoaded", function () {

    const mainMenuContainer =
        document.getElementById("mainMenuContainer");

    if (mainMenuContainer) {
        fetch("/MainMenu/Index")
            .then(response => response.text())
            .then(html => {
                mainMenuContainer.innerHTML = html;

                restoreOpenMenu();
                setActiveMenu();
                setupMenuState();
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


function setActiveMenu() {

    const currentPath =
        window.location.pathname.toLowerCase();

    const menuLinks =
        document.querySelectorAll(
            ".menu-link, .submenu-link"
        );

    menuLinks.forEach(function (link) {

        const linkPath =
            new URL(link.href).pathname.toLowerCase();

        if (linkPath === currentPath) {

            link.classList.add("active");

            const submenu =
                link.closest(".submenu");

            if (submenu) {

                submenu.classList.add("show");

                localStorage.setItem(
                    "openMainMenu",
                    submenu.id
                );

                const parentButton =
                    document.querySelector(
                        `[data-bs-target="#${submenu.id}"]`
                    );

                if (parentButton) {

                    parentButton.classList.add(
                        "parent-active"
                    );

                    parentButton.setAttribute(
                        "aria-expanded",
                        "true"
                    );

                    parentButton.classList.remove(
                        "collapsed"
                    );
                }
            }
        }
    });
}


function setupMenuState() {

    const collapseMenus =
        document.querySelectorAll(".submenu.collapse");

    collapseMenus.forEach(function (menu) {

        menu.addEventListener(
            "shown.bs.collapse",
            function () {

                localStorage.setItem(
                    "openMainMenu",
                    menu.id
                );
            }
        );

        menu.addEventListener(
            "hidden.bs.collapse",
            function () {

                const savedMenu =
                    localStorage.getItem(
                        "openMainMenu"
                    );

                if (savedMenu === menu.id) {
                    localStorage.removeItem(
                        "openMainMenu"
                    );
                }
            }
        );
    });
}


function restoreOpenMenu() {

    const savedMenu =
        localStorage.getItem("openMainMenu");

    if (!savedMenu) {
        return;
    }

    const menu =
        document.getElementById(savedMenu);

    if (!menu) {
        return;
    }

    menu.classList.add("show");

    const button =
        document.querySelector(
            `[data-bs-target="#${savedMenu}"]`
        );

    if (button) {

        button.setAttribute(
            "aria-expanded",
            "true"
        );

        button.classList.remove("collapsed");
    }
}