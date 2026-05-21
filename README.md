# ShelfMaster 🏬

ShelfMaster is a full-stack warehouse inventory management system 

---

## ✨ Features

* **Inventory Tracking:** Real-time logging for item restocks and stock withdrawals.
* **Immutable Audit Ledger:** Automatically records every asset change inside a transaction history log.
* **Low-Stock Alerts:** Automatically detects when stock levels dip below a safety margin and fires a background alert.
* **Fault-Tolerant Engine:** Notification systems run in isolated catch-blocks, ensuring that a network error never crashes your core database operations.
* **A compiled Next.js frontend** served directly out of an ASP.NET Core Web API `wwwroot` folder.

---

## 📋 Requirements

* **.NET 8.0 SDK** (or later) to compile and run the application.

---

## 🚀 How to Run Locally

1. Open your terminal and navigate to the WebAPI directory:
   ```bash
    cd src/ShelfMaster.WebAPI
2.  Start the unified application engine:
    dotnet run
3. Open your browser and navigate to:
    https://localhost:7001

   
## 💡 Optional Configuration

Database Setup: If running for the very first time or resetting data,to generate the local schema, you can optionally run 
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update 
```


JWT Token Configuration: You can customize your JWT authentication keys, issuers, and token lifetimes by updating the configuration values inside appsettings.json.

Email Setup: Go to appsettings.json to configure your source email addresses, credentials, and SMTP server details. To catch these alerts locally without a live email server, have an application like Papercut SMTP running on Port 25.
    
