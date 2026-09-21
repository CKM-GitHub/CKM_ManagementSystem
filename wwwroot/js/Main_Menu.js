document.addEventListener("DOMContentLoaded", function () {

    setupMobileSidebar();

    loadSubNavigation();

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

                const dropdownToggle =
                    profileContainer.querySelector(
                        '[data-bs-toggle="dropdown"]'
                    );

                if (dropdownToggle) {
                    bootstrap.Dropdown.getOrCreateInstance(
                        dropdownToggle
                    );
                }
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

        activeSubmenu.classList.add("show");
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

    document.querySelectorAll(".submenu.show").forEach(function (openMenu) {
        if (openMenu.id !== savedMenu) {
            const openCollapse = bootstrap.Collapse.getOrCreateInstance(
                openMenu, {
                toggle: false
            }
            );
            openCollapse.hide();
        }
    });

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
function setupMobileSidebar() {

    const sidebar =
        document.getElementById("sidebar");

    const menuButton =
        document.getElementById("mobileMenuButton");

    const overlay =
        document.getElementById("sidebarOverlay");

    if (!sidebar || !menuButton || !overlay) {
        return;
    }

    menuButton.addEventListener("click", function () {

        sidebar.classList.add("mobile-open");
        overlay.classList.add("show");
    });

    overlay.addEventListener("click", function () {

        closeMobileSidebar();
    });

    document.addEventListener("click", function (event) {

        const menuLink =
            event.target.closest(
                ".menu-link, .submenu-link"
            );

        if (
            menuLink &&
            window.innerWidth <= 991.98
        ) {
            closeMobileSidebar();
        }
    });

    function closeMobileSidebar() {

        sidebar.classList.remove("mobile-open");
        overlay.classList.remove("show");
    }
}
function loadSubNavigation() {

    const container = document.getElementById(
        "subNavigationContainer"
    );

    if (!container) {
        return;
    }
    const pathParts = window.location.pathname
        .split("/")
        .filter(Boolean);

    if (pathParts.length === 0) {
        container.innerHTML = "";
        return;
    }

    const currentController = pathParts[0];

    const currentAction = pathParts.length > 1
        ? pathParts[1] : "index";

    const url =
        `/MainMenu/SubNavigation` +
        `?currentController=${encodeURIComponent(currentController)}` +
        `&currentAction=${encodeURIComponent(currentAction)}`;

    fetch(url)
        .then(response => response.text())
        .then(html => {
            container.innerHTML = html;
        })
        .catch(() => {
            container.innerHTML = "";
        });
}