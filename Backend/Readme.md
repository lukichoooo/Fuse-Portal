
Backend  written in C# (.NET 9), providing APIs for managing university data, schedules, students, exams and communication with LLM.

# External Services
OCR - Tesseract (Linux)
LLM - ML Studio (responses api)
Cache - Redis (default port)
Database - MSSql (port 1433)

# Requirements
.NET 9 SDK
MSSQL Server (local or remote)
Redis server
Linux environment for Tesseract OCR
appsettings.Development.json (file with data)

# Setup

Clone the repository:
```
git clone https://github.com/lukichoooo/Fuse-Portal.git
cd Fuse-Portal/Backend
```

Update appsettings.Development.json with your database, Redis, OCR, and ML Studio API settings.

Build and run the backend:
```
dotnet build
cd Presentation
dotnet run
```
