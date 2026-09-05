# 🏢 Meeting Room Booking API

An educational backend project focused on solid engineering practices around a deceptively simple domain: booking meeting rooms. The core idea — bookings must never overlap, even under concurrent load — is used as an excuse to explore authentication, race-condition handling, rate limiting, structured logging, and clean architecture in a real ASP.NET Core codebase.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C%23](https://img.shields.io/badge/C%23-95.7%25-239120?style=for-the-badge&logo=csharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Npgsql-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-black?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Tests](https://img.shields.io/badge/tests-xUnit%20%7C%20NSubstitute%20%7C%20Moq-6E4C13?style=for-the-badge&logo=testinglibrary&logoColor=white)

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Frontend](#-frontend)

## 🔍 Overview

This project models a room-booking system for an organization: users register and log in, browse rooms, and create bookings for a room during a given time slot. On the surface that's a CRUD app — the interesting part is everything built around it to make it behave correctly and predictably under real-world conditions: concurrent booking attempts on the same room, duplicate account registrations, abusive clients, and unhandled failures.

## ✨ Features

### 🏗️ Architecture
- Clean, layered architecture: `Core` (domain models, abstractions, Result pattern, domain errors) → `Application` (services, DTOs, validation, business rules) → `DataAccess` (EF Core + PostgreSQL, repositories, migrations) → `Infrastructure` (hashing, JWT) → `API` (controllers, middleware)
- Result pattern used throughout the service layer instead of exceptions for expected failure cases
- Centralized error handling via `IExceptionHandler` implementations (`GlobalExceptionHandler`, `ValidationExceptionHandler`) returning consistent `ProblemDetails` responses, each tagged with a request ID for traceability

### 🔐 Authentication & Security
- JWT-based authentication, with the token stored in an `HttpOnly`, `Secure`, `SameSite=Strict` cookie rather than exposed to client-side JS
- Password hashing for user credentials
- Login / logout flow with cookie-based session handling

### ⚡ Concurrency & Data Integrity
- Advisory locking (by room + date) around booking creation to prevent two overlapping bookings on the same room from both succeeding under concurrent requests
- A unique index on user email to eliminate the race window between an application-level uniqueness check and the actual insert during registration

### 🚦 Rate Limiting
- Multiple rate limiting strategies implemented and wired up via ASP.NET Core's `RateLimiter` middleware:
  - Fixed window
  - Sliding window
  - Token bucket
  - Concurrency limiter
  - A per-authenticated-user fixed window policy (as opposed to per-IP)
- Rejected requests return a proper `429` with a `Retry-After` header and a `ProblemDetails` body instead of a bare status code

### 📊 Observability
- Structured logging via Serilog, including a custom request-logging template (`HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms`) with log level automatically escalated to `Error` on exceptions or 5xx responses

### ✅ Validation
- FluentValidation used across all write operations (users, rooms, bookings, nested value objects like address info), decoupled from the domain models

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core (.NET 10) |
| Database | PostgreSQL via Npgsql / EF Core |
| Auth | JWT (cookie-based) |
| Validation | FluentValidation |
| Logging | Serilog |
| Testing | xUnit, NSubstitute, Moq |
| Frontend (WIP) | Next.js, Ant Design |

## 📁 Project Structure

```
backend/RoomBooking.API/
├── RoomBooking.API/              # Controllers, middleware, composition root (Program.cs)
├── RoomBooking.Application/      # Services, DTOs, validators, business logic
├── RoomBooking.Core/             # Domain models, abstractions, Result pattern, errors
├── RoomBooking.DataAccess/       # EF Core DbContext, repositories, migrations
├── RoomBooking.Infrastructure/   # Password hashing, JWT provider
├── RoomBooking.Application.Tests/       # Unit tests
└── RoomBooking.Application.Tests.DI/    # Unit tests (DI-focused)

frontend/room-booking/            # Next.js app (work in progress)
```

## 🚀 Getting Started

**Prerequisites**
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A running PostgreSQL instance

**Setup**
1. Clone the repository
2. Configure your PostgreSQL connection string and JWT settings in `appsettings.json` / `appsettings.Development.json`
3. Run the API:
   ```
   dotnet run --project backend/RoomBooking.API/RoomBooking.API
   ```
   or open the solution in Rider / Visual Studio and hit **F5** ▶️

The database is initialized (migrated/seeded) automatically on startup via `DbInitializer`. 🌱

## 📖 API Documentation

Swagger UI is available automatically in the Development environment at `/swagger` once the API is running. 🔎

## 🧪 Testing

Unit tests cover the application/service layer using **xUnit**, with **NSubstitute** and **Moq** for mocking dependencies. Run them with:

```
dotnet test
```

## 🖥️ Frontend

A Next.js + Ant Design frontend lives under `frontend/room-booking`. It's a work in progress and not the focus of this project — some pages (e.g. room listing/interaction) exist, but the frontend isn't fully functional yet 🚧. The API itself is fully usable standalone via Swagger.
