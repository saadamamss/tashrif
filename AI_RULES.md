# Tashrif (تشريف) - Senior Frontend Engineering Rules

> Seasonal employment platform for Hajj/Umrah connecting companies (entities) with job seekers (individuals). Nuxt 3, Vue 3 Composition API, Tailwind CSS + SCSS, VeeValidate, Arabic RTL.

---

## 1. PROJECT ARCHITECTURE

```
frontend/
├── assets/scss/          # Global styles, variables, mixins
│   ├── abstracts/        # _variables.scss, _mixins.scss
│   ├── base/             # _global.scss (buttons, wrappers, utilities)
│   └── main.scss         # Root stylesheet
├── components/
│   ├── dashboard/        # company/ and individual/ dashboard components
│   ├── elements/         # Reusable UI primitives (CustomSelect, TextInput, FileInput, ArrowButton, Tabs)
│   └── icons/            # SVG icon components (one per file)
├── composables/          # Shared reactive logic (useAuth, useApi, useLoginModal, useScrollSpy)
├── layouts/              # default (public), dashboard (auth), login-layout
├── middleware/           # Route guards (auth, individual, entity, user-type)
├── pages/                # File-based routing
│   ├── dashboard/        # Authenticated pages (job management, interviews, contracts, profile)
│   └── register/         # Individual and entity registration flows
├── plugins/              # axios (401 queue), auth.server, VeeValidate rules, ripple directive
└── server/               # Nuxt server routes (API layer)
```

### Two User Types = Two Dashboard Experiences
- **Individual (فرد)**: explore jobs, apply, track requests, attend interviews, sign contracts
- **Entity (جهة)**: publish jobs, manage applicants, shortlist, interview, send contracts

Dashboard pages use `defineAsyncComponent` to load the correct component based on `useAuth().userType`.

---

## 2. CODE CONVENTIONS

### File Naming
- Components: **kebab-case** → `job-card.vue`, `filter-drawer.vue`, `apply-job-dialog.vue`
- Pages: follow Nuxt file-based routing conventions
- Composables: **camelCase** with `use` prefix → `useAuth.ts`, `useLoginModal.js`
- SCSS partials: **underscore prefix** → `_variables.scss`, `_mixins.scss`

### Vue SFC Order
Always follow this order inside `.vue` files:
```vue
<script setup>
// 1. imports
// 2. props / emits
// 3. reactive state (ref, reactive, computed)
// 4. composables
// 5. methods
// 6. lifecycle hooks (onMounted, watch, etc.)
</script>

<template>
  <!-- HTML -->
</template>

<style lang="scss" scoped>
/* styles */
</style>
```

### Script Setup Only
Always use `<script setup>` — never Options API, never `<script>` with `setup()` function.

### Template Rules
- Use `v-for` with `:key` always — never omit the key
- Prefer `<component :is="...">` for dynamic rendering over `v-if` chains
- Max 3 levels of nesting in template — extract sub-components if deeper
- Self-close components with no children: `<JobCard />` not `<JobCard></JobCard>`
- Use `@click` not `v-on:click`, use `:class` not `v-bind:class`

---

## 3. STYLING RULES

### Tailwind First, SCSS for Complexity
```vue
<!-- YES: Tailwind for layout, spacing, typography -->
<div class="flex items-center gap-4 p-6 rounded-xl bg-white">

<!-- YES: SCSS for complex animations, pseudo-elements, or using design tokens -->
<style lang="scss" scoped>
.card {
  &::after {
    content: '';
    background: $primary;
  }
}
</style>
```

### Design Tokens — Use Tailwind Config, Not Raw Hex
NEVER hardcode colors in templates. All colors must be in `tailwind.config.js`:
```js
// tailwind.config.js
theme: {
  extend: {
    colors: {
      primary: '#ECB42B',
      'primary-dark': '#d4a127',
      dark: '#161614',
      'dark-secondary': '#25343E',
      muted: '#667178',
      danger: '#F53D6B',
      success: '#22C55E',
      surface: '#F8F9FA',
      border: '#E5E7EB',
    }
  }
}
```

Then use: `text-primary`, `bg-dark`, `border-border`, `text-muted` — NOT `text-[#ECB42B]` or `style="color: #667178"`.

### SCSS Variables Available Globally
These are auto-injected by Nuxt config into every component:
- `$primary` — brand gold (#ECB42B)
- Select menu tokens from `_variables.scss`
- Mixins from `_mixins.scss`

### RTL Support
- Use logical properties: `ps-4` (padding-start) not `pl-4`, `me-2` not `mr-2`
- Use `gap` instead of directional margins between flex children
- Always test layout in RTL — `dir="rtl"` is set globally on `<html>`
- For absolute positioning, use `start-0` / `end-0` not `left-0` / `right-0`

### Responsive Breakpoints
```
xs: 480px (custom)
sm: 640px
md: 768px
lg: 1024px
xl: 1280px
```
Max content wrapper: `max-w-[1312px] mx-auto px-4` (class `.max-wrapper` exists in global CSS).

---

## 4. COMPONENT PATTERNS

### Icons — NEVER Use Inline SVG
All icons live in `components/icons/`. NEVER paste raw `<svg>` markup into templates.

```vue
<!-- WRONG — bloats template with 50+ lines of SVG -->
<svg xmlns="..." viewBox="..." class="w-5 h-5">
  <path d="M12 2C6.48..." />
</svg>

<!-- RIGHT — use icon component -->
<icons-edit class="w-5 h-5" />
```

To add a new icon: create `components/icons/new-icon-name.vue` with the SVG inside.

### Existing UI Elements — Use Them
Before building any form control or UI primitive, check `components/elements/`:
- `CustomSelect` — dropdown select with custom styling
- `TextInput` — text field with label and validation
- `FileInput` — file upload with preview
- `ArrowButton` — CTA button with arrow
- `Tabs` — tab navigation component

### Component Props — Always Define Types
```vue
<script setup>
const props = defineProps({
  title: { type: String, required: true },
  status: { type: String, default: 'pending', validator: v => ['pending', 'active', 'closed'].includes(v) },
  items: { type: Array, default: () => [] },
})

const emit = defineEmits(['update', 'delete'])
</script>
```

### Component Size Rule
If a component's `<template>` exceeds **150 lines**, it MUST be split into sub-components. Identify repeating blocks and extract them.

### Dynamic Dashboard Components
Dashboard pages that differ by user type use this pattern:
```vue
<script setup>
import { useAuth } from '~/composables/useAuth'

const { userType } = useAuth()
const DashboardComponent = computed(() => {
  if (userType.value === 'individual') return defineAsyncComponent(() => import('~/components/dashboard/individual/Home.vue'))
  if (userType.value === 'entity') return defineAsyncComponent(() => import('~/components/dashboard/company/Home.vue'))
  return null
})
</script>

<template>
  <component :is="DashboardComponent" v-if="DashboardComponent" />
</template>
```

---

## 5. STATE MANAGEMENT (COMPOSABLES)

There is **no Pinia**. Shared state lives in composables using Nuxt's `useState` (SSR-safe). Auth state is the canonical example (`composables/useAuth.ts`).

### Composable Pattern
```ts
// composables/useExample.ts
export function useExample() {
  const items = useState<any[]>("example:items", () => [])
  const isLoading = useState<boolean>("example:isLoading", () => false)
  const error = useState<string | null>("example:error", () => null)

  async function fetchItems() {
    isLoading.value = true
    error.value = null
    try {
      const { data } = await useApi().get<any[]>("/items")
      items.value = data
    } catch (err: any) {
      error.value = err?.message || "Failed"
    } finally {
      isLoading.value = false
    }
  }

  return {
    items: readonly(items),
    isLoading: readonly(isLoading),
    error,
    fetchItems,
  }
}
```

### Composable Rules
- Always give `useState` a unique key (namespaced with the feature name)
- Every async action must set `isLoading` before and after
- Every async action must catch errors and set `error`
- Never store derived data — use `computed`
- Return state via `readonly(...)` and expose mutation functions explicitly

### Auth Flow
- `useAuth()` (`composables/useAuth.ts`) — useState-based: `user`, `isAuthenticated`, `userType`, `isIndividual`, `isEntity`, `isAdmin`, `init()`, `login()`, `register()`, `logout()`
- `plugins/auth.server.ts` — restores session on SSR via `useAuth().init()` → `GET /auth/me`
- `auth` middleware is the single guard: checks `meta.requiresAuth` (redirects to `/?redirect=<path>` when logged out) and `meta.guest` (redirects to `/dashboard` when logged in)
- `individual` / `entity` middleware gates role-specific pages
- `user-type` middleware routes to the correct dashboard

---

## 6. API INTEGRATION

### Use `useApi()` for API Calls
```ts
// composables/useApi.ts — thin wrapper over the $api axios instance
const { data, error, pending } = await useApi().get("/jobs", { params })
```

All requests go through the `$api` axios instance created in `plugins/axios.ts`:
- `withCredentials: true` — session cookies are sent automatically
- Request interceptor forwards cookies on SSR
- Response interceptor handles the 401 refresh queue (`POST /auth/refresh`) and rotates cookies

### Server Routes (Mock API)
For features without a real backend, create Nuxt server routes:
```js
// server/api/jobs.get.js
export default defineEventHandler(async (event) => {
  return [
    { id: 1, title: 'مشرف حجاج', company: 'شركة الضيافة', location: 'مكة المكرمة' },
    // ...
  ]
})
```

This keeps mock data OUT of components and demonstrates real data-fetching patterns.

---

## 7. FORM HANDLING (VEEVALIDATE)

### Form Structure
```vue
<script setup>
import { Form, Field, ErrorMessage } from 'vee-validate'

const onSubmit = async (values) => {
  // values is the validated form data
}
</script>

<template>
  <Form @submit="onSubmit" v-slot="{ isSubmitting }">
    <Field name="email" rules="required|email" v-slot="{ field, errorMessage }">
      <TextInput v-bind="field" :error="errorMessage" label="البريد الإلكتروني" />
    </Field>
    <ErrorMessage name="email" class="text-danger text-sm" />

    <button type="submit" :disabled="isSubmitting" class="btn-primary">
      إرسال
    </button>
  </Form>
</template>
```

### Validation Rules
Defined in `plugins/vee-validate.js` with Arabic messages:
- `required` — حقل مطلوب
- `email` — بريد إلكتروني صالح
- `min:N` — minimum length
- `numeric` — numbers only

To add new rules:
```js
defineRule('phone', (value) => {
  if (!value) return true
  if (!/^[0-9+\-\s()]+$/.test(value)) return 'رقم هاتف غير صالح'
  return true
})
```

---

## 8. ROUTING & MIDDLEWARE

### Middleware Order
For protected pages, middleware runs in this order:
1. `auth` — checks `meta.requiresAuth` / `meta.guest` against `useAuth().isAuthenticated`
2. `user-type` — routes to the correct dashboard
3. `individual` OR `entity` — checks user type matches

### Applying Middleware to Pages
```vue
<script setup>
definePageMeta({
  layout: 'dashboard',
  middleware: ['auth', 'individual'],
  meta: { requiresAuth: true },
})
</script>
```

### Middleware Rules
- Always use `navigateTo()` for redirects inside middleware, never `router.push()`
- Return `navigateTo(...)` to redirect, or nothing to allow
- Never return `false` silently — always redirect to a meaningful page
- Never mutate `to.query` directly — it's read-only

---

## 9. ERROR HANDLING & LOADING STATES

### Every Data-Dependent Component Needs 3 States
```vue
<template>
  <!-- Loading -->
  <div v-if="pending" class="animate-pulse">
    <div class="h-6 bg-gray-200 rounded w-3/4 mb-4" />
    <div class="h-4 bg-gray-200 rounded w-1/2" />
  </div>

  <!-- Error -->
  <div v-else-if="error" class="text-center py-12">
    <p class="text-danger mb-4">{{ error.message }}</p>
    <button @click="refresh" class="btn-outline">إعادة المحاولة</button>
  </div>

  <!-- Empty -->
  <div v-else-if="!data?.length" class="text-center py-12">
    <p class="text-muted">لا توجد نتائج</p>
  </div>

  <!-- Data -->
  <div v-else>
    <JobCard v-for="job in data" :key="job.id" :job="job" />
  </div>
</template>
```

NEVER show a blank page while data loads. NEVER silently fail.

---

## 10. ACCESSIBILITY (A11Y)

### Mandatory Rules
```vue
<!-- Icon-only buttons MUST have aria-label -->
<button @click="close" aria-label="إغلاق">
  <icons-close class="w-5 h-5" />
</button>

<!-- Decorative SVGs must be hidden from screen readers -->
<icons-decoration aria-hidden="true" />

<!-- Interactive elements need focus styles -->
<button class="focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2">

<!-- Modals must trap focus and have role -->
<div role="dialog" aria-modal="true" aria-labelledby="dialog-title">
  <h2 id="dialog-title">عنوان النافذة</h2>
</div>

<!-- Toggle buttons need aria-expanded -->
<button @click="isOpen = !isOpen" :aria-expanded="isOpen">
  القائمة
</button>

<!-- Form inputs must have labels -->
<label for="email">البريد</label>
<input id="email" type="email" />
```

---

## 11. PERFORMANCE

### Image Optimization
- Use `<NuxtImg>` or `<NuxtPicture>` instead of raw `<img>` when available
- Always set `width` and `height` on images to prevent layout shift
- Use `loading="lazy"` on below-the-fold images

### Component Lazy Loading
```vue
<!-- For heavy components not needed immediately -->
const HeavyChart = defineAsyncComponent(() => import('~/components/HeavyChart.vue'))

<!-- For route-level code splitting — Nuxt handles this automatically -->
```

### List Rendering
```vue
<!-- For long lists, avoid unnecessary re-renders -->
<JobCard
  v-for="job in jobs"
  :key="job.id"
  :job="job"
/>
<!-- NEVER use index as key when list can be reordered/filtered -->
```

---

## 12. SECURITY

### Never Trust Client Input
- Validate on server side even if client validates too
- Sanitize any user-generated content before rendering with `v-html`
- Prefer `{{ text }}` (auto-escaped) over `v-html`

### Authentication Tokens
- Store tokens in httpOnly cookies — never in localStorage or regular cookies
- Never expose tokens in URLs or query parameters
- Always send tokens via Authorization header, not query params

### Environment Variables
```js
// nuxt.config.ts
export default defineNuxtConfig({
  runtimeConfig: {
    apiSecret: '', // server-only, from NUXT_API_SECRET env var
    public: {
      apiBase: '', // client+server, from NUXT_PUBLIC_API_BASE env var
    }
  }
})
```

Access in code:
```js
const config = useRuntimeConfig()
const apiBase = config.public.apiBase // client-safe
```

---

## 13. TESTING PATTERNS

### Component Tests (Vitest + Vue Test Utils)
```js
import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import JobCard from '~/components/job-card.vue'

describe('JobCard', () => {
  it('renders job title', () => {
    const wrapper = mount(JobCard, {
      props: { job: { id: 1, title: 'مشرف حجاج', company: 'شركة' } }
    })
    expect(wrapper.text()).toContain('مشرف حجاج')
  })
})
```

### Composable Tests (useAuth)
```js
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { useAuth } from '~/composables/useAuth'

// Stub useAuth globals (see __tests__/stores -> composables/useAuth.test.ts)
describe('useAuth', () => {
  it('starts unauthenticated', () => {
    const auth = useAuth()
    expect(auth.isAuthenticated.value).toBe(false)
  })
})
```

See `__tests__/composables/useAuth.test.ts` for the full pattern (stubs `useAuth` globals).

---

## 14. GIT CONVENTIONS

### Commit Messages
```
feat: add job application flow for individuals
fix: mobile menu buttons not emitting correct events
refactor: extract FilterSection from job listing pages
style: move hardcoded colors to Tailwind config
chore: remove unused counter store
```

### Branch Names
```
feat/job-application-flow
fix/mobile-menu-events
refactor/filter-section-component
```

---

## 15. WHAT NOT TO DO

- **NEVER** paste raw SVG markup into templates — create icon components
- **NEVER** hardcode colors as hex values in templates — use Tailwind config
- **NEVER** use `v-for="i in 12"` with static content — fetch from API or mock data
- **NEVER** leave `console.log` in production code
- **NEVER** leave commented-out code — delete it, git has history
- **NEVER** use Options API — always `<script setup>` with Composition API
- **NEVER** put test/debug data in form defaults (like `jobTitle: "abdullah"`)
- **NEVER** return `false` from middleware — always `navigateTo()` to redirect
- **NEVER** store full objects in cookies — store only the token/ID
- **NEVER** use `any` type or skip error handling in async functions
- **NEVER** create a component without props validation when it receives data
- **NEVER** duplicate template blocks across pages — extract shared components
- **NEVER** use `pl-`, `pr-`, `ml-`, `mr-` — use `ps-`, `pe-`, `ms-`, `me-` for RTL
- **NEVER** add an external icon library — use `components/icons/`
- **NEVER** suggest running shell commands to the user

---

## 16. COMMON TASKS REFERENCE

### Adding a New Page
1. Create file in `pages/` following Nuxt routing conventions
2. Set `definePageMeta` with correct layout and middleware
3. Add loading, error, and empty states
4. Use existing UI elements from `components/elements/`

### Adding a New Dashboard Feature
1. Create company and individual variants in `components/dashboard/`
2. Create the page in `pages/dashboard/` with dynamic component loading
3. Apply correct middleware (`auth` + `individual`/`entity`)
4. Add to SideNav navigation items

### Adding a New API Endpoint
1. Create server route in `server/api/`
2. Create or update the relevant composable / `useApi` call
3. Use `useApi` composable for authenticated requests
4. Handle loading/error/empty states in the consuming component

### Creating a Reusable Component
1. Use kebab-case filename in `components/`
2. Define props with types and defaults
3. Define emits explicitly
4. Keep template under 150 lines
5. Use scoped styles with Tailwind + SCSS
6. Add aria attributes for accessibility

### Fixing a Bug
1. Identify the root cause — don't patch symptoms
2. Check if the same bug pattern exists elsewhere
3. Fix all instances
4. Remove any dead code or workarounds left behind
5. Test the fix doesn't break related features

---

## 17. PROJECT-SPECIFIC KNOWLEDGE

### Brand Identity
- Primary color: gold `#ECB42B` (use `primary` token)
- Font: Alexandria (Arabic web font, loaded via CSS)
- Direction: RTL throughout
- Language: Arabic (all UI text)

### Key Business Terms
- **فرد** (fard) = Individual / Job seeker
- **جهة** (jiha) = Entity / Company / Employer
- **ضيوف الرحمن** = Guests of the Most Merciful (pilgrims)
- **موسم الحج** = Hajj season
- **موسم العمرة** = Umrah season
- **تقديم** = Apply / Submit
- **ترشيح** = Shortlist / Nominate
- **مقابلة** = Interview
- **عقد** = Contract

### Hiring Pipeline Flow
```
نشر الوظيفة (Publish Job)
  → استقبال الطلبات (Receive Applications)
    → الترشيح (Shortlist)
      → المقابلات (Interviews)
        → إرسال العقد (Send Contract)
          → قبول (Accept) / رفض (Refuse)
```

### Page → Layout → Middleware Mapping
| Page Group | Layout | Middleware |
|---|---|---|
| `/`, `/jobs` | default | none |
| `/login` | login-layout | auth (guest meta) |
| `/register/*` | login-layout | auth (guest meta) |
| `/dashboard` (individual) | dashboard | auth, individual |
| `/dashboard` (entity) | dashboard | auth, entity |
| `/dashboard` (shared) | dashboard | auth, user-type |
