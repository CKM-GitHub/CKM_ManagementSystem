(function () {
    "use strict";

    var ERROR_MEDIA = {
        "403": {
            type: "video",
            src: "/player/gobanana.mp4"
        },
        "404": {
            //type: "dotlottie",
            ////src: "https://lottie.host/a5e71762-8e33-4c21-a9d1-5fa817571271/PgZ5u1zzA7.lottie",
            ////hideCode: true,
            ////size: "lg"
            type: "video",
            src: "/player/otera.mp4"
        },
        "500": {
            type: "lottie",
            src: "https://lottie.host/0e9e6009-991d-41c1-aabc-db59c0231217/dP2IK97OhY.json"
        },
        "default": {
            type: "lottie",
            src: "https://lottie.host/0e9e6009-991d-41c1-aabc-db59c0231217/dP2IK97OhY.json"
        }
    };

    var LOTTIE_CDN = "https://unpkg.com/@lottiefiles/lottie-player@latest/dist/lottie-player.js";
    var DOTLOTTIE_CDN = "https://unpkg.com/@dotlottie/player-component@latest/dist/dotlottie-player.mjs";

    function init() {
        var slot = document.getElementById("errorMediaSlot");
        if (!slot) return;

        var code = slot.dataset.code || "default";
        var cfg = ERROR_MEDIA[code] || ERROR_MEDIA["default"];

        if (cfg.hideCode) {
            document.body.classList.add("hide-error-code");
        }
        if (cfg.size) {
            slot.classList.add("size-" + cfg.size);
        }

        switch (cfg.type) {
            case "video": renderVideo(slot, cfg); break;
            case "dotlottie": renderDotLottie(slot, cfg); break;
            case "image": renderImage(slot, cfg); break;
            case "lottie":
            default: renderLottie(slot, cfg); break;
        }
    }

    function renderLottie(slot, cfg) {
        if (customElements.get("lottie-player")) {
            insertLottieTag(slot, cfg);
            return;
        }
        loadScript(LOTTIE_CDN,
            function () { insertLottieTag(slot, cfg); },
            function () { fallbackImage(slot, cfg); });
    }

    function insertLottieTag(slot, cfg) {
        var el = document.createElement("lottie-player");
        el.className = "error-anim";
        el.setAttribute("src", cfg.src);
        el.setAttribute("background", "transparent");
        el.setAttribute("speed", "1");
        el.setAttribute("loop", "");
        el.setAttribute("autoplay", "");
        el.addEventListener("error", function () { fallbackImage(slot, cfg); });
        slot.replaceChildren(el);
    }

    function renderDotLottie(slot, cfg) {
        if (customElements.get("dotlottie-player")) {
            insertDotLottieTag(slot, cfg);
            return;
        }
        var s = document.createElement("script");
        s.type = "module";
        s.src = DOTLOTTIE_CDN;
        s.onload = function () { insertDotLottieTag(slot, cfg); };
        s.onerror = function () { fallbackImage(slot, cfg); };
        document.head.appendChild(s);
    }

    function insertDotLottieTag(slot, cfg) {
        var el = document.createElement("dotlottie-player");
        el.className = "error-anim";
        el.setAttribute("src", cfg.src);
        el.setAttribute("background", "transparent");
        el.setAttribute("speed", "1");
        el.setAttribute("loop", "");
        el.setAttribute("autoplay", "");
        el.addEventListener("error", function () { fallbackImage(slot, cfg); });
        slot.replaceChildren(el);
    }

    function renderImage(slot, cfg) {
        var img = document.createElement("img");
        img.className = "error-anim";
        img.alt = "Error";
        img.src = cfg.src;
        img.onerror = function () { fallbackImage(slot, cfg); };
        slot.replaceChildren(img);
    }
    function renderVideo(slot, cfg) {
        var video = document.createElement("video");
        video.className = "error-anim";
        video.autoplay = true;
        video.muted = true;
        video.loop = true;
        video.playsInline = true;
        video.preload = "metadata";
        video.setAttribute("playsinline", "");
        video.setAttribute("muted", "");
        if (cfg.poster) video.poster = cfg.poster;
        video.setAttribute("aria-label", "Error animation");

        var source = document.createElement("source");
        source.src = cfg.src;
        source.type = "video/mp4";
        video.appendChild(source);

        var fallback = document.createElement("img");
        fallback.className = "error-anim";
        fallback.alt = "Error";
        if (cfg.poster) fallback.src = cfg.poster;
        video.appendChild(fallback);

        video.addEventListener("error", function () { fallbackImage(slot, cfg); }, true);
        slot.replaceChildren(video);

        if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
            video.autoplay = false;
            video.removeAttribute("autoplay");
            video.pause();
        } else {
            var p = video.play();
            if (p && typeof p.catch === "function") p.catch(function () { });
        }
    }

    function fallbackImage(slot, cfg) {
        var img = document.createElement("img");
        img.className = "error-anim";
        img.alt = "Error";
        img.src = cfg.poster || FALLBACK_IMG;
        slot.replaceChildren(img);
    }

    function loadScript(src, onload, onerror) {
        var s = document.createElement("script");
        s.src = src;
        s.onload = onload;
        s.onerror = onerror;
        document.head.appendChild(s);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();