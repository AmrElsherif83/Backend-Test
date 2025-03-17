# 🚀 Backend Test API

![GitHub](https://img.shields.io/github/license/AmrElsherif83/Backend-Test)
![GitHub last commit](https://img.shields.io/github/last-commit/AmrElsherif83/Backend-Test)

## 📌 Overview
This project is a backend API built using **.NET 9**, **DapperExtensions**, and **SQLite** with **Clean Architecture** and **CQRS** patterns. It provides CRUD operations for managing **Drivers** using **Generic Repository** and **Unit of Work**.

## 🏗️ Architecture
- **CQRS (Command Query Responsibility Segregation)**
- **Clean Architecture** (Separation of Concerns)
- **DapperExtensions** for ORM
- **SQLite** as the database
- **Unit of Work & Generic Repository** pattern
- **Logging with ILogger**
- **Validation with FluentValidation**

## 📦 Features
- ✅ CRUD operations for Driver entity
- ✅ Transactions & Unit of Work for database operations
- ✅ DapperExtensions integration with mappings
- ✅ Logging & Exception handling
- ✅ Swagger API Documentation
- ✅ Integration Tests using xUnit

---

## ⚙️ Setup & Installation
### 1️⃣ Prerequisites
- Install **.NET 9 SDK**
- Install **SQLite** (if not using in-memory mode)
- Install **Postman** (optional for API testing)

### 2️⃣ Clone the Repository
```sh
git clone https://github.com/your-repo/backend-test-api.git
cd backend-test-api
```

### 3️⃣ Configure the Database
Modify the **`appsettings.json`** file in the API project:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=App_Data/test.db"
  }
}
```

### 4️⃣ Run Database Migrations (DbUp)
```sh
dotnet run --project Backend_Test.DbMigrator
```

### 5️⃣ Run the API
```sh
dotnet run --project Backend_Test.API
```

### 6️⃣ Access Swagger Documentation
📌 Open **Swagger UI** in your browser:
```
http://localhost:5000/swagger
```

---

## 🚀 API Endpoints
### 🏠 **Base URL**
```
http://localhost:5000/api/drivers
```

### ✅ **Create Driver** (POST)
```http
POST /api/drivers
Content-Type: application/json

{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890"
}
```

### 📥 **Get Driver by ID** (GET)
```http
GET /api/drivers/{id}
```

### 📃 **Get All Drivers** (GET)
```http
GET /api/drivers
```

### ✏️ **Update Driver** (PUT)
```http
PUT /api/drivers/{id}
Content-Type: application/json

{
    "firstName": "Jane",
    "lastName": "Doe",
    "email": "jane.doe@example.com",
    "phoneNumber": "+9876543210"
}
```

### ❌ **Delete Driver** (DELETE)
```http
DELETE /api/drivers/{id}
```

---

## 🔬 Running Tests
### ✅ **Run Unit & Integration Tests**
```sh
dotnet test
```

### 📌 **Test Coverage Includes:**
- `GenericRepositoryTests`
- `UpdateDriverCommandHandlerTests`
- `DeleteDriverCommandHandlerTests`
- `GetDriverByEmailQueryHandlerTests`
- `DriversControllerTests`

---

## 🛠️ Technologies Used
- **.NET 9** - Backend Framework
- **Dapper & DapperExtensions** - ORM for database operations
- **SQLite** - Database (in-memory & file-based)
- **CQRS & Clean Architecture** - Design Patterns
- **FluentValidation** - Input validation
- **xUnit & Moq** - Unit Testing
- **Serilog & ILogger** - Logging
- **Swagger** - API Documentation

---

## 📌 Contribution
1. **Fork** the repository.
2. **Create** a new branch: `feature-branch`.
3. **Commit** your changes: `git commit -m "Added new feature"`
4. **Push** to the branch: `git push origin feature-branch`
5. **Open a Pull Request**.

---

## 📜 License
This project is licensed under the **MIT License**.

---

## 📞 Support
For any issues, please **open an issue** in the GitHub repository or contact the maintainers.

---

⭐ **Don't forget to give this repo a star
