# CinemaBooking

### Description

**CinemaBooking** is a back-end application for movie tickets reservation split into multiple microservices with each keeping its own database. Communication between microservices is, for now, done only synchronously through their REST API endpoints. User access is role-based with "user" and "admin" roles. Ocelot API Gateway provides a single entry point for clients, featuring caching and authentication.

### Technologies

- ASP.NET Core
- Entity Framework Core
- SQL Server
- Ocelot

### Users

Login credentials:

- _USERNAME_: novana _PASSWORD_: Novana-0
- _USERNAME_: admin _PASSWORD_: Administrator-0

### Features in progress

- Distributed caching with Redis

### Diagram

Basic idea of client's interaction with microservices:

![Diagram](diagram.png)

### Databases

![Databases](databases.png)
