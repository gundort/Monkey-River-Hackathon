# 🏞️ Team 6 - Monkey & River Hackathon 2025

## 🚀 ASP.NET Core API with MongoDB

This is the backend API for **Travel Risk Monitoring** — our submission for the Monkey & River Hackathon 2025.  
It is built with **ASP.NET Core**, integrated with **MongoDB Atlas**, and secured with **JWT Authentication**.

---

## ✨ Features

✅ ASP.NET Core 8 Web API  
✅ JWT Authentication (Login & Register)  
✅ MongoDB Atlas Integration  
✅ Protected Routes  
✅ Health Check Endpoint (`/api/health`)  
✅ Modular Service Layer  
✅ Unit Tests with xUnit & Moq  
✅ Swagger API Documentation

---
## 🌐 Website 
👉 https://hackathon-group-6.netlify.app/
## 🌐 Swagger Endpoint

📄 **Swagger UI**:  
👉 [https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/swagger/index.html](https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/swagger/index.html)

---

## 🛠 Getting Started

### ✅ Prerequisites
- [.NET SDK 8+](https://dotnet.microsoft.com/en-us/download)  
- [MongoDB Atlas](https://www.mongodb.com/atlas/database) account **or** local MongoDB instance  

---

### 📂 Setup & Configuration

1️⃣ **Clone the repository**
```bash
git clone https://github.com/Thandeka-Lubisi1/team6-monkey-river-hackathon-2025.git
cd team6-monkey-river-hackathon-2025/backend

\## Running the Test Suite


This project uses xUnit and Moq for unit testing.


\### Run All Tests

Use the following command in the root of the test project:


```bash

dotnet test


\### Run tests with coverage:

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover