document.addEventListener("DOMContentLoaded", function () {

    const mainMenuContainer =
        document.getElementById("mainMenuContainer");

    if (mainMenuContainer) {
        const cachedMenu = sessionStorage.getItem("mainMenuHtml");
        if (cachedMenu) {
            mainMenuContainer.innerHTML = cachedMenu;

            setActiveMenu();
            setupMenuState();
        }
        fetch("/MainMenu/Index")
            .then(response => response.text())
            .then(html => {
                sessionStorage.setItem(
                    "mainMenuHtml", html
                );
                if (html !== cachedMenu) {
                    mainMenuContainer.innerHTML = html;
                    setActiveMenu();
                    setupMenuState();
                }
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
        normalizePath(window.location.pathname);

    const menuLinks =
        document.querySelectorAll(
            ".menu-link, .submenu-link"
        );

    let activeSubmenu = null;

    menuLinks.forEach(function (link) {

        link.classList.remove("active");

        const linkPath =
            normalizePath(
                new URL(link.href).pathname
            );

        if (linkPath === currentPath) {

            link.classList.add("active");

            const submenu =
                link.closest(".submenu");

            if (submenu) {

                activeSubmenu = submenu;

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


    if (activeSubmenu) {

        const collapse =
            bootstrap.Collapse.getOrCreateInstance(
                activeSubmenu,
                {
                    toggle: false
                }
            );

        collapse.show();

        localStorage.setItem(
            "openMainMenu",
            activeSubmenu.id
        );

        return;
    }


    restoreOpenMenu();
}


function setupMenuState() {

    const collapseMenus =
        document.querySelectorAll(
            ".submenu.collapse"
        );

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
        localStorage.getItem(
            "openMainMenu"
        );

    if (!savedMenu) {
        return;
    }

    const menu =
        document.getElementById(
            savedMenu
        );

    if (!menu) {
        return;
    }

    const collapse =
        bootstrap.Collapse.getOrCreateInstance(
            menu,
            {
                toggle: false
            }
        );

    collapse.show();

    const button =
        document.querySelector(
            `[data-bs-target="#${savedMenu}"]`
        );

    if (button) {

        button.setAttribute(
            "aria-expanded",
            "true"
        );

        button.classList.remove(
            "collapsed"
        );
    }
}


function normalizePath(path) {

    if (!path) {
        return "/";
    }

    let normalized =
        path.toLowerCase();

    if (
        normalized.length > 1 &&
        normalized.endsWith("/")
    ) {
        normalized =
            normalized.slice(0, -1);
    }

    return normalized;
}