# 🏢 Conference Rooms API

REST API for managing conference room bookings. Allows searching for available rooms, making reservations, and calculating rental costs based on time-of-day pricing and selected services.

---

## 📋 Table of Contents

- [Business Logic](#business-logic)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Request Examples](#request-examples)

---

## 💼 Business Logic

### Initial Rooms
| Room | Capacity | Base Price/hr |
|------|----------|---------------|
| Room A | 50 people | 2000 UAH |
| Room B | 100 people | 3500 UAH |
| Room C | 30 people | 1500 UAH |

### Available Services
| Service | Price |
|---------|-------|
| Projector | 500 UAH |
| Wi-Fi | 300 UAH |
| Sound System | 700 UAH |

### Time-of-Day Pricing
| Time | Rate |
|------|------|
| 06:00 – 09:00 | -10% (morning discount) |
| 09:00 – 12:00 | base price |
| 12:00 – 14:00 | +15% (peak surcharge) |
| 14:00 – 18:00 | base price |
| 18:00 – 23:00 | -20% (evening discount) |

> If a booking spans multiple pricing zones, each zone is calculated separately.

---

## 🛠 Tech Stack

- **Runtime:** .NET 10
- **Framework:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL
- **Documentation:** Swagger / OpenAPI
- **Architecture:** Clean Architecture

---

## 🏛 Architecture

The project follows **Clean Architecture** principles with a clear separation of concerns between layers:

```
Domain (Core)
    ↑
Infrastructure
    ↑
Controllers
```

- **Domain** — entities, DTOs, repository interfaces. Zero external dependencies.
- **Infrastructure** — repository implementations, AppDbContext, PricingService, DataSeeder.
- **Controllers** — controllers, middleware, dependency injection registration.

---

## 📁 Project Structure

```
ConferenceRoomsAPI/
├── Domain/
│   ├── Entities/
│   │   ├── Room.cs
│   │   ├── Service.cs
│   │   ├── Booking.cs
|   ├── Enums/
│   │   ├── BookingStatus.cs
│   │   └── TimaSlotType.cs
│   ├── DTOs/
│   │   ├── RoomDtos.cs
│   │   ├── ServiceDtos.cs
│   │   ├── BookingDtos.cs
│   │   └── ReportDtos.cs
|   ├── Services/
│   │   ├── BookingService.cs
│   │   └── RoomService.cs    
├── Infrastructure/
│   ├── Database/
│   │   ├── AppDbContext.cs
│   │   └── DataSeeder.cs
│   ├── Repositories/
│   │   ├── RoomRepository.cs
│   │   ├── BookingRepository.cs
│   │   └── ServiceRepository.cs
|   ├── InterfacesOfRepositories/
│   │   ├── IRoomRepository.cs
│   │   ├── IBookingRepository.cs
│   │   └── IServiceRepository.cs
│   └── Services/
│       └── PricingService.cs
│       ├── RoomAppService.cs
│       ├── BookingAppService.cs
│       ├── ServiceAppService.cs
│       └── ReportService.cs
├── Controllers/
│   ├── RoomsController.cs
│   ├── BookingsController.cs
│   ├── ServicesController.cs
│   └── ReportsController.cs
├── Middleware/
│   └── GlobalExceptionHandler.cs
├── Program.cs
└── appsettings.json
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Steps

**1. Clone the repository**
```bash
git clone https://github.com/your-username/ConferenceRoomsAPI.git
cd ConferenceRoomsAPI
```

**2. Create the database in pgAdmin**

Create an empty database named `ConferenceRoomsDB`.

**3. Configure the connection string**

Update `appsettings.json` with your credentials:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ConferenceRoomsDB;Username=postgres;Password=your_password"
  }
}
```

**4. Apply migrations**
```bash
dotnet ef database update
```

**5. Run the project**
```bash
dotnet run
```

Swagger UI will open at `http://localhost:{port}/`

> On first launch, `DataSeeder` will automatically populate the database with the initial rooms and services.

---

## 📡 API Endpoints

### Rooms
| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/rooms` | Get all rooms |
| `GET` | `/api/rooms/{id}` | Get room by ID |
| `GET` | `/api/rooms/available` | Search available rooms |
| `POST` | `/api/rooms` | Create a room |
| `PUT` | `/api/rooms/{id}` | Update a room |
| `DELETE` | `/api/rooms/{id}` | Delete a room |

### Bookings
| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/bookings` | Get all bookings |
| `GET` | `/api/bookings/{id}` | Get booking by ID |
| `POST` | `/api/bookings` | Create a booking |
| `PATCH` | `/api/bookings/{id}/cancel` | Cancel a booking |

### Services
| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/services` | Get all services |
| `GET` | `/api/services/{id}` | Get service by ID |
| `POST` | `/api/services` | Create a service |
| `PUT` | `/api/services/{id}` | Update a service |
| `DELETE` | `/api/services/{id}` | Delete a service |

### Reports
| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/reports/revenue` | Revenue per room for a date range |
| `GET` | `/api/reports/occupancy` | Room occupancy for a date range |
| `GET` | `/api/reports/popular-services` | Most popular services |

---

## 📝 Request Examples

### Search available rooms
```
GET /api/rooms/available?startTime=2024-09-01T10:00:00&endTime=2024-09-01T14:00:00&minCapacity=50
```

### Create a booking
```json
POST /api/bookings
{
  "roomId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startTime": "2024-09-01T10:00:00",
  "endTime": "2024-09-01T14:00:00",
  "serviceIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "3fa85f64-5717-4562-b3fc-2c963f66afa8"
  ]
}
```

### Revenue report
```
GET /api/reports/revenue?from=2024-09-01T00:00:00&to=2024-09-30T23:59:59
```
