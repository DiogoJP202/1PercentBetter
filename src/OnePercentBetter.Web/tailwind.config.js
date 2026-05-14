/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Views/**/*.cshtml',
    './Areas/**/*.cshtml',
    './wwwroot/js/**/*.js'
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        surface: '#0b1120',
        panel: '#111827',
        line: 'rgba(148, 163, 184, 0.18)'
      }
    }
  },
  plugins: []
};
