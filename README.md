## ℹ️ Description

Chao.ABP makes your ABP projects more powerful and easier to use. It adds features like millisecond-precision time, fast batch database operations, better logging, and a unified API response format—helping you build robust applications with less effort.

### 🌐 Learn More

- [GitHub Repository](https://github.com/nankingcigar/Chao.ABP.git)
- [Nanking Cigar Blog](https://nankingcigar.com)

## 🚀 Key Features

- 🕒 **DateTime JSON Numeric Return**  
  Dates are serialized as Unix timestamps (since 1970/01/01 UTC) for easy handling.
- 🚀 **SQL Server EF Bulk Operations**  
  Super-fast batch processing with Entity Framework Core.
- 📝 **Audit Log Bulk Insert**  
  Efficiently records audit logs in bulk.
- 🔄 **Navigation Change Events**  
  Navigation property change events are off by default for better performance.
- ⚡ **Optimized UpdateManyAsync**  
  Only updates entities with real changes—no wasted database calls.
- 🛠️ **Background Job Context**  
  Automatically sets tenant and user info in background jobs.
- 🛠️ **Event Bus Context**  
  Tenant and user info are auto-set for both memory，database inbox/outbox, RabbitMQ event buses.
- 🏗️ **Sub-Application Support**  
  Easily use ABP as a sub-application in your system.
- 📦 **API Response Wrapper**  
  All API responses follow a consistent, easy-to-use structure.
- 🏢 **Tenant Configuration**  
  Flexible tenant settings using JSON files.
- 💾 **WebApiClientCore 20x Response Cache**  
  Caches successful HTTP responses for faster results.
- 🧩 **Swagger Multiple Configuration**  
  Supports multiple Swagger docs for your APIs.
- 🧪 **Hangfire Unit Testing**  
  Test background jobs with SQLite and Hangfire.
- 🔐 **CAS Integration**  
  Supports both Cookie and Token authentication with CAS.


*Like plum blossoms in winter, Chao.ABP brings elegance and resilience to your ABP projects.*

## 📫 Bug Reports & Support

For questions or help, please use [GitHub Issues](https://github.com/nankingcigar/Chao.ABP/issues).
