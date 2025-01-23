# Automated Computer Network Laboratory Examination System

Thesis-International Hellenic University.

## Description

The Exam System is a web-based application designed to manage and facilitate online exams. The system provides functionality for both teachers and students, including creating exams, managing questions and categories, and participating in exams. 
Authorization ensures that different roles (students and teachers) have access to appropriate functionalities.

## Getting Started

### Features

Student Features:

View and participate in exams.
Submit answers and view results if enabled by the teacher.
Teacher Features:

Create and manage exams.
Add categories and organize questions into packages.
Review and grade exams.
Admin Features:

Manage users and roles.
Monitor system activities.
Authorization:

Role-based access control ensures only authorized users can access certain features.
Caching:

Exam questions are cached for performance optimization.

### Technical Stack

Backend: ASP.NET Core
Frontend: Razor Pages,Bootstrap
Database: Microsoft SQL Server
ORM: Entity Framework Core
Caching: MemoryCache
Authentication/Authorization: Custom claims-based roles (Teacher, Student)

