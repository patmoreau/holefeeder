# Holefeeder

[![Continuous Integration](https://github.com/patmoreau/Holefeeder/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/patmoreau/Holefeeder/actions/workflows/ci-cd.yml)
[![CodeQL](https://github.com/patmoreau/Holefeeder/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/patmoreau/Holefeeder/actions/workflows/codeql-analysis.yml)
[![Linter](https://github.com/patmoreau/Holefeeder/actions/workflows/linter.yml/badge.svg)](https://github.com/patmoreau/Holefeeder/actions/workflows/linter.yml)
![Test Results](https://gist.githubusercontent.com/patmoreau/230a77d0ee69381bbe869c0fc62f9cc6/raw/holefeeder-tests-badge.svg)

## Introduction

The main purpose of this repository is to showcase what I can do. This is also a constant work in progress to learn new designs, architecture, technologies where my main goal is to be the most efficient in my craft and offering the best/simple solutions for the job.

Here are the key concepts which drive my thinking today:

1. [Test-Driven Development](https://martinfowler.com/bliki/TestDrivenDevelopment.html) — red, green, refactor, one small test at a time.
1. [Tidy First](https://tidyfirst.substack.com/) — structural and behavioral changes never share a commit.
1. [Testing Without Mocks](https://www.jamesshore.com/v2/projects/nullables/testing-without-mocks) — sociable tests against real collaborators and a real test database.
1. [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) — a simplified, feature-sliced take described in [architecture principles](docs/contributing/architecture-principles.md).
1. [Offline-first sync](https://www.powersync.com/) — the mobile app works on a local database kept in sync by PowerSync.

## Repository structure

This is a monorepo containing both the backend and the clients:

- **`backend/`** — the .NET solution (API, domain, application, infrastructure), tests, and Docker Compose orchestration. See [`backend/AGENTS.md`](backend/AGENTS.md).
- **`frontend/`** — the pnpm + Turborepo workspace: Expo/React Native mobile app, React web app, and shared packages. See [`frontend/AGENTS.md`](frontend/AGENTS.md).
- **`docs/business-rules/`** — the canonical, language-agnostic business rules shared by the backend and the clients.
- **`docs/contributing/`** — engineering principles, git workflow and task playbooks. Start with [`AGENTS.md`](AGENTS.md).

## Requirements

You will need the following:

1. [Docker](https://www.docker.com/) or [Podman](https://podman.io/)
1. [.NET 10](https://dotnet.microsoft.com/en-us/download)
1. [Node.js](https://nodejs.org/) and [pnpm](https://pnpm.io/) 11, for `frontend/`
1. [Xcode](https://developer.apple.com/xcode/), for the iOS mobile app

## Getting ready

### Start the local stack

```bash
cd backend
cp .env.template .env
docker compose --profile local up -d
```

Fill in the blank values in `.env` (database passwords, PowerSync token, Docker socket) before starting. This starts Traefik, PostgreSQL, PowerSync, the API and the web app on `*.localtest.me`.

### Restore the .NET tools

```bash
cd backend
dotnet tool restore
```

### Generate a code coverage report

Run the `holefeeder code coverage` task in VS Code, or the commands under *Code Coverage* in [`backend/AGENTS.md`](backend/AGENTS.md).
