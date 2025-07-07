# Khabri API

**Khabri** is a modern, extensible news aggregation and personalization platform built with **ASP.NET Core (.NET 8)**. It provides **RESTful APIs** for user authentication, news management, personalized feeds, notifications, reporting, and more.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## 🚀 Features

- **User Authentication** (JWT-based)
- **News Feed** – Paginated, searchable, and personalized news
- **Categories & Keywords** – Organize and filter news
- **Save, Like, Dislike News** – User engagement features
- **News Source Management** – Add/edit news sources
- **Notifications** – User-specific notifications
- **Reporting** – Report inappropriate news
- **Subscriptions** – Subscribe to categories/keywords

---

## 🛠 Tech Stack

- **Backend:** ASP.NET Core (.NET 8)
- **ORM:** Entity Framework Core (SQL Server)
- **Authentication:** JWT Bearer
- **API Docs:** Swagger (OpenAPI 3.0)
- **Mapping:** AutoMapper

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### Setup

1️⃣ **Clone the repository**
```
git clone https://github.com/your-org/khabri.git
cd khabri
```

2️⃣ **Configure the database**
- Update the `DefaultConnection` string in `appsettings.json` with your SQL Server details.

3️⃣ **Apply migrations**
```
dotnet ef database update
```

4️⃣ **Run the API**
```
dotnet run
```
The API will be available at `https://localhost:5001` (or as configured).

5️⃣ **Access Swagger UI**
- Navigate to `https://localhost:5001/swagger` for interactive API documentation.

---

## ⚙️ Configuration

- **JWT Settings** – Set in `appsettings.json` under the `Jwt` section.
- **Database** – Set `DefaultConnection` in `appsettings.json`.
- **CORS** – Configured to allow all origins by default (adjust as needed).

Example `appsettings.json` snippet:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=KhabriDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
},
"Jwt": {
  "Key": "YOUR_SECRET_KEY",
  "Issuer": "KhabriAPI",
  "Audience": "KhabriUsers",
  "ExpireMinutes": 60
}
```

---

## 📘 API Documentation

The API follows RESTful conventions and is documented via Swagger/OpenAPI.

### Authentication
- Obtain a JWT token via `POST /api/auth/login`.
- Include the token in the header:  
  ```
  Authorization: Bearer <token>
  ```

### Main Endpoints

| Resource          | Endpoint Example                 | Description                        |
|-------------------|----------------------------------|------------------------------------|
| Auth              | `POST /api/auth/login`           | User login                         |
| User              | `POST /api/user/signup`          | User registration                  |
| News              | `GET /api/news/page`             | Paginated news                     |
| News              | `GET /api/news/search`           | Search news                        |
| News              | `GET /api/news/personalized`     | Personalized news feed             |
| News              | `POST /api/news/save`            | Save news for user                 |
| News Like/Dislike | `POST /api/newslikedislike/like` | Like a news item                   |
| News Source       | `POST /api/newssource/add`       | Add a news source                  |
| Notification      | `GET /api/notification/user`     | Get user notifications             |
| Report            | `POST /api/report/news`          | Report a news item                 |

For a full list, see the [Swagger UI](https://localhost:5001/swagger).

---

## 📂 Project Structure

```
Khabri.API/
 ├── Controllers/          # API controllers
 ├── Data/                 # Database context and migrations
 ├── Models/               # Entity models
 ├── DTOs/                 # Data Transfer Objects
 ├── Services/             # Business logic
 ├── Helpers/              # Utilities (e.g., JWT generator)
 ├── appsettings.json       # Configuration
 └── Program.cs / Startup.cs # App entry and configuration
```

---

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

1️⃣ Fork the repository  
2️⃣ Create a new branch  
```
git checkout -b feature/your-feature
```  
3️⃣ Commit your changes  
4️⃣ Push to your branch  
```
git push origin feature/your-feature
```  
5️⃣ Open a pull request

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 📬 Contact

For questions or support, please open an issue or contact the maintainers.

---
