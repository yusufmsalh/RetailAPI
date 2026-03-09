# Retail Inventory & Order Management API

A secure RESTful API built with 
**ASP.NET Core 8**,
 **Entity Framework Core**, 
 and **SQL Server**, 
 implementing N-Tier Architecture with JWT authentication.
 
 ## you can build a similar md file by https://github.com/ap0llo/markdown-generator or write it down manually

---

## Project Structure

```
RetailAPI/
├── src/
│   ├── RetailAPI.Core/              # Domain layer (Entities, Interfaces, DTOs, Exceptions)
│   ├── RetailAPI.Infrastructure/    # Data layer (DbContext, Repositories, Services, Migrations)
│   └── RetailAPI.API/               # Presentation layer (Controllers, Middleware, Extensions)
└── database/
    └── seed.sql                     # Optional manual SQL setup script
```

---



## Setup Instructions

### 1. Configure the Database Connection

Edit `src/RetailAPI.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=RetailDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

For SQL Server Express, use:
```
Server=.\\SQLEXPRESS;Database=RetailDB;Trusted_Connection=True;TrustServerCertificate=True;
```

For LocalDB:
```
Server=(localdb)\\mssqllocaldb;Database=RetailDB;Trusted_Connection=True;
```

### 2. Configure JWT Settings (Optional)

Update the `JwtSettings` section in `appsettings.json` to use a strong secret key in production:

you can use : https://jwtsecrets.com/#generator to generate sample secret keys

```json
"JwtSettings": {
  "SecretKey": "WriteKeyHere,atleast128bitskey!",
  "Issuer": "RetailAPI",
  "Audience": "RetailAPIClients",
  "ExpiryHours": "24"
}
```

### 3. Run the Application

```bash
cd src/RetailAPI.API
dotnet run
```

The application will **automatically apply migrations and seed the database** on first run.

Swagger UI will be available at: **http://localhost:5000** (or the port shown in your terminal)

---

## Default Seeded Accounts

| Username  | Password      | Role     |
|-----------|---------------|----------|
| admin     | Admin@123     | Admin    |
| customer1 | Customer@123  | Customer |
| customer2 | Customer@123  | Customer |

---

## API Endpoints

### Authentication
| Method | Endpoint        | Description              | Auth Required |
|--------|-----------------|--------------------------|---------------|
| POST   | /api/auth/login | Login and receive JWT    | No            |

### Products
| Method | Endpoint            | Description                   | Role     |
|--------|---------------------|-------------------------------|----------|
| GET    | /api/products       | List all products             | Any      |
| GET    | /api/products/{id}  | Get product details           | Any      |
| POST   | /api/products       | Create new product            | Admin    |
| PUT    | /api/products/{id}  | Update product                | Admin    |

### Orders
| Method | Endpoint          | Description              | Role     |
|--------|-------------------|--------------------------|----------|
| POST   | /api/orders       | Place a new order        | Customer |
| GET    | /api/orders/{id}  | Get order details        | Customer (own) / Admin |

### Inventory
| Method | Endpoint                          | Description             | Role  |
|--------|-----------------------------------|-------------------------|-------|
| GET    | /api/inventory                    | View all stock levels   | Admin |
| PATCH  | /api/inventory/{productId}/stock  | Adjust stock quantity   | Admin |

---



---

## Testing with Postman

sample collection can be found in repo

### Step 1 - Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "customer1",
  "password": "Customer@123"
}
```

### Step 2 - Place Order (use token from step 1)
```http
POST http://localhost:5000/api/orders
Authorization: Bearer <token>
Content-Type: application/json

{
  "items": [
    { "productId": 1, "quantity": 1 },
    { "productId": 2, "quantity": 3 }
  ]
}
```

### Step 3 - Check Inventory as Admin
```http
GET http://localhost:5000/api/inventory
Authorization: Bearer <admin-token>
```

### Step 4 - Adjust Stock
```http
PATCH http://localhost:5000/api/inventory/2/stock
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "adjustment": 50,
  "reason": "New shipment arrived"
}
```

---

## Manual Database Setup (Alternative)

If you prefer not to use EF Migrations, run the SQL script manually:

```bash
sqlcmd -S YOUR_SERVER -i database/seed.sql
```

> **Note:** The SQL script creates the schema but uses placeholder password hashes. Use the EF migration approach (default) for proper BCrypt-hashed seeded users.

---

## Key Design Decisions

- **N-Tier Architecture**: Core (domain), Infrastructure (data/services), API (presentation)
- **Unit of Work + Repository Pattern**: Encapsulates data access and transaction management
- **ACID Transactions**: Order placement wraps stock deduction and order creation in a single database transaction
- **BCrypt Password Hashing**: Industry-standard secure password storage
- **Global Exception Middleware**: Consistent JSON error responses across all endpoints
- **Input Validation**: DataAnnotations on all request DTOs
- **Role-Based Authorization**: Admin vs. Customer access control enforced at controller level

---

## Running Migrations Manually

```bash
# From solution root
cd src/RetailAPI.API
dotnet ef migrations add <MigrationName> --project ../RetailAPI.Infrastructure
dotnet ef database update
```
