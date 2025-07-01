# DotnetPracticeAPI

A production-ready Web API built with ASP.NET Core 8.0, using PostgreSQL and Redis, containerized with Docker and deployed on Render.

## 🚀 Features

- ✅ ASP.NET Core 8.0 Web API
- ✅ PostgreSQL(Prod) and SQL Server(Dev) with Entity Framework Core
- ✅ Redis (Upstash) for caching
- ✅ Hangfire for background jobs + protected dashboard
- ✅ Identity with JWT + Cookie authentication
- ✅ Hub service with WebSocket using SignalR
- ✅ CQRS with MediatR and FluentValidation
- ✅ Health checks, Rate Limiting and logging (Serilog)
- ✅ Dockerfile for production-ready deployment
- ✅ Graceful fallback when Redis is unavailable
- ✅ Secure configuration using environment variables
- ✅ Clean Architecture and SOLID principles
- ✅ Testing with xUnit, Benchmark and Stress Tests

## 📦 Tech Stack

- **Backend:** ASP.NET Core 8.0, Entity Framework Core, MediatR
- **Auth:** Identity, JWT, Cookie Auth
- **Database (Production):** PostgreSQL (via Neon)
- **Database (Development):** SQL Server
- **Cache:** Redis (via Upstash) or local Redis using Docker
- **Background Jobs:** Hangfire
- **Logging:** Serilog
- **Containerization:** Docker
- **Deployment:** Render

## 🔐 Authentication

- Default scheme: JWT Bearer
- Hangfire dashboard: Cookie-based login with protected admin access

## 📥 Local Development

```bash
# Build and run services
docker-compose up --build
