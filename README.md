# Solution Overview: Microservices & Event-driven Architecture

This solution demonstrates a modular structure for building microservices using .NET 9, with a focus on event-driven communication and shared abstractions. Below is a summary of the main class libraries and their features.

---

## Shared Libraries

### Shared.Application

- **Purpose:** Provides cross-cutting application services and infrastructure for microservices.
- **Key Features:**
  - **Dependency Injection Extensions:** Simplifies registration of services, middleware, and event handlers.
  - **Event Bus Integration:** Registers and configures RabbitMQ-based event bus for publishing and consuming domain events.
  - **Event Handler Discovery:** Scans assemblies and registers all implementations of `IEventHandler<T>`.
  - **Exception Handling:** Adds custom exception handler middleware for consistent error responses.
  - **Startup Utilities:** Automates event handler subscription at application startup.
  - **Validation & Behaviors:** Integrates FluentValidation and pipeline behaviors for request validation and logging.

---

## Service APIs

### Catalog.Public.API

- **Purpose:** Exposes public endpoints for catalog operations.
- **Key Features:**
  - **Mediator Pattern:** Uses Mediator for CQRS-style request handling.
  - **Validation:** Integrates FluentValidation for DTO validation.
  - **Redis Caching:** Adds distributed caching via Redis.
  - **Event-driven:** Publishes and consumes events using the shared event bus.
  - **API Documentation:** Includes OpenAPI/Swagger support.
  - **Carter Integration:** Simplifies endpoint routing and modularization.

### Catalog.Admin.API

- **Purpose:** Provides administrative endpoints for catalog management.
- **Key Features:**
  - **Entity Framework Core:** Supports data access and migrations.
  - **Mediator Pattern:** Handles commands and queries using Mediator.
  - **Event-driven:** Integrates with the shared event bus for publishing/consuming events.
  - **API Documentation:** OpenAPI/Swagger enabled.
  - **Carter Integration:** Modular endpoint routing.

### Cart.API

- **Purpose:** Manages shopping cart operations.
- **Key Features:**
  - **Redis Caching:** Uses Redis for fast cart data storage.
  - **Mediator Pattern:** Implements CQRS with Mediator.
  - **Event-driven:** Publishes and consumes events via the shared event bus.
  - **API Documentation:** OpenAPI/Swagger support.
  - **Carter Integration:** Modular endpoint routing.
  - **Custom Exception Handling:** Consistent error responses.

---

## Event-driven Architecture

- **Event Bus:** All services use a shared RabbitMQ-based event bus for asynchronous communication.
- **Event Handlers:** Services auto-discover and register event handlers for domain events.
- **Loose Coupling:** Services communicate via events, reducing direct dependencies.

---

## How to Extend

- Add new microservices by referencing `Shared.Application` for event-driven and cross-cutting features.
- Implement new event handlers by following the `IEventHandler<T>` interface and registering them via DI.
- Use the provided DI extensions for consistent setup across services.

---

## Technologies Used

- .NET 9
- RabbitMQ (Event Bus)
- Redis (Caching)
- Carter (Minimal API Routing)
- Mediator (CQRS)
- FluentValidation
- Entity Framework Core (Admin API)
- OpenAPI/Swagger
- Docker Compose

---

This structure enables scalable, maintainable, and loosely coupled microservices with a strong focus on event-driven patterns and shared infrastructure.
