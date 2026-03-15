# Tashreef Platform 🕋

Tashreef is a smart digital platform specialized in seasonal recruitment for Hajj and Umrah seasons. The platform aims to bridge the gap between qualified individuals and operating entities, ensuring high standards of quality and efficiency in serving the guests of Allah.

## 🚀 Tech Stack

- **Framework**: [Nuxt 3](https://nuxt.com/) (Vue 3 Composition API)
- **State Management**: [Pinia](https://pinia.vuejs.org/)
- **Styling**: 
  - [Tailwind CSS](https://tailwindcss.com/) (Utility-first layout & spacing)
  - [SCSS](https://sass-lang.com/) (Complex component logic & global variables)
- **Forms & Validation**: [Vee-Validate](https://vee-validate.logaretm.com/v4/)
- **Animations**: Swiper.js & Vue Built-in Transitions
- **Icons**: Custom-built SVG Components

## ✨ Key Features

- **Advanced ScrollSpy System**: A custom-built navigation system using the `Intersection Observer API` for high-performance section tracking and smooth cross-page navigation.
- **Custom UI Kit**: Hand-crafted UI components (Selects, Inputs, Dialogs, Tabs) built from scratch to ensure full design control and optimized performance.
- **Native RTL Support**: Fully optimized for Arabic language layouts and Middle Eastern design standards.
- **Atomic Component Architecture**: The homepage and dashboard are broken down into small, independent, and maintainable components.
- **Smart Route Middleware**: Robust access control management based on authentication state and user roles (Individual vs. Entity).
- **Clean URL Navigation**: Intelligent routing that handles section scrolling across different pages while maintaining clean, hash-free URLs.

## 📁 Project Structure

```text
├── assets/             # Static assets (SCSS, Fonts, Images)
├── components/         # Vue components (Elements, Home, Dashboard)
├── composables/        # Reusable business logic & state
├── layouts/            # Page layouts (Default, Dashboard, Login)
├── middleware/         # Route guards and permission logic
├── pages/              # Application views (File-based routing)
├── stores/             # Global state management (Auth, User)
└── nuxt.config.ts      # Nuxt configuration