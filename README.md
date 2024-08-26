# Company API

## Overview

The **Company API** is a .NET Core-based web service that allows for the management of company records. This API supports creating, retrieving, updating, and listing company records. It enforces uniqueness on the ISIN field and includes authentication for secure access.

## Features

- **Create**: Add a new company with Name, Stock Ticker, Exchange, ISIN, and optional Website URL.
- **Read**: Retrieve a company by ID or ISIN.
- **List**: Retrieve a collection of all companies.
- **Update**: Modify existing company details.
- **Authentication**: Secure API endpoints with Bearer Token authentication.

## Technologies Used

- **Backend**: ASP.NET Core 8 Web API
- **Frontend**: ASP.NET Core MVC (for the client application)
- **Database**: SQL Server (production) / In-Memory Database (testing)
- **Testing**: xUnit, Moq
- **Authentication**: Bearer Token Authentication

## Database Design

### Company Table Schema

```sql
CREATE TABLE Companies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Exchange NVARCHAR(100) NOT NULL,
    Ticker NVARCHAR(10) NOT NULL,
    Isin NVARCHAR(12) NOT NULL UNIQUE,
    Website NVARCHAR(255) NULL
);
```

### SQL Scripts

To set up the database schema, use the following SQL script:

```sql
-- Create the database
CREATE DATABASE CompanyDb;

-- Use the created database
USE CompanyDb;

-- Create the Companies table
CREATE TABLE Companies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Exchange NVARCHAR(100) NOT NULL,
    Ticker NVARCHAR(10) NOT NULL,
    Isin NVARCHAR(12) NOT NULL UNIQUE,
    Website NVARCHAR(255) NULL
);
```

## API Endpoints

### Companies

- **POST** `/api/companies` - Create a new company
- **GET** `/api/companies` - Retrieve a list of all companies
- **GET** `/api/companies/{id}` - Retrieve a specific company by ID
- **GET** `/api/companies/isin/{isin}` - Retrieve a specific company by ISIN
- **PUT** `/api/companies/{id}` - Update an existing company

### Authentication

The API uses Bearer Token authentication. Include the token in the `Authorization` header as `Bearer <token>`.

## Setup

### Prerequisites

- [.NET 6 SDK or later](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-editions)

### Cloning and Building the Project

1. **Clone the Repository**

   ```sh
   git clone https://github.com/thisismck/CompanyApi.git
   cd CompanyApi
   ```

2. **Build and Run the API**

   ```sh
   dotnet build
   dotnet run
   ```

   The API will be available at `http://localhost:7110`.

3. **Build and Run the Client**

   Navigate to the `CompanyClient` directory and run:

   ```sh
   dotnet build
   dotnet run
   ```

   The client application will be available at `http://localhost:7027`.

## Testing

### API Tests

Navigate to the `CompanyApi.Tests` directory and run:

```sh
dotnet test
```

### Client Tests

Navigate to the `CompanyClient.Tests` directory and run:

```sh
dotnet test
```
## Deployment

### Deployment Steps

1. **Deploy the API to a server or cloud service that supports .NET Core applications.**
2. **Set up SQL Server and run the provided SQL scripts to create the database schema.**
3. **Configure the connection strings and authentication settings as required.**

### Environment Configuration

Ensure the following configuration settings are defined in `appsettings.json` or environment variables:

- **Connection String**: for SQL Server
- **JWT Settings**: for Bearer Token authentication

## Bonus Features

### Simple Client

The project includes a simple ASP.NET Core MVC client that interacts with the API and displays results in a browser.

### Authentication

The API is secured using Bearer Token authentication. Tokens must be included in the `Authorization` header of requests.
