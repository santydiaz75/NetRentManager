const sidebarNav = document.getElementById("sidebar-nav");
const sidebarToggle = document.getElementById("sidebar-toggle");
const sidebarOverlay = document.getElementById("sidebar-overlay");

if (sidebarNav && sidebarToggle && sidebarOverlay) {
    const closeMenu = () => {
        document.body.classList.remove("is-sidebar-open");
        sidebarNav.setAttribute("aria-expanded", "false");
        sidebarToggle.setAttribute("aria-expanded", "false");
        sidebarOverlay.setAttribute("aria-hidden", "true");
    };

    const toggleMenu = () => {
        const nextState = !document.body.classList.contains("is-sidebar-open");

        document.body.classList.toggle("is-sidebar-open", nextState);
        sidebarNav.setAttribute("aria-expanded", nextState ? "true" : "false");
        sidebarToggle.setAttribute("aria-expanded", nextState ? "true" : "false");
        sidebarOverlay.setAttribute("aria-hidden", nextState ? "false" : "true");
    };

    sidebarToggle.addEventListener("click", toggleMenu);
    sidebarOverlay.addEventListener("click", closeMenu);
    sidebarNav.addEventListener("click", closeMenu);

    window.addEventListener("resize", () => {
        if (window.innerWidth >= 1024) {
            closeMenu();
        }
    });
}
