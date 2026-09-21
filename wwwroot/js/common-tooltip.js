function initializeTruncatedTooltips() {

    const elements =
        document.querySelectorAll(
            "[data-tooltip-truncate]"
        );

    elements.forEach(function (element) {

        const tooltipText =
            element.getAttribute(
                "data-tooltip-text"
            );

        const isTruncated =
            element.scrollWidth >
            element.clientWidth;

        const existingTooltip =
            bootstrap.Tooltip.getInstance(
                element
            );

        if (existingTooltip) {
            existingTooltip.dispose();
        }

        element.removeAttribute("title");
        element.removeAttribute("data-bs-original-title");
        element.removeAttribute("data-bs-title");

        if (isTruncated && tooltipText) {

            element.setAttribute(
                "data-bs-title",
                tooltipText
            );

            bootstrap.Tooltip.getOrCreateInstance(
                element,
                {
                    placement: "top"
                }
            );
        }
    });
}


document.addEventListener(
    "DOMContentLoaded",
    function () {

        initializeTruncatedTooltips();
    }
);


window.addEventListener(
    "resize",
    function () {

        initializeTruncatedTooltips();
    }
);