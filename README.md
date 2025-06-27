## ℹ️ Description

Chao.ABP enhances the ABP framework by adding features such as millisecond-precision time retrieval (timezone-agnostic), integrated EF SQL Server batch operations, advanced logging, and a standardized API response shell.

### Learn More

- [GitHub Repository](https://github.com/nankingcigar/Chao.ABP.git)
- [Nanking Cigar Blog](https://nankingcigar.com)

## 🚀 Key Features

- **DateTime JSON Numeric Return**: Serializes dates as Unix timestamps (since 1970/01/01 UTC).
- **SQL Server EF Bulk Operations**: Efficient batch processing with Entity Framework.
- **Audit Log Bulk Insert**: High-performance audit logging.
- **Navigation Change Events**: `PublishEntityUpdatedEventWhenNavigationChanges` & `SaveEntityHistoryWhenNavigationChanges` default to `false`.
- **Optimized UpdateManyAsync**: Updates only entities with actual property changes, skipping unchanged or audit-only modifications.
- **Background Job Context**: Automatically sets `CurrentTenant` & `CurrentUser` in background jobs.
- **Sub-Application Support**: Use ABP as a sub-application.
- **API Response Wrapper**: Consistent API response structure.
- **Tenant Configuration**: Supports JSON-based tenant configuration.
- **WebApiClientCore 20x Response Cache**: Caches successful responses.
- **Swagger Multiple Configuration**: Supports multiple Swagger setups.
- **Hangfire Unit Testing**: SQLite-based Hangfire tests.
- **CAS Integration**: Supports both Cookie and Token authentication via CAS.

## 📫 Bug Reports & Support

For issues or support, please use [GitHub Issues](https://github.com/nankingcigar/Chao.ABP/issues).
