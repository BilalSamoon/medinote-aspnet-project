# MediNote

MediNote is a role-based medical appointment management system built with **ASP.NET Core MVC**, **Razor Pages**, **Web API**, **Entity Framework Core**, and **SQL Server LocalDB**.

The application supports three core roles:

* **Patients** can register, log in, book appointments from valid doctor time slots, cancel appointments, and review appointment history and prescriptions.
* **Doctors** can manage their schedule and availability, review pending requests, approve or cancel appointments, and add doctor notes, prescriptions, reminders, and completion updates.
* **Admins** can monitor the system, manage appointments, generate doctor/admin accounts and IDs, and book appointments on behalf of patients.

---

## Features

### Patient Portal

* Account registration and sign-in
* Dashboard with appointment counts and next appointment summary
* Book appointments from doctor-published availability only
* Cancel and reschedule appointments
* Review appointment details, doctor notes, and prescriptions

### Doctor Portal

* View personal schedule
* Manage availability slots
* Review pending appointments
* Approve or cancel appointments
* Add doctor notes and prescriptions
* Queue reminders and mark visits as completed

### Admin Portal

* Dashboard with appointment metrics and recent activity
* Manage appointments across the system
* Book appointments on behalf of patients
* Generate doctor and admin accounts
* Generate and manage secure Doctor/Admin IDs
* View reports and priority-based appointment summaries

### API Layer

The project includes API endpoints for:

* account registration
* patient dashboards and appointment actions
* doctor scheduling and clinical actions
* admin dashboards, appointment management, and account generation

---

## Tech Stack

* **.NET 10**
* **ASP.NET Core MVC**
* **Razor Pages**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server LocalDB**
* **Cookie Authentication** for the web app
* **JWT package configured** for the API project
* **NUnit** for unit testing

---

## Solution Structure

```text
MediNote/
├── MediNote.API/        # Web API project
├── MediNote.Web/        # Main MVC + Razor Pages web application
├── MediNote.Tests/      # NUnit test project
└── MediNote.slnx        # Solution file
```

### Main folders in `MediNote.Web`

* `Controllers/` – MVC and API controllers
* `Views/` – Razor UI for home, account, admin, doctor, and patient pages
* `Models/` – Core domain models such as `Appointment`, `Availability`, `DoctorNote`, and `Prescription`
* `Services/` – Business logic services and repositories
* `ViewModels/` – UI-focused models used by pages and views
* `Data/` – `DbContext` and schema bootstrap logic
* `Migrations/` – Entity Framework migrations

---

## Architecture Overview

MediNote follows a layered structure:

* **Presentation layer**: MVC views and Razor Pages
* **Controller layer**: request handling for web and API endpoints
* **Service layer**: scheduling, availability, appointment workflows, patient dashboards, notifications, and reporting
* **Data layer**: Entity Framework Core with SQL Server LocalDB

Both the **web app** and the **API project** use the same underlying data and service layer through the `MediNote.Web` project.

---

## Authentication and Access Rules

* **Patients** log in with username and password.
* **Doctors** and **Admins** log in with username, password, and a valid **Doctor/Admin ID**.
* Cookie authentication is used in the main web app.
* Role-based authorization protects doctor and admin functionality.

---

## Default Test Credentials

For quick instructor/demo testing, the repository includes a credentials file with sample accounts:

```text
DOCTOR
username: doctor1
password: password123

PATIENT
username: patient1
password: patientpassword

ADMIN
username: admin1
password: adminpassword
```

Note: these are demo/testing credentials only and should not be used in a real production application.

---

## API Endpoints

### Account

* `POST /api/account/register`

### Patient

* `GET /api/patient/dashboard`
* `GET /api/patient/appointments`
* `GET /api/patient/appointments/{id}`
* `POST /api/patient/appointments`
* `POST /api/patient/appointments/{id}/cancel`
* `POST /api/patient/appointments/{id}/reschedule`
* `GET /api/patient/doctor-slots`
* `GET /api/patient/bookable-slots`
* `GET /api/patient/doctors`

### Doctor

* `GET /api/doctor/schedule`
* `GET /api/doctor/availability`
* `GET /api/doctor/pendingappointments`
* `POST /api/doctor/appointments/{id}/approve`
* `POST /api/doctor/appointments/{id}/cancel`
* `GET /api/doctor/appointments/{id}`
* `POST /api/doctor/appointments/{id}/notes`
* `POST /api/doctor/appointments/{id}/prescriptions`
* `POST /api/doctor/appointments/{id}/complete`
* `POST /api/doctor/appointments/{id}/reminders`

### Admin

* `GET /api/admin/dashboard`
* `GET /api/admin/appointments`
* `POST /api/admin/appointments`
* `POST /api/admin/appointments/{id}/approve`
* `POST /api/admin/appointments/{id}/cancel`
* `GET /api/admin/doctor-slots`
* `GET /api/admin/bookable-slots`
* `POST /api/admin/users/generate`
* `POST /api/admin/security-codes/generate`

---

## Running the Project

### Prerequisites

* .NET 10 SDK
* SQL Server LocalDB
* Visual Studio 2022 or the `dotnet` CLI

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd medinote-aspnet-project/MediNote
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Build the solution

```bash
dotnet build
```

### 4. Run the web app

```bash
cd MediNote.Web
dotnet run
```

The web app is configured for:

* `https://localhost:7067`
* `http://localhost:5274`

### 5. Run the API project

```bash
cd ../MediNote.API
dotnet run
```

In development, the API also exposes OpenAPI metadata.

---

## Database Notes

The web project is configured to use:

```json
Server=(localdb)\\mssqllocaldb;Database=MediNoteDb;Trusted_Connection=True;MultipleActiveResultSets=true
```

On startup, the application calls schema bootstrap logic to ensure the database schema is compatible.

Entity Framework migration files are already included in the project.

---

## Testing

The project includes NUnit tests for major services and repositories, including:

* `AdminReportServiceTests`
* `AppointmentRepositoryTests`
* `AvailabilityServiceTests`
* `DoctorAppointmentServiceTests`
* `PatientServiceTests`
* `PriorityCalculationServiceTests`
* `ScheduleServiceTests`
* `UserRepositoryTests`

Run tests with:

```bash
cd MediNote.Tests
dotnet test
```

---

## Project Highlights

* Clean separation between UI, service, and data layers
* Shared service/data logic between web app and API
* Role-based workflows for patients, doctors, and admins
* Clinical actions such as doctor notes, prescriptions, reminders, and appointment completion
* Admin support for booking on behalf of patients and generating secure IDs

---

## Authors

* **Daniel Guillaumont**
* **Camila Esguerra**
* **Bilal Ahmed Samoon**
