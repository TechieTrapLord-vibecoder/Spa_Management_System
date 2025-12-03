# Spa Sync API

Cloud sync API for the Spa Management System. This API enables offline-first data synchronization between the MAUI desktop application and a cloud SQL Server database.

## Features

- **API Key Authentication**: Secure endpoint access using X-API-Key header
- **Entity Sync**: Upload/download for all major entities (Customers, Employees, Services, etc.)
- **Version-based Conflict Resolution**: Uses sync_version for last-write-wins with version check
- **Device Registration**: Track sync clients
- **Health Checks**: Built-in health endpoint for monitoring

## Deployment to MonsterASP.net

### 1. Create SQL Server Database

In your MonsterASP.net control panel:

1. Go to Databases > SQL Server
2. Create a new database (e.g., `SpaSync`)
3. Note the connection string provided

### 2. Configure appsettings.json

Update the connection string and API key:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sql.monsterasp.net;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
  },
  "ApiKey": "YOUR_SECURE_API_KEY_HERE"
}
```

**Important**: Generate a strong API key (32+ characters, mix of letters/numbers/symbols)

### 3. Publish the Application

```powershell
dotnet publish -c Release -o ./publish
```

### 4. Upload to MonsterASP.net

1. Go to your hosting control panel
2. Navigate to File Manager or use FTP
3. Upload the contents of the `./publish` folder to your web root
4. Ensure the application pool is set to .NET 8.0 or later

### 5. Configure the MAUI App

In the Spa Management System:

1. Go to Settings > Cloud Sync
2. Enter your API URL: `https://your-domain.monsterasp.net`
3. Enter the API Key you configured
4. Test the connection

## API Endpoints

| Method | Endpoint                          | Description                         |
| ------ | --------------------------------- | ----------------------------------- |
| GET    | `/api/sync/status`                | Check API status (no auth required) |
| POST   | `/api/sync/register-device`       | Register a sync device              |
| POST   | `/api/sync/upload/{entityType}`   | Upload records to cloud             |
| GET    | `/api/sync/download/{entityType}` | Download records from cloud         |
| GET    | `/api/sync/download-all`          | Bulk download all entities          |
| GET    | `/health`                         | Health check endpoint               |

### Entity Types

- `persons`, `customers`, `employees`, `services`, `products`
- `appointments`, `sales`, `expenses`, `inventories`
- `payrolls`, `journalentries`, `payments`

## Local Development

```powershell
# Run the API
dotnet run

# API will be available at https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

## Security Notes

1. **API Key**: Keep the API key secret. Don't commit it to source control.
2. **HTTPS**: Always use HTTPS in production.
3. **Connection String**: Never expose your database credentials.

## Database Schema

The API uses Entity Framework Core with automatic migrations. Tables are created with the `sync_` prefix:

- `sync_persons`, `sync_customers`, `sync_employees`, etc.
- `sync_devices` for tracking connected clients

Each entity has sync metadata:

- `SyncId` (GUID): Universal identifier across devices
- `LastModifiedAt`: When the record was last changed
- `LastSyncedAt`: When the record was last synced
- `SyncStatus`: Current sync state
- `SyncVersion`: Incrementing version for conflict resolution
