# AcademiaX - Educational Management System API

A modern, enterprise-grade ASP.NET Core Web API designed to provide comprehensive educational management capabilities for academic institutions. AcademiaX streamlines student enrollment, course management, grade tracking, attendance monitoring, and institutional communications.

---

## 📋 Table of Contents

- [Technical Overview](#technical-overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Setup Guide](#setup-guide)
- [Database Migration](#database-migration)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Development](#development)

---

## 🏗️ Technical Overview

### System Architecture

AcademiaX follows a **Layered Architecture (N-Tier)** pattern with strict separation of concerns across four distinct layers:

```
┌─────────────────────────────────────────┐
│   Presentation Layer (AcademiaX)        │
│   - RESTful API Controllers             │
│   - Request/Response Handling           │
│   - CORS & Security Headers             │
└────────────┬────────────────────────────┘
             │
┌────────────▼────────────────────────────┐
│   Business Logic Layer (AcademiaX_Business)         │
│   - Service Interfaces (Abstraction)    │
│   - Service Implementations (Concrete)  │
│   - Business Rules & Validation         │
│   - AutoMapper Profiles                 │
│   - Data Transfer Objects (DTOs)        │
└────────────┬────────────────────────────┘
             │
┌────────────▼────────────────────────────┐
│   Core Layer (AcademiaX_Core)           │
│   - Models (ApiResponse)                │
│   - Configuration Settings              │
│   - Constants & Enumerations            │
└────────────┬────────────────────────────┘
             │
┌────────────▼────────────────────────────┐
│   Data Access Layer (AcademiaX_Data_Access)          │
│   - DbContext (EF Core)                 │
│   - Domain Entities                     │
│   - User Models (ASP.NET Identity)      │
│   - Database Migrations                 │
│   - Enumerations                        │
└─────────────────────────────────────────┘
             │
┌────────────▼────────────────────────────┐
│   Database Layer                        │
│   - SQL Server 2019+                    │
└─────────────────────────────────────────┘
```

### Design Patterns Implemented

1. **Dependency Injection (DI)**: Utilized throughout Program.cs for loose coupling
2. **Repository-like Pattern**: Services abstract data access operations
3. **DTO Pattern**: Data Transfer Objects for API contracts
4. **Mapper Pattern**: AutoMapper for entity-to-DTO transformations
5. **Identity Pattern**: ASP.NET Core Identity for authentication
6. **JWT Token Pattern**: Stateless authentication for REST APIs

---

## 🔧 Tech Stack

### Core Framework

- **Runtime**: .NET 7.0 (LTS)
- **Framework**: ASP.NET Core Web API
- **Language**: C# 11.0+

### Data Access & ORM

- **Entity Framework Core** (v7.0.13)
  - SQL Server provider
  - Migrations support
  - Lazy loading & eager loading
- **SQL Server 2019+**: Primary database

### Authentication & Authorization

- **ASP.NET Core Identity**: Role-based and claims-based authorization
- **JWT (JSON Web Tokens)**: Stateless authentication
- **Microsoft.AspNetCore.Authentication.JwtBearer** (v7.0.13)

### API & Documentation

- **Swagger/Swashbuckle** (v6.5.0): Interactive API documentation (Swagger UI)
- **OpenAPI 3.0** specification support

### Data Mapping

- **AutoMapper** (v12.0.1): Object-to-object mapping for DTOs

### Cross-Cutting Concerns

- **CORS (Cross-Origin Resource Sharing)**: Enabled for React frontend (http://localhost:3000)
- **HTTPS Redirection**: Enforced in production
- **Structured Logging**: Built-in Microsoft.Extensions.Logging

### External Integrations

- **GTFS Data**: General Transit Feed Specification for public transportation integration

---

## ✨ Features

### User Management

- **Registration**: User sign-up with role assignment (Student, Teacher, Admin)
- **Authentication**: Login with JWT token generation
- **Authorization**: Role-based access control (RBAC)
- **Profile Management**: User profile viewing and updates
- **User Types**:
  - **Students**: Academic information (Department, Faculty, GPA, Academic Level)
  - **Teachers**: Professional information (Title, Office, Biography, Department)
  - **Administrators**: System management capabilities

### Course Management

- **CRUD Operations**: Create, read, update, and delete courses
- **Course Enrollment**: Students can enroll/unenroll from courses
- **Many-to-Many Relationships**: Support multiple students per course
- **Teacher Assignment**: Each course assigned to a single teacher
- **Course Metadata**:
  - Course code, title, and description
  - Credit hours
  - Semester and department mapping
  - Student roster management

### Academic Features

- **Grade Management**:
  - Multiple exam types (Midterm, Final, Makeup)
  - Grade entry and tracking
  - Weighted grade calculation
  - GPA calculations

- **Attendance Tracking**:
  - Per-student, per-course attendance records
  - Attendance status (Present, Absent, Excused, Late)
  - Attendance reporting

- **Announcements**:
  - System-wide and course-specific announcements
  - Timestamped communication
  - User association tracking

### Communication Features

- **Internal Messaging**: Direct messaging between users
- **Message History**: Persistent message storage
- **Sender/Receiver Relationships**: Bi-directional communication support

### Transportation Integration

- **GTFS Support**: Public transit data integration
  - Stops and routes management
  - Trip scheduling
  - Stop time information
- **Data Import**: GTFS zip file parsing and data population

### API Features

- **RESTful Endpoints**: Standard HTTP methods (GET, POST, PUT, DELETE)
- **Standardized Responses**: Consistent ApiResponse model
- **HTTP Status Codes**: Proper HTTP status code usage
- **Input Validation**: DTO-level validation with error messages
- **Error Handling**: Centralized error response format
- **Asynchronous Operations**: Full async/await support for scalability

---

## 📚 API Documentation

### Interactive Documentation

The API includes **Swagger/OpenAPI documentation** that is automatically generated and available at runtime.

**Access Swagger UI:**

```
http://localhost:5000/swagger/index.html
```

### Core Endpoints

#### Authentication Endpoints

```
POST   /api/user/register          - Register new user
POST   /api/user/login             - User login (returns JWT)
GET    /api/user/{id}              - Get user by ID
```

#### Course Endpoints

```
POST   /api/course/create          - Create new course
PUT    /api/course/update          - Update course details
DELETE /api/course/delete/{id}     - Delete course
GET    /api/course/all             - List all courses
GET    /api/course/{id}            - Get course by ID
POST   /api/course/enroll          - Enroll student in course
POST   /api/course/unenroll        - Unenroll student from course
```

#### Additional Controllers

- **Student Management**: /api/student
- **Teacher Management**: /api/teacher
- **GTFS Data**: /api/gtfs
- (Additional endpoints documented in Swagger UI)

### Response Format

All API responses follow a standardized format:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "errorMessages": [],
  "result": {
    // Data object
  }
}
```

Error Response Example:

```json
{
  "statusCode": 400,
  "isSuccess": false,
  "errorMessages": ["Invalid email format", "Password too short"],
  "result": null
}
```

---

## 🗄️ Database Schema

### Core Entities

#### ApplicationUser (Extended Identity User)

- **PK**: Id (String - GUID)
- **Authentication**: UserName, Email, PasswordHash
- **Profile**: FirstName, LastName, DateOfBirth, Address, RegistrationDate, Image
- **Classification**: UserType (Student, Teacher, Admin)
- **Academic Info**: Department, Faculty, GPA, AcademicLevel (for Students)
- **Professional Info**: Branch, Title, Office, Biography (for Teachers)
- **Relationships**: Courses, Grades, Attendances, Announcements, Advisor

#### Course

- **PK**: Id (Int)
- **Details**: Code, Title, Name, Description, Credits
- **Academic**: SemesterId, DepartmentId
- **FK**: TeacherId → ApplicationUser
- **Relationships**: Students (Many-to-Many), Teacher (One-to-Many)
- **Junction Table**: StudentCourses (Implicit via Many-to-Many)

#### Grade

- **PK**: Id (Int)
- **FK**: StudentId → ApplicationUser, CourseId → Course
- **Details**: ExamType (enum), Value (0-100), TotalGrade
- **Tracking**: Exam type differentiation

#### Attendance

- **PK**: Id (Int)
- **FK**: StudentId → ApplicationUser, CourseId → Course
- **Details**: AttendanceStatus (enum: Present, Absent, Excused, Late)
- **Tracking**: Date and time information

#### Announcement

- **PK**: Id (Int)
- **FK**: UserId → ApplicationUser
- **Content**: Title, Content
- **Tracking**: DatePosted

#### Message

- **PK**: Id (Int)
- **Communication**: Content, Subject
- **Participants**: SenderId, RecipientId (both → ApplicationUser)
- **Tracking**: Timestamps (Sent, Read)

#### GTFS Tables (Transportation)

- **Stop**: Transit stop information
- **Trip**: Transit trip details
- **StopTime**: Stop timing information (composite view - no key)

### Entity Relationships

```
ApplicationUser (1) ──── (M) Course          [Teacher-Course: One-to-Many]
        │                                     [Delete Behavior: Restrict]
        │
        ├── (M) StudentCourses (M) ────  [Many-to-Many via junction]
        │
        ├── (1) ──── (M) Grade            [Student-Grade: One-to-Many]
        │
        ├── (1) ──── (M) Attendance       [Student-Attendance: One-to-Many]
        │
        ├── (1) ──── (M) Announcement     [User-Announcement: One-to-Many]
        │
        └── (1) ──── (M) Advisor          [Student-Advisor: Self-Referential]
```

---

## 🚀 Setup Guide

### Prerequisites

- **.NET 7.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/7.0))
- **SQL Server 2019** or later (locally installed or remote connection)
- **Visual Studio 2022** or **Visual Studio Code**
- **Git** for version control

### Installation Steps

#### 1. Clone Repository

```bash
git clone https://github.com/48pinaribs/AcademiaXAPI.git
cd AcademiaX
```

#### 2. Restore NuGet Packages

```bash
dotnet restore
```

#### 3. Configure appsettings.json

Update `appsettings.json` with your environment-specific settings (see [Configuration](#configuration) section).

#### 4. Apply Database Migrations

```bash
# Navigate to the solution directory
cd AcademiaX

# Update database with latest migrations
dotnet ef database update
```

#### 5. Run the Application

```bash
dotnet run
```

The API will start on:

- HTTP: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger/index.html`

#### 6. Verify Installation

Navigate to Swagger UI and test a basic endpoint to confirm the API is operational.

---

## 🔄 Database Migration

### Understanding Entity Framework Migrations

Migrations track database schema changes over time, enabling version control of the database structure.

### Existing Migrations

The project includes the following migrations:

1. **20250510213403_InitialCreate**
   - Initial database schema
   - ApplicationUser, Course, Grade, Attendance, Announcement, Message tables

2. **20250530133411_InitialGtfsSchema**
   - GTFS transportation data integration
   - Stop, Trip, StopTime tables

### Creating New Migrations

If you modify entity models, create a new migration:

```bash
# Create a migration
dotnet ef migrations add MigrationName

# Review the migration file in Migrations/
# Then apply it
dotnet ef database update
```

### Migrations Commands Reference

```bash
# List all migrations
dotnet ef migrations list

# Create a new migration
dotnet ef migrations add <MigrationName>

# Remove the last migration (if not applied to db)
dotnet ef migrations remove

# Update database to latest migration
dotnet ef database update

# Update database to specific migration
dotnet ef database update <MigrationName>

# Generate SQL script for migration
dotnet ef migrations script <FromMigration> <ToMigration>

# Drop the entire database
dotnet ef database drop
```

### Best Practices

- Always create migrations for model changes
- Name migrations descriptively (e.g., `AddGradeExamTypeColumn`)
- Review generated SQL before applying to production
- Test migrations in development environment first
- Keep migrations minimal and focused on single responsibility
- Never manually edit migration files after creation

---

## ⚙️ Configuration

### appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "server=YOUR_SERVER;database=AcademiaX;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "SecretKey": {
    "jwtKey": "your-secret-key-min-32-characters"
  },
  "GtfsSettings": {
    "DataPath": "path/to/gtfs/zip/file"
  }
}
```

### Configuration Parameters

#### ConnectionStrings

- **DefaultConnection**: SQL Server connection string
  - **server**: Server name or IP address
  - **database**: Database name
  - **Trusted_Connection**: Windows authentication (True/False)
  - **TrustServerCertificate**: SSL certificate validation

#### SecretKey

- **jwtKey**: JWT signing key (minimum 32 characters recommended)
  - Used for token generation and validation
  - Keep secure; never commit to version control

#### GtfsSettings

- **DataPath**: Path to GTFS data ZIP file
  - Used for public transportation data import
  - Can be local or network path

#### Logging

- **Default**: Minimum log level for application
- **Microsoft.AspNetCore**: Minimum log level for framework

### Environment-Specific Configuration

#### Development (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Override specific settings in development without exposing secrets in source control.

#### Production Considerations

**Security Best Practices:**

- Use **Azure Key Vault** or environment secrets for sensitive values
- Never commit secrets to version control
- Use strong JWT keys (minimum 32 characters)
- Enable HTTPS exclusively
- Set appropriate CORS policies (restrict to known origins)
- Use SQL Server authentication (avoid Trusted_Connection in production)

**Example Production Configuration:**

```bash
# Set environment variables
set ASPNETCORE_ENVIRONMENT=Production
set ConnectionStrings__DefaultConnection="your-production-connection-string"
set SecretKey__jwtKey="your-production-jwt-key"
```

---

## 📂 Project Structure

```
AcademiaX/                          # Main ASP.NET Core Web API Project
├── Controllers/                    # API Controllers
│   ├── CourseController.cs
│   ├── StudentController.cs
│   ├── TeacherController.cs
│   ├── UserController.cs
│   └── GtfsController.cs
├── Program.cs                      # Application startup & DI configuration
├── appsettings.json               # Configuration (production)
├── appsettings.Development.json   # Configuration (development)
└── Properties/
    └── launchSettings.json        # Launch profiles

AcademiaX_Business/                 # Business Logic Layer
├── Abstraction/                   # Service Interfaces
│   ├── IUserService.cs
│   ├── IStudentService.cs
│   ├── ITeacherService.cs
│   ├── ICourseService.cs
│   └── IGtfsService.cs
├── Concrete/                      # Service Implementations
│   ├── UserService.cs
│   ├── StudentService.cs
│   ├── TeacherService.cs
│   ├── CourseService.cs
│   └── GtfsService.cs
├── Dtos/                          # Data Transfer Objects
│   ├── LoginRequestDTO.cs
│   ├── RegisterRequestDTO.cs
│   ├── PersonDTO.cs
│   ├── Courses/                   # Course-related DTOs
│   ├── Gtfs/                      # GTFS-related DTOs
│   └── [Other DTOs]
└── Mapper/
    └── MappingProfile.cs          # AutoMapper configuration

AcademiaX_Core/                     # Core/Models Layer
├── Models/
│   ├── ApiResponse.cs             # Standard API response model
│   ├── LoginResponseModel.cs
│   └── [Other models]
└── Configuration/
    └── GtfsSettings.cs            # GTFS configuration

AcademiaX_Data_Access/              # Data Access Layer
├── Context/
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Domain/                        # Domain Entities
│   ├── Course.cs
│   ├── Grade.cs
│   ├── Attendance.cs
│   ├── Announcement.cs
│   ├── Message.cs
│   └── [Other entities]
├── Models/
│   ├── ApplicationUser.cs         # Identity User extension
│   ├── Stop.cs
│   ├── Trip.cs
│   ├── StopTime.cs
│   └── [Other models]
├── Enums/
│   ├── UserType.cs
│   ├── AttendanceStatus.cs
│   ├── ExamType.cs
│   └── [Other enums]
└── Migrations/                    # EF Core migrations
    ├── [Migration files]
    └── ApplicationDbContextModelSnapshot.cs
```

### Layer Responsibilities

**Presentation Layer (AcademiaX)**

- HTTP request/response handling
- Route definitions
- CORS configuration
- Swagger/OpenAPI setup
- Request validation at API level

**Business Layer (AcademiaX_Business)**

- Business logic implementation
- Service interfaces defining contracts
- Data validation and business rules
- DTO mapping (AutoMapper)
- Transaction management
- No direct database access

**Core Layer (AcademiaX_Core)**

- Shared models and constants
- Configuration classes
- Cross-layer data structures
- No dependencies on data access

**Data Access Layer (AcademiaX_Data_Access)**

- Entity Framework Core context
- Domain model definitions
- Database migrations
- Entity relationships and constraints
- No business logic

---

## 👨‍💻 Development

### Build the Solution

```bash
dotnet build
```

### Run Tests (if available)

```bash
dotnet test
```

### Clean Build

```bash
dotnet clean
dotnet build
```

### Code Standards

**Naming Conventions:**

- **Classes/Interfaces**: PascalCase
- **Methods**: PascalCase
- **Properties**: PascalCase
- **Local Variables**: camelCase
- **Constants**: UPPER_SNAKE_CASE
- **Private Fields**: \_camelCase
- **Interfaces**: IPascalCase (I prefix)

**Documentation:**

- XML comments for public methods
- Meaningful variable and method names
- Comments for complex business logic
- Clear commit messages

**Coding Principles:**

- SOLID Principles adherence
- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple, Stupid)
- Single Responsibility Principle
- Dependency Injection everywhere

### Debugging

**Enable Detailed Logging:**
Update appsettings.Development.json:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

**Using Visual Studio Debugger:**

- Set breakpoints (F9)
- Run in Debug mode (F5)
- Inspect variables in Watch window
- Use Debug → Step Into (F11)

### Adding New Features

**Adding a New Service:**

1. Create interface in `AcademiaX_Business/Abstraction/I<Feature>Service.cs`
2. Create implementation in `AcademiaX_Business/Concrete/<Feature>Service.cs`
3. Register in `Program.cs`: `builder.Services.AddScoped<I<Feature>Service, <Feature>Service>();`
4. Create DTOs in `AcademiaX_Business/Dtos/<Feature>/`
5. Add mapping profiles in `Mapper/MappingProfile.cs`
6. Create controller in `AcademiaX/Controllers/<Feature>Controller.cs`

**Adding a New Entity:**

1. Create entity class in `AcademiaX_Data_Access/Domain/`
2. Add DbSet to `ApplicationDbContext.cs`
3. Configure relationships in `OnModelCreating()`
4. Create migration: `dotnet ef migrations add Add<Entity>`
5. Apply migration: `dotnet ef database update`

---

## 📞 Support & Troubleshooting

### Common Issues

**Issue**: Connection string error

- **Solution**: Verify SQL Server is running and connection string is correct in appsettings.json

**Issue**: Migration fails

- **Solution**: Check if database exists; if not, drop and recreate: `dotnet ef database drop --force` then `dotnet ef database update`

**Issue**: JWT token invalid

- **Solution**: Ensure jwtKey in appsettings.json is consistent across environment

**Issue**: CORS errors

- **Solution**: Verify allowed origins in Program.cs match your frontend URL

---

## 📄 License

This project is proprietary. All rights reserved.

---

## 👥 Contributors

- Senior Development Team
- Database Architecture Team
- Quality Assurance Team

---

## 📝 Version History

| Version | Date       | Changes                                          |
| ------- | ---------- | ------------------------------------------------ |
| 1.0.0   | 2025-05-10 | Initial release with core academic functionality |
| 1.1.0   | 2025-05-30 | Added GTFS transportation integration            |

---

## 🔒 Security Notice

- Never commit `appsettings.json` with real secrets to version control
- Use environment variables for sensitive configuration in production
- Implement rate limiting for production deployment
- Enable HTTPS only in production
- Validate and sanitize all user inputs
- Use parameterized queries (EF Core does this automatically)

---

**Last Updated**: February 8, 2026  
**API Version**: 1.1.0  
**Target Framework**: .NET 7.0
