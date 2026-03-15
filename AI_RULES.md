# AI Development Rules for Tashreef Platform

## Tech Stack
- **Framework**: Nuxt 3 (Vue 3) with Composition API.
- **State Management**: Pinia for global state (Auth, User Profile).
- **Styling**: Tailwind CSS for utility-first layout and SCSS for structured component styling and global variables.
- **Form Validation**: Vee-Validate with custom rules and localized messages.
- **Icons**: Custom SVG components located in `components/icons/`.
- **Components**: Custom-built UI components following a consistent design language.
- **Animations**: Vue built-in Transitions and Swiper.js for carousels/sliders.
- **Internationalization**: Native RTL (Right-to-Left) support for Arabic.

## Development Rules

### 1. Component Architecture
- **Atomic Design**: Keep components small and focused.
- **Icons**: Do NOT use external icon libraries. Use the existing SVG components in `components/icons/`. If a new icon is needed, create a new `.vue` file in that directory.
- **UI Elements**: Use existing elements in `components/elements/` (e.g., `CustomSelect`, `TextInput`, `ArrowButton`) to maintain consistency.

### 2. Styling Guidelines
- **Tailwind First**: Use Tailwind classes for layout, spacing, and simple styling.
- **SCSS for Complexity**: Use SCSS in `<style lang="scss">` blocks for complex animations, pseudo-elements, or when leveraging global variables from `assets/scss/abstracts/_variables.scss`.
- **RTL Support**: Always ensure designs work in RTL. Use logical properties (e.g., `ps-` instead of `pl-`) where applicable.

### 3. State & Logic
- **Pinia Stores**: Use Pinia for persistent data like authentication (`authStore.js`) and user metadata (`userStore.js`).
- **Composables**: Encapsulate reusable logic (like scroll spying or modal management) in the `composables/` directory.
- **Middleware**: Use route middleware for access control (e.g., `auth-guard.js`, `individual.js`, `entity.js`).

### 4. Forms & Validation
- **Vee-Validate**: All forms must use `Vee-Validate` (`<Form>`, `<Field>`, `<ErrorMessage>`).
- **Custom Inputs**: Wrap custom input logic (like `PhoneInput`) to work seamlessly with Vee-Validate's `v-model`.

### 5. File Naming
- **Components**: Use kebab-case for file names (e.g., `job-card.vue`).
- **Pages**: Follow Nuxt's directory-based routing conventions.

### 6. Best Practices
- **No Shell Commands**: Never suggest running npm/shell commands to the user.
- **Complete Files**: Always provide the full content of the file when using `<dyad-write>`.
- **Simple & Elegant**: Avoid overengineering; focus on the specific request using existing patterns.