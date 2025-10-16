# Phase 0 – Next.js 14 Monorepo Scaffold

A batteries-included Turborepo that bootstraps a production-ready Next.js 14 (App Router) web application with shared tooling, automated quality gates, and containerized infrastructure for local development.

## 🧱 Project structure

```
.
├── apps
│   └── web/                # Next.js 14 application (App Router + TypeScript)
├── packages
│   ├── eslint-config/      # Shared ESLint presets
│   └── tsconfig/           # Shared TypeScript configurations
├── docker-compose.yml      # Local Postgres + Redis + Mailcatcher services
├── pnpm-workspace.yaml     # Workspace definition
├── turbo.json              # Turborepo pipeline
└── .husky/                 # Git hooks (lint, commit message)
```

## 🚀 Getting started

1. **Install prerequisites**
   - [Node.js](https://nodejs.org/) ≥ 18.17
   - [pnpm](https://pnpm.io/) ≥ 8
   - Docker & Docker Compose

2. **Install dependencies**
   ```bash
   pnpm install
   ```

3. **Configure environment variables**
   ```bash
   cp .env.example .env
   ```

4. **Start backing services**
   ```bash
   docker compose up -d
   ```

5. **Run the web app**
   ```bash
   pnpm --filter @phase/web dev
   ```
   The application is available at **http://localhost:3000**.

## 📦 Workspaces & scripts

Root-level scripts orchestrate commands via Turborepo:

| Command | Description |
| --- | --- |
| `pnpm dev` | Run all workspace apps in development mode (parallel). |
| `pnpm build` | Create a production build for every package/app. |
| `pnpm lint` | Execute linting in every workspace. |
| `pnpm test` | Run unit tests (Jest) in every workspace. |
| `pnpm test:e2e` | Launch Playwright end-to-end test suites. |
| `pnpm format` | Check formatting with Prettier. |
| `pnpm format:write` | Auto-format with Prettier. |

Application-specific scripts can be run with `pnpm --filter`:

| Command | Purpose |
| --- | --- |
| `pnpm --filter @phase/web dev` | Start Next.js dev server. |
| `pnpm --filter @phase/web build` | Build the production bundle. |
| `pnpm --filter @phase/web lint` | Run Next.js + ESLint checks. |
| `pnpm --filter @phase/web test` | Execute Jest unit tests. |
| `pnpm --filter @phase/web test:e2e` | Execute Playwright e2e suite. |
| `pnpm --filter @phase/web typecheck` | Type-check the project with `tsc`. |

## 🧰 Tooling overview

- **Next.js 14 + TypeScript** using the App Router.
- **Turborepo** for orchestrating workspace builds and tasks.
- **Shared TypeScript configs** (`@phase/tsconfig`) and **ESLint presets** (`@phase/eslint-config`).
- **ESLint + Prettier** with auto-fix on commit via `lint-staged` and Husky.
- **Commitlint** enforcing Conventional Commits on `commit-msg` hooks.
- **Testing**: Jest (unit/component) and Playwright (e2e) scaffolding with ready-to-run sample tests.

## 🎨 UI foundations

The global layout shell lives in `apps/web/app/layout.tsx` and ships with:

- `next/font` integration for Montserrat (exposed as the `--font-montserrat` CSS variable).
- A base color palette and spacing tokens defined in `app/globals.css`.
- A hero-style landing page (`app/page.tsx`) highlighting Postgres, Redis, and Mailcatcher services.

## 🔐 Environment variables

`.env.example` documents the required variables for local development:

- `NEXT_PUBLIC_APP_URL` – application URL used on the client.
- `DATABASE_URL`, `POSTGRES_*` – Postgres connection settings.
- `REDIS_URL` – Redis connection string.
- `SMTP_*` – Mailcatcher SMTP host and port for captured email.

Copy the example file and adjust values as necessary before running the app.

## 🐳 Local infrastructure

`docker-compose.yml` provisions the required backing services:

| Service | Image | Ports | Notes |
| --- | --- | --- | --- |
| Postgres | `postgres:15-alpine` | 5432 | Persists data in the `postgres-data` volume. |
| Redis | `redis:7-alpine` | 6379 | Ready for caching, chat, and queue workloads. |
| Mailcatcher | `schickling/mailcatcher` | 1080 (UI), 1025 (SMTP) | Inspect outbound mail during development. |

Bring the stack up with `docker compose up -d` and tear it down using `docker compose down`.

## 🔄 Git hooks

Husky automatically runs after `pnpm install`:

- `pre-commit` – runs `lint-staged` for ESLint and Prettier autofixes.
- `commit-msg` – validates messages with Commitlint (Conventional Commits).

## ✅ Next steps

You are now ready to iterate on application features, add more workspaces, or expand shared packages within the monorepo. Refer to the scripts above for development, testing, and deployment workflows.
