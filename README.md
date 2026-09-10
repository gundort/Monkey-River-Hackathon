# 🏞️ Team 6 – Monkey & River Hackathon 2025

Welcome to our submission for the **Monkey & River Hackathon 2025**.  
This project is a **Travel Risk Monitoring Platform** powered by **ASP.NET Core 8**, **MongoDB Atlas**, and **JWT Authentication**, built to monitor global travel alerts in real time.

---

## 🧪 Test Case Screenshots

### 🔐 Login & Dashboard Test

This screenshot shows a successful login followed by dashboard rendering during automated test execution.

![Login Test Screenshot](https://i.ibb.co/BV1GYzn9/image.png)

### 📊 Travel Alerts Test

This screenshot confirms travel alerts are correctly fetched and displayed in the UI during test validation.

![Travel Alerts Test Screenshot](https://i.ibb.co/svGtHBnf/screenshot.png)

---

## 🚀 Backend: ASP.NET Core API with MongoDB

### ✨ Features

- ✅ **ASP.NET Core 8 Web API**
- ✅ **JWT Authentication** (Register & Login)
- ✅ **MongoDB Atlas Integration**
- ✅ **Role-Based Protected Routes**
- ✅ **Swagger API Documentation**
- ✅ **Health Check Endpoint** (`/api/health`)
- ✅ **Modular Service & Repository Layers**
- ✅ **Unit Tests** with xUnit & Moq

---

## 🌐 Live Links

🔗 **Frontend Website**:  
👉 [https://hackathon-group-6.netlify.app](https://hackathon-group-6.netlify.app)

📄 **Swagger API Docs**:  
👉 [https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/swagger/index.html](https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/swagger/index.html)

---

## 🛠 Getting Started

### ✅ Prerequisites

- [.NET SDK 8+](https://dotnet.microsoft.com/en-us/download)
- [MongoDB Atlas](https://www.mongodb.com/atlas/database) account **or** local MongoDB instance
- Node.js (for frontend)

---

### 📦 Backend Setup

```bash
# Clone the repository
git clone https://github.com/Thandeka-Lubisi1/team6-monkey-river-hackathon-2025.git
cd team6-monkey-river-hackathon-2025/backend

# Restore dependencies and run the project
dotnet restore
dotnet run

# Navigate to frontend folder
cd ../front-end/client

# Install dependencies and start development server
npm install
npm run dev

