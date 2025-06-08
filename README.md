# BlockchainVote

BlockchainVote is a secure, blockchain-based voting system that provides transparent and tamper-proof elections. This application allows users to create polls, cast votes, and verify results through a blockchain ledger.

### Features

* **Secure Authentication**: JWT-based authentication with refresh tokens

* **Poll Management**: Create and manage polls with customizable options

* **Blockchain Integration**: All votes are recorded on a blockchain for transparency and immutability

* **Real-time Results**: View poll results as they come in

* **Flexible Voting Options**: Support for single-choice and multi-selection polls

* **Anonymous Voting**: Option to create polls with anonymous voting

### Architecture

The application follows a clean architecture pattern with the following components:

* **Domain**: Core business entities and logic

* **Application**: Business rules, CQRS handlers, and interfaces

* **Persistence**: Database access and entity configurations

* **Services**: Implementation of application services

* **WebAPI**: RESTful API endpoints

### Technologies

* **.NET 9.0**: Modern C# framework

* **Entity Framework Core**: ORM for database access

* **PostgreSQL**: Primary database

* **Redis**: Caching layer

* **MediatR**: CQRS and mediator pattern implementation

* **FluentValidation**: Request validation

* **AutoMapper**: Object mapping

* **Serilog**: Structured logging

* **Swagger**: API documentation

* **JWT Authentication**: Secure token-based authentication

### API Endpoints

![Swagger Endpoints](/images/image.png)

## Getting Started

### Prerequisites

* *.NET 9.0 SDK*

* PostgreSQL

* Redis

### Configuration

*appsettings.json*:
```json
{
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Postgres": "connectionString",
    "Redis": "connectionString"
  },
  "JwtOptions": {
    "Issuer": "Issuer",
    "Audience": "Audience",
    "Key": "YOURs_SECRET_KEY"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Running the Application

1. Clone the repository

2. Navigate to the project directory

3. Run the following commands:
    ```do
    dotnet restore
    dotnet build
    dotnet run --project WebAPI
    ```

4. Access the Swagger UI at https://localhost:9000/swagger

### Background Services

The application includes a background service that processes pending votes and adds them to the blockchain. This service runs every 10 minutes to collect votes and create new blocks.

### Security Features

* Password hashing using BCrypt

* JWT token authentication

* Refresh token rotation

* Input validation using FluentValidation

* Anonymous voting option

* Blockchain immutability for vote integrity

#### License: [CC-BY](https://creativecommons.org/licenses/by/4.0/legalcode)
