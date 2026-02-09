# ACIG PDF Template System

A complete runtime-driven PDF template system built with .NET 10 Web API and Next.js frontend, featuring runtime schema discovery, dynamic field trees, Handlebars templating, and PuppeteerSharp PDF generation.

## 🏗️ Architecture

The application is structured as a .NET solution with the following projects:

```
TestPDFGenerator/
├── src/
│   ├── PdfTemplateSystem.Api/    # .NET 10 Web API backend
│   └── web/                        # Next.js frontend application
├── frontend/                       # Legacy HTML frontend (deprecated)
├── TestPDFGenerator.sln           # Solution file
├── build.sh / build.bat           # Build scripts
└── run-dev.sh                     # Development startup script
```

## 🌟 Features

### Runtime-Driven Architecture
- **Schema Discovery at Runtime** - Uses EF Core `IModel` introspection to discover database schema
- **No Compile-Time DTOs** - All field trees generated dynamically from database schema
- **Context Profiles** - Persisted in database to define what fields/navigations are exposed per template context
- **Runtime Custom Joins** - Define joins between tables at runtime without EF navigation properties
- **Admin Endpoints** - Access raw metadata; public endpoints only see shaped data
- **Comprehensive Caching** - Schema, field trees, compiled Handlebars templates, and browser pool

### Backend (.NET 10 Web API)
- **Models**:
  - `PdfTemplate` - Stores template definitions with Handlebars syntax
  - `ContextProfile` - Defines field exposure and labeling per context
  - `SampleInvoice` & `SampleInvoiceItem` - Demo entities

- **Services**:
  - `SchemaDiscoveryService` - Runtime EF Core schema introspection
  - `TemplateEngineService` - Handlebars compilation and data shaping
  - `PdfGenerationService` - PuppeteerSharp browser pool management
  - `HybridContextDataFetcher` - Handles both EF relationships and custom joins
  - `DynamicSqlQueryBuilder` - Safe, parameterized SQL generation for custom joins
  - `CustomJoinValidator` - Validates custom join configurations
  - `ResultShaper` - Converts flat Dapper results to nested objects
  - `DataSeeder` - Initial data population

- **Controllers**:
  - `SchemaController` - Field trees and raw metadata endpoints
  - `TemplatesController` - CRUD operations for templates
  - `ContextProfilesController` - CRUD for context profiles
  - `PdfController` - PDF generation and HTML preview
  - `AdminController` - Schema introspection and join validation

### Frontend (Next.js with TypeScript & Tailwind CSS)
- **ACIG-Branded UI** - Professional blue navigation, modern design
- **Pages**:
  - Dashboard - Overview with statistics and quick actions
  - Templates - Template management with inline editor
  - Generate PDF - PDF generation with live preview
  - Schema Discovery - Dynamic field tree visualization
- **Features**:
  - RTL support for Arabic content
  - Responsive design
  - API integration with error handling
  - Real-time PDF preview
  - Template editor with syntax highlighting

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js 18+ and npm
- Modern web browser

### Quick Start

#### Option 1: Use the Startup Script (Recommended)

**Linux/Mac:**
```bash
./run-dev.sh
```

This will start both the backend API on http://localhost:5000 and the frontend on http://localhost:3000.

#### Option 2: Manual Setup

**1. Clone the repository**
```bash
git clone <repository-url>
cd TestPDFGenerator-
```

**2. Build everything**
```bash
# Linux/Mac
./build.sh

# Windows
build.bat
```

**3. Start the backend**
```bash
cd src/PdfTemplateSystem.Api
dotnet run --urls "http://localhost:5000"
```

The API will be available at: http://localhost:5000  
Swagger documentation at: http://localhost:5000/swagger

**4. Start the frontend** (in a separate terminal)
```bash
cd src/web
npm install
npm run dev
```

The frontend will be available at: http://localhost:3000

### Production Build

**Build backend:**
```bash
cd src/PdfTemplateSystem.Api
dotnet build -c Release
```

**Build frontend:**
```bash
cd src/web
npm run build
npm start
```

## 📋 API Endpoints

### Schema Discovery
- `GET /api/Schema/field-tree/{contextName}` - Get dynamically generated field tree for a context
- `GET /api/Schema/metadata/{entityName}` - Get raw EF Core metadata (admin)

### Templates
- `GET /api/Templates` - List all templates
- `GET /api/Templates/{id}` - Get template by ID
- `GET /api/Templates/by-context/{contextName}` - Get templates for a context
- `POST /api/Templates` - Create new template
- `PUT /api/Templates/{id}` - Update template
- `DELETE /api/Templates/{id}` - Delete template

### Context Profiles
- `GET /api/ContextProfiles` - List all profiles
- `GET /api/ContextProfiles/{id}` - Get profile by ID
- `GET /api/ContextProfiles/by-name/{contextName}` - Get profile by context name
- `POST /api/ContextProfiles` - Create new profile
- `PUT /api/ContextProfiles/{id}` - Update profile
- `DELETE /api/ContextProfiles/{id}` - Delete profile

### PDF Generation
- `GET /api/Pdf/generate/{contextName}/{entityId}` - Generate and download PDF
- `GET /api/Pdf/preview/{contextName}/{entityId}` - Preview HTML before PDF generation
- `POST /api/Pdf/generate-from-html` - Generate PDF from custom HTML

## 💡 Usage Examples

### Generate a PDF
```bash
curl -o invoice.pdf http://localhost:5000/api/Pdf/generate/invoice/11111111-1111-1111-1111-111111111111
```

### Get Field Tree for Invoice Context
```bash
curl http://localhost:5000/api/Schema/field-tree/invoice
```

### Preview HTML
```bash
curl http://localhost:5000/api/Pdf/preview/invoice/11111111-1111-1111-1111-111111111111
```

## 🎨 Template Syntax

Templates use Handlebars syntax. Example:

```html
<!DOCTYPE html>
<html dir="rtl" lang="ar">
<head>
    <meta charset="UTF-8">
    <style>
        body { font-family: Arial; direction: rtl; }
        table { width: 100%; border-collapse: collapse; }
        th, td { border: 1px solid #ddd; padding: 12px; }
    </style>
</head>
<body>
    <h1>فاتورة رقم {{Number}}</h1>
    <p>العميل: {{CustomerName}}</p>
    <p>التاريخ: {{Date}}</p>
    <table>
        <thead>
            <tr>
                <th>الوصف</th>
                <th>الكمية</th>
                <th>السعر</th>
            </tr>
        </thead>
        <tbody>
            {{#each Items}}
            <tr>
                <td>{{Description}}</td>
                <td>{{Quantity}}</td>
                <td>{{UnitPrice}}</td>
            </tr>
            {{/each}}
        </tbody>
    </table>
</body>
</html>
```

## 🔧 Configuration

### Database
The system uses an in-memory database by default. To use a persistent database, update `Program.cs`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### CORS
CORS is configured to allow all origins in development. Update `Program.cs` for production:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("https://your-frontend-domain.com")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

## 📦 NuGet Packages

- Microsoft.EntityFrameworkCore (8.0.0)
- Microsoft.EntityFrameworkCore.InMemory (8.0.0)
- Microsoft.EntityFrameworkCore.Relational (8.0.0)
- Microsoft.EntityFrameworkCore.Design (8.0.0)
- Microsoft.Data.Sqlite (8.0.0)
- Dapper (2.1.35)
- System.Data.SqlClient (4.8.6)
- PuppeteerSharp (19.0.0)
- Handlebars.Net (2.1.6)
- Swashbuckle.AspNetCore (10.1.2)

## 🏗️ Architecture Highlights

### Runtime Schema Discovery
The system introspects the EF Core model at runtime to build dynamic field trees. No compile-time DTOs are needed.

### Context Profiles
Define what fields are exposed for each context:
```json
{
  "contextName": "invoice",
  "rootEntity": "SampleInvoice",
  "includePaths": ["Items"],
  "allowedFields": ["Number", "Date", "CustomerName", "Items.Description", "Items.Quantity", "Items.UnitPrice"],
  "labels": {
    "Number": "رقم الفاتورة",
    "Date": "التاريخ",
    "CustomerName": "اسم العميل"
  }
}
```

### Runtime Custom Joins

The system supports defining custom joins at runtime without requiring EF Core navigation properties. This is useful for:
- Joining tables that don't have foreign key relationships
- Integrating with legacy databases
- Creating dynamic relationships based on non-standard fields

#### Example: Custom Join Configuration

```json
{
  "contextName": "invoice",
  "rootEntity": "SampleInvoice",
  "includePaths": ["Items"],
  "customJoins": [
    {
      "alias": "ExternalCustomer",
      "targetEntity": "Customer",
      "joinType": "LEFT",
      "condition": {
        "leftField": "CustomerCode",
        "rightField": "ExternalId",
        "operator": "="
      },
      "nestedJoins": []
    }
  ],
  "allowedFields": [
    "Number",
    "Date",
    "CustomerCode",
    "ExternalCustomer.Name",
    "ExternalCustomer.Email",
    "ExternalCustomer.Phone",
    "Items.Description",
    "Items.Quantity"
  ]
}
```

#### Generated SQL
The system generates safe, parameterized SQL queries:

```sql
SELECT
    root.[Id] AS root_Id,
    root.[Number] AS Number,
    root.[Date] AS Date,
    root.[CustomerCode] AS CustomerCode,
    ExternalCustomer.[Name] AS ExternalCustomer_Name,
    ExternalCustomer.[Email] AS ExternalCustomer_Email,
    ExternalCustomer.[Phone] AS ExternalCustomer_Phone,
    Items.[Description] AS Items_Description,
    Items.[Quantity] AS Items_Quantity
FROM SampleInvoices AS root
LEFT JOIN SampleInvoiceItems AS Items ON root.[Id] = Items.[SampleInvoiceId]
LEFT JOIN Customers AS ExternalCustomer 
    ON root.[CustomerCode] = ExternalCustomer.[ExternalId]
WHERE root.[Id] = @Id
```

#### Security Features
- **SQL Injection Prevention**: All identifiers validated with regex `^[a-zA-Z_][a-zA-Z0-9_]*$`
- **Operator Whitelisting**: Only `=`, `!=`, `>`, `<`, `>=`, `<=`, `LIKE` allowed
- **Parameterized Queries**: All values passed as parameters
- **Identifier Escaping**: All identifiers escaped with `[brackets]`
- **Join Depth Limit**: Maximum 3 levels of nested joins
- **Join Count Limit**: Maximum 10 joins per context

#### Admin Endpoints for Custom Joins

```bash
# Get available entities
GET /api/admin/schema/entities

# Get fields for an entity
GET /api/admin/schema/entities/{entityName}/fields

# Validate join configuration
POST /api/admin/validate-joins
{
  "contextName": "invoice",
  "joins": [...]
}

# Refresh schema cache
POST /api/admin/refresh-schema
```

### Data Shaping
Data is automatically shaped based on the context profile before being passed to the template engine, ensuring only allowed fields are exposed.

### Caching Strategy
- Schema metadata: 1 hour
- Field trees: 1 hour
- Compiled templates: 1 hour
- Shaped data: 5 minutes
- Browser pool: Persistent singleton

## 📝 Sample Data

The system comes pre-seeded with:
- 2 Sample Invoices (with Arabic text)
- 1 Context Profile (invoice)
- 1 PDF Template (Arabic Invoice Template)

## 🤝 Contributing

Contributions are welcome! Please follow these steps:
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🙋 Support

For issues and questions, please open an issue on GitHub.

---

**Built with ❤️ using .NET 10, EF Core, Next.js, React, and TypeScript**
