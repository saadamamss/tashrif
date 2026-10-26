// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-05-15",
  devtools: { enabled: false },
  runtimeConfig: {
    public: {
      apiBaseUrl: process.env.NUXT_PUBLIC_API_BASE_URL || "/api",
      appName: process.env.NUXT_PUBLIC_APP_NAME || "منصة تشريف",
      appUrl: process.env.NUXT_PUBLIC_APP_URL || "http://localhost:3000",
    },
  },
  app: {
    head: {
      htmlAttrs: { lang: "ar", dir: "rtl" },
      charset: "utf-8",
      viewport: "width=device-width, initial-scale=1",
      link: [{ rel: "icon", type: "image/x-icon", href: "/favicon.ico" }],
      meta: [
        { name: "theme-color", content: "#ECB42B" },
      ],
    },
    layoutTransition: { name: "layout", mode: "out-in" },
    pageTransition: { name: "page", mode: "out-in" },
  },
  components: {
    dirs: [
      { path: '~/components/icons', prefix: '' },
      '~/components',
    ],
  },
  modules: [],
  css: ["~/assets/scss/main.scss"],
  postcss: {
    plugins: {
      tailwindcss: {},
      autoprefixer: {},
    },
  },
  plugins: ["~/plugins/vee-validate.js", "~/plugins/ripple.client.js", "~/plugins/axios.ts", "~/plugins/auth.server.ts"],

  vite: {
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: `
            @use "@/assets/scss/abstracts/variables" as *;
            @use "@/assets/scss/abstracts/mixins" as *;
          `,
        },
      },
    },
    resolve: {
      alias: {
        'form-data': 'form-data/lib/form_data.js',
      },
    },
  },
});
