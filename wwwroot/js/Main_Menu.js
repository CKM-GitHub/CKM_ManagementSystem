const menuParent = document.querySelector(".menu-parent");
const submen = document.querySelector(".submenu");

menuParent.addEventListener("click", function () {
    submenu.classList.toggle("show");
});