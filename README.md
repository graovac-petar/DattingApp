# DattingApp 

Live Demo
You can check out the live version of the DattingApp https://dattingappp-avdxeng3fygdh2ah.italynorth-01.azurewebsites.net/

**DattingApp** is a modern, feature-rich dating application built using **ASP.NET Core** for the backend and **Angular** for the frontend. The app provides a seamless user experience with real-time chat functionality, profile management, and matching algorithms to connect users.

## Features

- **User Authentication**: Secure registration and login using JWT (JSON Web Token) authentication.
- **Profile Management**: Users can create, edit, and update their profiles with personal information, photos, and preferences.
- **Real-time Chat**: Integrated chat functionality that allows users to communicate instantly with matches.
- **Search & Matching**: Advanced search functionality with a robust matching algorithm to find suitable matches based on user preferences.
- **Admin Dashboard**: Provides an administrative interface for managing user data and activity.
- **Responsive Design**: Optimized for mobile and desktop views.

## Tech Stack

- **Backend**: ASP.NET Core (Web API)
- **Frontend**: Angular
- **Database**: SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **Real-time Communication**: SignalR

## Getting Started

### Prerequisites

To run this application, you will need the following installed:

- [Node.js](https://nodejs.org/) (v14+)
- [Angular CLI](https://angular.io/cli)
- [.NET Core SDK](https://dotnet.microsoft.com/download)
- SQL Server

### Installation

1. **Clone the repository**:

    ```bash
    git clone https://github.com/graovac-petar/DattingApp.git
    ```

2. **Backend Setup**:

    - Navigate to the `API` directory and restore the necessary NuGet packages:

      ```bash
      cd API
      dotnet restore
      ```

    - Update the connection string in `appsettings.Development.json` to match your SQL Server configuration.

    - Apply migrations and seed the database:

      ```bash
      dotnet ef database update
      ```

    - Run the API:

      ```bash
      dotnet run
      ```

3. **Frontend Setup**:

    - Navigate to the `ClientApp` directory and install the dependencies:

      ```bash
      cd ClientApp
      npm install
      ```

    - Run the Angular development server:

      ```bash
      ng serve
      ```

    - Open your browser and navigate to `http://localhost:4200`.


## API Documentation

The API follows RESTful conventions and includes endpoints for:

- **User Authentication**: `POST /api/auth/register`, `POST /api/auth/login`
- **Profiles**: `GET /api/users`, `GET /api/users/{id}`, `PUT /api/users/{id}`
- **Messages**: `GET /api/messages`, `POST /api/messages`
- **Admin**: `GET /api/admin/users`, `DELETE /api/admin/users/{id}`

- The DattingApp is hosted on Azure and utilizes Azure SQL Server for the database.

Live Application: DattingApp
Database: Azure SQL Server
Azure services provide reliable scalability, ensuring the app performs efficiently under various workloads.

Continuous Integration
The DattingApp project utilizes GitHub Actions for Continuous Integration (CI). Every push or pull request triggers the CI pipeline to:

Run unit tests
Build the application
Deploy to Azure upon successful build
This ensures that every change is automatically tested and deployed, maintaining code quality and application stability.
