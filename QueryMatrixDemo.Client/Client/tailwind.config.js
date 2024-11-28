/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./**/*.razor",
        "./wwwroot/index.html",
        "./Pages/**/*.cshtml",
        "./Shared/**/*.razor"
    ],
    theme: {
        theme: {
            extend: {
                visibility: {
                    hidden: 'hidden',
                    visible: 'visible',
                },
            },
        },
    },
    corePlugins: {
        // Disable Tailwind's visibility plugin to avoid conflicts
        visibility: false,
        // Disable Tailwind's collapse plugin to let Bootstrap handle it
        collapse: false,
    }
};
