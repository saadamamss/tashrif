/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./components/**/*.{js,vue,ts}",
    "./layouts/**/*.vue",
    "./pages/**/*.vue",
    "./plugins/**/*.{js,ts}",
    "./app.vue",
    "./error.vue",
  ],
  theme: {
    extend: {
      screens: {
        xs: "480px",
      },
      colors: {
        primary: {
          DEFAULT: '#ECB42B',
          50: '#FFF5E8',
          100: '#FFDA85',
        },
        muted: '#667178',
        dark: '#161614',
        surface: '#25343E',
        danger: '#F53D6B',
        'icon-muted': '#696C68',
        'border-muted': '#929A9F',
        'bg-subtle': '#F8F9F9',
        'bg-light': '#F5F5F5',
        success: '#1E874C',
        'badge-green': '#35685F',
      },
    },
  },
  plugins: [],
};
