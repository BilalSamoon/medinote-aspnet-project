# MediNote

MediNote is a web-based medical appointment management system built with ASP.NET Core MVC.

The goal of the project is to help manage appointments more efficiently for different types of users, including doctors, patients, and administrators.

---

## Current Features

### Doctor Module
- View doctor schedule  
- Manage availability  
- Review pending appointments  
- Approve or reject appointment requests  
- Reschedule appointments  

### Admin Module
- View system reports  
- Manage appointments (approve/reject)  
- Priority-based appointment evaluation  
- Dashboard with system overview  

---

## API Endpoints

Doctor-related API endpoints return JSON data for:

- schedule  
- availability  
- pending appointments  
- reschedule details  

---

## Testing

NUnit tests have been implemented for both doctor and admin service logic.

### Current test coverage:
- Schedule service  
- Availability service  
- Doctor appointment service  
- Priority calculation service  
- Admin report service  

---

## Technologies Used

- ASP.NET Core MVC  
- C#  
- Bootstrap  
- Web API  
- NUnit  
- Entity Framework Core (configured)  
- SQL Server (configured)  

---

## Project Structure

- **MediNote.Web** – main web application  
- **MediNote.Tests** – NUnit test project  

---

## Notes

This project follows MVC architecture and applies separation of concerns by using service classes for business logic.

The system currently uses in-memory data for simplicity, but it is structured to support database persistence using Entity Framework Core and SQL Server.

---

## Authors

- Daniel Guillaumont  
- Camila Esguerra  
- Bilal Ahmed Samoon  
