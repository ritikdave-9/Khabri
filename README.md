# Khabri

Khabri is a modular .NET (C#) solution for building a customizable, extensible news aggregation platform. It provides APIs for user authentication, news management, news sources, categories, keywords, notifications, reporting, and more.

---

## Table of Contents

- [About](#about)
- [Project Structure](#project-structure)
- [Features](#features)
- [API Overview](#api-overview)
- [Getting Started](#getting-started)
- [Building and Running](#building-and-running)
- [Testing](#testing)
- [Contributing](#contributing)

---

## About

Khabri is designed to serve as a backend for a news aggregator application. It provides RESTful APIs for handling users, news content, categories, keywords, user preferences, notifications, and more. The project is organized as a Visual Studio solution and follows a clean architecture, separating concerns across multiple projects.

---

## Project Structure

- `Khabri.sln`: Visual Studio solution file
- `Khabri/`: Main web/API project
- `Data/`: Data access layer
- `Service/`: Business logic and services
- `Common/`: Shared code and utilities
- `UnitTest/`: Unit test project
- `NewsApi/`, `TheNewsApi/`: Integrations for external news sources
- `Docs/`: Documentation (including Swagger/OpenAPI spec)
- `Swagger.json`: OpenAPI/Swagger definition for the API

---

## Features

- **User Authentication**: Signup/login for users.
- **News Management**: CRUD operations for news, with support for categories, keywords, and personalized feeds.
- **News Source Management**: Manage and integrate external news feeds.
- **Categories/Keywords**: Organize and filter news.
- **Like/Dislike & Save News**: User interactions.
- **Notifications**: Push notifications for updates and relevant news.
- **Reporting**: Report news items for moderation.
- **Subscriptions**: Subscribe/unsubscribe to categories/keywords for personalized news.
- **OpenAPI (Swagger) Docs**: [Swagger.json](./Swagger.json) for full API documentation.

---

## API Overview

A sample of available endpoints (see Swagger for full list):

- `POST /api/auth/login` – User login
- `POST /api/user/signup` – User registration
- `GET /api/news/page` – List paginated news
- `GET /api/news/search` – Search news by term
- `POST /api/news/save` – Save news for user
- `DELETE /api/news/saved` – Remove a saved news item
- `GET /api/news/personalized` – Personalized news for user
- `POST /api/newslikedislike/like` – Like a news item
- `POST /api/newslikedislike/dislike` – Dislike a news item
- `POST /api/notification/add` – Add notification for user
- `GET /api/notification/user` – Get user notifications
- `POST /api/report/news` – Report a news item

> For the complete API, review the [Swagger.json](./Swagger.json) or import into Swagger UI for interactive docs.

---

## Getting Started

### Prerequisites

- [.NET 7.0+ SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/vs/) or [VS Code](https://code.visualstudio.com/)
- (Optional) SQL Server or other configured data store

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/ritikdave-9/Khabri.git
    cd Khabri
    ```
2. Open `Khabri.sln` in Visual Studio.

3. Restore NuGet packages:
    ```bash
    dotnet restore
    ```

4. Update the connection string and app settings as required in `Khabri/appsettings.json`.

---

## Building and Running

From the solution root:

```bash
dotnet build
dotnet run --project Khabri/Khabri.csproj
```

The API will start (by default) on `http://localhost:5000` or `https://localhost:5001`.

---

## Testing

To run unit tests:

```bash
dotnet test UnitTest/UnitTest.csproj
```

---

## Contributing

Contributions are welcome! Please fork the repository and open a pull request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -am 'Add feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a pull request


## Contact

For questions, contact [ritikdave-9](https://github.com/ritikdave-9).

---

> _This README was generated based on the project structure and API documentation as of July 2025. Please refer to the source code and Swagger docs for the most up-to-date information._
