# Routeplanner API Backend

A robust ASP.NET Core Web API for managing locations, routes, and users in a route planning system.
This API is build by students from University of applied science of Rotterdam.

## 🎯 Core Features

- **Location Management:** CRUD operations for locations with detailed information
- **Route Management:** Create and manage routes between locations
- **User Management:** User authentication and authorization
- **Database Integration:** EF Core with PostgreSQL backend

## 🛠️ Technology Stack

- **Framework:** ASP.NET Core
- **Language:** C#
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Object Mapping:** AutoMapper
- **Authentication:** JWT-based authentication

## 📦 Key Dependencies

- `Microsoft.EntityFrameworkCore.Design`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `AutoMapper.Extensions.Microsoft.DependencyInjection`

## 🏗️ Architecture

The project follows Clean Architecture principles with clearly separated layers:

- **API Layer:** ASP.NET Core Web API controllers
- **Service Layer:** Business logic and coordination
- **Data Layer:** EF Core repositories and DbContext
- **Domain Layer:** Entity models and DTOs

## 🌟 Key Features

### Location Management

- CRUD operations for locations
- Location details and categories
- Opening times management
- Selectable locations with category grouping

### Route Management

- Create and retrieve routes
- Start and end location association
- Route creator tracking

### User Management

- User authentication
- Role-based authorization
- Secure password handling

## 🔧 Development Setup

1. Ensure .NET SDK is installed
2. Install required global tools:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
3. Configure database connection in `secrets.json`
4. Run the application:
   ```bash
   dotnet run
   ```

## 🤝 Contributing

When contributing to this project:

1. Follow the established Clean Architecture patterns
2. Use AutoMapper for object mapping
3. Include appropriate unit tests
4. Follow the existing code style and patterns
