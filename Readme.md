# DynamicLogger

A modern, minimal, and dynamic logging library for ASP.NET Core and .NET applications. Easily manage and change log levels at runtime for fine-grained logging control.

## Features

- **Dynamic log level adjustment** for categories via API endpoint or code.
- **Minimal API ready** extension for fast integration.
- Configurable via swagger-exposed endpoints.
- Supports Microsoft.Extensions.Logging abstractions.

***

## Getting Started

### 1. Install via NuGet

```bash
dotnet add package DynamicLogger
```

### 2. Register Services

In your `Program.cs` or `Startup.cs`:

```csharp
// Register DynamicLogger services
builder.Services.AddLoggerServices();
```

### 3. Add DynamicLogger Endpoint

For Minimal API:

```csharp
app.MapLoggerEndpoint();
```

**Example:**  
Sets log level for the default category:

```
POST /logging/setlevel/{level}
Body: { "level": "Information" }
```

***

## Usage Example

```csharp
// Change log level for category
DynamicLogger.SetLogLevel("Default", LogLevel.Debug);
```

***

## Swagger Integration

Endpoints added via your library are auto-detected by Swagger/OpenAPI:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

app.MapLoggerEndpoint();

app.UseSwagger();
app.UseSwaggerUI();
```

***
## API Reference

Get All Log Categories

```csharp
GET /logging/categories
```
Returns all log categories currently tracked.
***

Set Log Level for 'Default' Category

```csharp
POST /logging/setlevel/{level}
```
level: Trace, Debug, Information, Warning, Error, Critical, None.
Sets log level for the 'Default' category.

***
Set Log Level for All Non-System Categories
```
POST /logging/set-nonsystem-level/{level}
```
level: Same as above
Sets log level for all non-system (application-specific) categories.

***
Set Log Level for a Specific Category
```
POST /logging/set-category-level/{category}/{level}
```
category: Log category name
level: Log level
Sets log level for a specified category.

***
Set Log Level for All Categories with Prefix
```
POST /logging/set-prefix-level/{prefix}/{level}
```
prefix: Prefix string
level: Log level
Sets log level for all categories starting with the given prefix.

***
Set Log Level with Custom Path
```
POST {path}/setlevel/{level}
```
path: Custom route prefix
level: Log level
Allows endpoint customization for setting default log level.

***
Allowed Log Levels:
Trace, Debug, Information, Warning, Error, Critical, None

***

## License

MIT

***

## Author

Created by IoJo

***
