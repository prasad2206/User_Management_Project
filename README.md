# User Management System

A simple **User Management** web application including a static frontend (HTML/CSS/JS) and a .NET 6 Web API backend.  
This project demonstrates a basic CRUD API for user records, country/city lookups, and integration with a client-side UI.

---

## Project Highlights

- **Frontend (UI)**: Static HTML pages with JavaScript for interaction.
  - Pages include: Registration, Login, User List, User View, User Edit, About.
  - JS uses `fetch` to call backend APIs at `http://localhost:5138`.
- **Backend (API)**: .NET 6 Web API
  - Controllers expose endpoints for user CRUD and country/city data.
  - Uses a `DBoperations` helper class (in `Database/DBoperations.cs`) for database operations.
  - Swagger enabled for API testing (Development environment).
  - CORS configured to allow frontend requests.

---

## Tech Stack

- Frontend: HTML5, CSS3, JavaScript (vanilla)
- Backend: .NET 6 Web API, C#
- Database: SQL Server (MSSQL)
- ORM / DB Access: Direct DB helper class (DBoperations) / ADO.NET style queries (no EF migrations required)
- Tools: Visual Studio 2022 (recommended), SQL Server Management Studio (SSMS), Git/GitHub

---

## Project Structure

```
UserManagementProject/
├── UI/                      # Static frontend (HTML/CSS/JS)
│   ├── HTML/
│   ├── CSS/
│   └── JS/
├── UserManagementAPI/       # .NET 6 Web API project
│   ├── Controllers/
│   ├── Database/            # DBoperations.cs
│   ├── Properties/
│   └── Program.cs
└── README.md
```

---

## Getting Started (Local Development)

> **Prerequisites**
> - .NET 6 SDK installed
> - Visual Studio 2022 (recommended) or VS Code
> - SQL Server instance (LocalDB or full SQL Server)
> - SQL Server Management Studio (optional, for DB viewing)

### Backend: run API
1. Open the solution `UserManagementAPI.sln` in Visual Studio 2022.
2. Update the connection string in `appsettings.json` (or `appsettings.Development.json`) to point to your SQL Server instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=UserManagementDB;Trusted_Connection=True;"
}
```
3. If the project includes SQL script(s) under `Database/` or a `.sql` file, run them in SSMS to create the required tables (`Users`, `Countries`, `Cities`, etc.). If not, create a database and the following minimal table structure:

```sql
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(200),
    Email NVARCHAR(200),
    Username NVARCHAR(100),
    DOB DATE,
    CountryId INT,
    CityId INT
);
```

4. Build and run the Web API project (F5 in Visual Studio). By default the API listens on `http://localhost:5138` (verify in `launchSettings.json`).

5. Open Swagger at `http://localhost:5138/swagger` to test endpoints.

### Frontend: open UI pages
- The frontend consists of static HTML files in `UI/HTML`. You can open these directly in a browser (double-click `Index.html`), or serve them via a simple static server.
- Ensure the `baseURL` in `UI/JS/*.js` files points to your API URL (default `http://localhost:5138`).

---

## Key API Endpoints (examples)

- `GET /api/User` — Get all users  
- `GET /api/User/{id}` — Get user by id  
- `POST /api/User` — Create user (expects JSON body)  
- `PUT /api/User/{id}` — Update user  
- `DELETE /api/User/{id}` — Delete user  
- `GET /api/Country` — Get all countries  
- `GET /api/Country/{countryId}/Cities` — Get cities by country

> Use Swagger UI or Postman to interact with these endpoints.

---

## Common Tasks

- **Change API URL in frontend**: Edit the top of `UI/JS/Registration.js` (and other JS files) where `baseURL` is defined.
- **Database seeding**: If seed scripts are not present, insert sample users and countries in SSMS for testing.
- **CORS issues**: Ensure backend CORS policy allows `http://localhost:5173` or `file://` origins depending on how you open UI.

---

## Notes & Recommendations

- The backend uses a `DBoperations` helper class for DB access. For larger projects, consider switching to **Entity Framework Core** with migrations for schema management.
- Add proper input validation and server-side checks to secure the API.
- Store connection strings and secrets in environment variables or secret storage (never commit credentials).
- Consider adding unit tests and API integration tests for reliability.

---

## Screenshots

*(Add screenshots of the UI and SSMS database here)*

---

## Author

**Prasad Deshpande**  
- GitHub: https://github.com/your-username  
- Email: your.email@example.com  
