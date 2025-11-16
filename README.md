# Codemy

A microservices-based learning management system built with .NET, following Clean Architecture and Domain-Driven Design principles.

## 🏗️ Architecture

This project follows a microservices architecture pattern, with each service implementing Clean Architecture layers:

### API Gateway
ApiGateway - Entry point that facilitates communication between frontend and backend services

### Building Blocks   
Core shared libraries used across all services:
- **Application** - Application layer abstractions and interfaces
    - Extensions - Useful extension methods
    - Interfaces - Common application interfaces
    - Models - Shared application models and DTOs
- **Core** - Shared domain entities and value objects
- **Domain** - Domain layer base classes and interfaces
- **EventBus** - Event-driven communication infrastructure
- **Infrastructure** - Shared infrastructure implementations

### Services

#### Course Service
Manages course content, curriculum, and course-related operations.
- `Codemy.Services.Course.Api`
- `Codemy.Services.Course.Application`
- `Codemy.Services.Course.Domain`
- `Codemy.Services.Course.Infrastructure`

#### Enrollment Service
Handles student enrollments and course registration.
- `Codemy.Services.Enrollment.Api`
- `Codemy.Services.Enrollment.Application`
- `Codemy.Services.Enrollment.Domain`
- `Codemy.Services.Enrollment.Infrastructure`

#### Identity Service
Manages authentication, authorization, and user identity.
- `Codemy.Services.Identity.Api`
- `Codemy.Services.Identity.Application`
- `Codemy.Services.Identity.Domain`
- `Codemy.Services.Identity.Infrastructure`

#### Notification Service
Handles system notifications.
- `Codemy.Services.Notification.Api`
- `Codemy.Services.Notification.Application`
- `Codemy.Services.Notification.Infrastructure`

#### Payment Service
Processes payments and manages transactions.
- `Codemy.Services.Payment.Api`
- `Codemy.Services.Payment.Application`
- `Codemy.Services.Payment.Domain`
- `Codemy.Services.Payment.Infrastructure`

#### Review Service
Manages course reviews and ratings.
- `Codemy.Services.Review.Api`
- `Codemy.Services.Review.Application`
- `Codemy.Services.Review.Domain`
- `Codemy.Services.Review.Infrastructure`

#### Search Service
Provides search functionality across the platform.
- `Codemy.Services.Search.Api`
- `Codemy.Services.Search.Application`
- `Codemy.Services.Search.Infrastructure`

#### Video Service
Handles video content storage and streaming.
- `Codemy.Services.Video.Api`
- `Codemy.Services.Video.Application`
- `Codemy.Services.Video.Domain`
- `Codemy.Services.Video.Infrastructure`

### API Gateway
- **ApiGateway** - Entry point for all client requests, handles routing and aggregation

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK 
- Visual Studio 2022 or VS Code with C# extension

## 🗄️ Database Migrations

This project uses Entity Framework Core for database management. Each service follows the same pattern for migrations:

```bash
# Add migration
dotnet ef migrations add <MigrationName> -p .\src\Services\<ServiceName>\Infrastructure\Codemy.<ServiceName>.Infrastructure.csproj -s .\src\Services\<ServiceName>\API\Codemy.<ServiceName>.API.csproj

# Update database
dotnet ef database update -p .\src\Services\<ServiceName>\Infrastructure\Codemy.<ServiceName>.Infrastructure.csproj -s .\src\Services\<ServiceName>\API\Codemy.<ServiceName>.API.csproj
```


### Running the Application

Each service needs to be run independently. Navigate to each API project and run:

```bash
# Example: Running the Course Service
cd src/Services/Course/Codemy.Services.Course.Api
dotnet run

# Example: Running the Identity Service
cd src/Services/Identity/Codemy.Services.Identity.Api
dotnet run
```

### Docker
```bash
docker network create codemy-network
docker compose up -d --build
```


## 📦 Project Structure

```
Codemy/
├── src/
│   ├── ApiGateway/
│   ├── BuildingBlocks/
│   │   ├── Application/
│   │   ├── Core/
│   │   ├── Domain/
│   │   ├── EventBus/
│   │   └── Infrastructure/
│   └── Services/
│       ├── Course/
│       ├── Enrollment/
│       ├── Identity/
│       ├── Notification/
│       ├── Payment/
│       ├── Review/
│       ├── Search/
│       └── Video/
└── test/
└── Codemy.sln
```

## 🛠️ Technology Stack

- **.NET 9** - Primary framework
- **Clean Architecture** - Architectural pattern
- **Domain-Driven Design** - Design approach 
