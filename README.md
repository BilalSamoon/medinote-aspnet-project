# MediNote

MediNote is a web-based medical appointment management system built with ASP.NET Core MVC.

The goal of the project is to help manage appointments more efficiently for different types of users, including doctors, patients, and administrators.

## Current Features

### Doctor Module
- View doctor schedule
- Manage availability
- Review pending appointments
- Approve or reject appointment requests
- Reschedule appointments

### API Endpoints
Doctor-related API endpoints are available and return JSON data for:
- schedule
- availability
- pending appointments
- reschedule details

### Testing
- NUnit tests were added for doctor-related service logic
- Current test coverage includes:
  - schedule service
  - availability service
  - doctor appointment service

## Technologies Used
- ASP.NET Core MVC
- C#
- Bootstrap
- Web API
- NUnit

## Project Structure
- `MediNote.Web` – main web application
- `MediNote.Tests` – NUnit test project

## Notes
This project is still in progress.  
The doctor module is currently implemented, while the patient and admin modules are still under development.

## Authors
- Daniel Guillaumont
- Camila Esguerra
- Bilal Ahmed Samoon
