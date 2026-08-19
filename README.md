# Tashreef Platform 🕋

A smart digital platform specialized in seasonal recruitment for Hajj and Umrah seasons. Bridges the gap between qualified individuals and operating entities.

## Quick Start

```bash
cp .env.example .env
npm install
npm run dev
```

Open http://localhost:3000

## Test Credentials

| Role | National ID | Password |
|------|-------------|----------|
| Individual | `1010101010` | `individual` |
| Entity | `2020202020` | `entity` |

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | [Nuxt 3](https://nuxt.com/) (Vue 3, Composition API) |
| State | `useAuth` composable (useState-based) |
| Styling | Tailwind CSS + SCSS |
| Forms | Vee-Validate 4 |
| Icons | Custom SVG components (57 icons) |
| Testing | Vitest (unit) + Playwright (E2E) |
| Types | TypeScript + JSDoc |

## Project Structure

```
frontend/
├── assets/              # SCSS, images, fonts
│   └── scss/
│       ├── abstracts/   # Variables, mixins
│       └── components/  # Component SCSS
├── components/
│   ├── elements/        # Reusable UI (Select, Tabs, DropDown, Pagination)
│   ├── icons/           # 57 SVG icon components
│   ├── dashboard/       # Dashboard-specific components
│   └── ...              # Page-level components
├── composables/         # useApi, useAuth, usePagination, useFormPersistence, etc.
├── e2e/                 # Playwright E2E tests (login, registration, job search)
├── layouts/             # default, dashboard, login-layout
├── middleware/          # auth, user-type, individual, entity
├── pages/               # File-based routing (17 pages)
│   ├── dashboard/
│   │   ├── jobs-explore/    # Individual: browse & apply
│   │   ├── job-requests/    # Individual: my applications
│   │   ├── published-jobs/  # Entity: manage listings
│   │   ├── publish-job/     # Entity: create new job
│   │   ├── interviews/      # Both: manage interviews
│   │   ├── employment-contracts/ # Both: manage contracts
│   │   └── profile/         # Both: user profile
│   └── register/         # individual, entity, success
├── plugins/             # axios (401 queue), auth.server, Vee-Validate, Ripple
├── public/              # Static files (favicon, robots.txt)
├── server/
│   └── api/             # Mock API routes (auth, jobs, applications, etc.)
├── types/               # TypeScript interfaces (auth, job, application, etc.)
├── __tests__/           # Vitest unit tests (9 files, 38 tests)
├── playwright.config.ts
├── vitest.config.ts
└── nuxt.config.ts
```

## Architecture

### API Layer
- `plugins/axios.ts` — creates the `$api` axios instance (`withCredentials: true`), forwards SSR cookies, handles 401 refresh queue
- `composables/useApi.ts` — thin `{ data, error, pending }` wrapper over `$api`
- `server/api/` — Mock server routes with in-memory data stores, realistic delays (200-500ms), proper HTTP status codes
- Auth token flow: login → backend sets `access_token`/`refresh_token` HttpOnly cookies → browser sends them automatically

### Auth Flow
- `composables/useAuth.ts` — useState-based auth state: user, isAuthenticated, userType, login/register/logout/init
- `plugins/auth.server.ts` — restores session on SSR via `useAuth().init()` → `GET /auth/me`
- `middleware/auth.js` — single guard: redirects to `/?redirect=<path>` if not authenticated and `meta.requiresAuth` (opens login modal, returns user to the page), redirects to `/dashboard` if authenticated and `meta.guest`
- `middleware/user-type.js` — routes individuals vs entities to their respective dashboards
- `middleware/individual.js` / `entity.js` — role gates

### Data Flow
- Pages use `usePagination` composable for page state + URL sync
- List pages always render 3 states: LoadingSkeleton / ErrorState + retry / EmptyState
- Multi-step entity registration form persists to sessionStorage (survives refresh)

## Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Start dev server (http://localhost:3000) |
| `npm run build` | Production build |
| `npm run generate` | Static site generation |
| `npm run preview` | Preview production build |
| `npm test` | Run unit tests (Vitest, 38 tests) |
| `npm run test:e2e` | Run E2E tests (Playwright, 12 tests) |
| `npx nuxi typecheck` | TypeScript check |

## Testing

### Unit Tests (Vitest)
```bash
npm test
# 9 test files, 38 tests, ~2s
```
Covers: useAuth, useApi, middleware guards, LoginDialog, JobCard, ApplyJobDialog.

### E2E Tests (Playwright)
```bash
npm run test:e2e
# 3 test files, 12 tests
```
Covers: login flow (individual + entity), registration navigation, job browsing.

Playwright config auto-starts `npm run dev` — no manual server setup needed.

## Features

- **RTL support** — `lang="ar" dir="rtl"` throughout
- **Role-based dashboards** — Individuals browse/apply, Entities publish/manage
- **3-state UI** — Loading skeletons, empty states with CTAs, error states with retry
- **Pagination** — URL-synced page state, configurable page size
- **Form persistence** — Multi-step forms survive page refresh
- **Accessibility** — ARIA labels, landmark roles, skip link, aria-expanded toggles
- **SEO** — Per-page titles + meta descriptions, title templates, OG tags, Twitter card
- **TypeScript** — Full JSDoc typing on all composables/components
- **Custom icons** — 57 SVG components with `aria-hidden="true"`
- **Swiper carousel** — Home page job slider

## User Flows

### Individual
1. Browse public jobs → Click "قدم الآن" → Login modal
2. Login → Dashboard → Explore jobs → Filter → Apply
3. Track applications in "طلبات العمل"
4. Receive interview invitations → Accept/schedule
5. Sign contract digitally

### Entity
1. Register entity → 3-step form (basic info, social links, contact)
2. Login → Dashboard → Publish job
3. Review applicants → Shortlist → Schedule interviews
4. Send contracts → Track acceptance

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `NUXT_PUBLIC_API_BASE_URL` | `/api` | API base URL |
| `NUXT_PUBLIC_APP_NAME` | `منصة تشريف` | Application name |
| `NUXT_PUBLIC_APP_URL` | `http://localhost:3000` | Public URL |

Copy `.env.example` to `.env` and customize.
