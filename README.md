# ConcurrencyDemo

Minimal EF Core concurrency demo using a rowversion token on `Person` with a
simple clean architecture layout.

## Requirements
- .NET SDK 8.0
- SQL Server (local instance or container)

## Setup
1. Update the connection string in `appsettings.json` if needed.
2. Apply migrations:
	- `dotnet ef database update`

## Run
- `dotnet run`

## Structure
- ConcurrencyDemo.Application: domain models and service contracts
- ConcurrencyDemo.Infrastructure: EF Core, seeding, and concurrency handling