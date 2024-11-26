/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./**/*.razor",
        "./wwwroot/index.html",
        "./Pages/**/*.cshtml",
        "./Shared/**/*.razor"
    ],
    theme: {
        extend: {},
    },
    plugins: [],
};
