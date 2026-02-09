# TestPDFGenerator - .NET Solution

A complete runtime-driven PDF template system with a modern Next.js frontend. This solution provides schema discovery, dynamic field trees, Handlebars templating, PuppeteerSharp PDF generation, and an ACIG-branded claims management interface.

## 🏗️ Solution Structure

```
TestPDFGenerator/
├── TestPDFGenerator.sln          # Root solution file
├── src/
│   ├── Api/                       # ASP.NET Core Web API
│   │   ├── Controllers/           # API controllers
│   │   ├── Models/                # Domain models
│   │   ├── Services/              # Business logic services
│   │   ├── Data/                  # Data context
│   │   └── Program.cs             # API entry point
│   └── Web/                       # Next.js frontend
│       ├── app/                   # Next.js app router
│       ├── components/            # React components
│       ├── lib/                   # Utilities and theme
│       └── package.json           # Frontend dependencies
├── frontend/                      # Legacy frontend (deprecated)
└── README.md
```

## 🌟 Features

### Backend API (.NET 10.0)
- **Runtime Schema Discovery** - EF Core introspection for dynamic field trees
- **Context Profiles** - Persistent field exposure configuration
- **Runtime Custom Joins** - Dynamic table joins without EF navigation
- **Handlebars Templates** - Flexible HTML templating
- **PDF Generation** - PuppeteerSharp-powered PDF creation
- **Comprehensive Caching** - Schema, templates, and browser pooling

### Frontend (Next.js 16 + TypeScript)
- **ACIG Branding** - Deep blue navigation, modern interface
- **Claims Dashboard** - Interactive data table with filters
- **RTL Support** - Arabic/English text support
- **Responsive Design** - Mobile-friendly layout
- **TypeScript** - Type-safe development
- **Tailwind CSS v4** - Modern styling framework

## 🚀 Getting Started

### Prerequisites
- .NET 10.0 SDK
- Node.js 18+ and npm
- Modern web browser

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd TestPDFGenerator-
```

2. **Build the solution**
```bash
dotnet restore
dotnet build TestPDFGenerator.sln
```

3. **Run the backend API**
```bash
cd src/Api
dotnet run
```

The API will be available at: http://localhost:5000
Swagger documentation at: http://localhost:5000/swagger

4. **Install and run the frontend**

In a separate terminal:

```bash
cd src/Web
npm install
npm run dev
```

The Next.js app will be available at: http://localhost:3000

### Building for Production

**Backend:**
```bash
dotnet build -c Release TestPDFGenerator.sln
cd src/Api
dotnet publish -c Release -o ./publish
```

**Frontend:**
```bash
cd src/Web
npm run build
npm start
```

## 📋 API Endpoints

### Schema Discovery
- `GET /api/Schema/field-tree/{contextName}` - Get field tree for context
- `GET /api/Schema/metadata/{entityName}` - Get raw EF Core metadata

### Templates
- `GET /api/Templates` - List all templates
- `POST /api/Templates` - Create new template
- `PUT /api/Templates/{id}` - Update template
- `DELETE /api/Templates/{id}` - Delete template

### Context Profiles
- `GET /api/ContextProfiles` - List all profiles
- `POST /api/ContextProfiles` - Create profile
- `PUT /api/ContextProfiles/{id}` - Update profile

### PDF Generation
- `GET /api/Pdf/generate/{contextName}/{entityId}` - Generate PDF
- `GET /api/Pdf/preview/{contextName}/{entityId}` - Preview HTML
- `POST /api/Pdf/generate-from-html` - Generate PDF from custom HTML

### Admin
- `GET /api/admin/schema/entities` - Get available entities
- `GET /api/admin/schema/entities/{entityName}/fields` - Get entity fields
- `POST /api/admin/validate-joins` - Validate join configuration
- `POST /api/admin/refresh-schema` - Refresh schema cache

## 🎨 Frontend Components

### TopNav
ACIG-branded navigation bar with deep blue background (#1e3a8a)

### Breadcrumb
Navigation path indicator for current page location

### DataTable
Interactive table with:
- Column filtering (status, type)
- Search functionality
- Pagination
- Row hover effects
- Status badges with color coding

### Theme System
Centralized theme configuration in `lib/theme.ts`:
- ACIG brand colors
- Typography scales
- Spacing system
- Shadow definitions
- RTL/LTR support

## 💾 Sample Data

The system comes with sample claims data for demonstration:
- 8 sample claims with various statuses
- Support for English and Arabic names
- Multiple claim types (Medical, Vehicle, Property)
- Status tracking (Pending, Approved, Rejected, Under Review)

## 📦 Dependencies

### Backend
- Microsoft.EntityFrameworkCore 8.0.0
- PuppeteerSharp 19.0.0
- Handlebars.Net 2.1.6
- Dapper 2.1.35
- Swashbuckle.AspNetCore 10.1.2

### Frontend
- Next.js 16.1.6
- React 19.2.3
- TypeScript 5.x
- Tailwind CSS 4.x

## 🔧 Configuration

### Backend CORS
Update `Program.cs` to configure CORS for your frontend domain:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

### Database
Default: In-memory database. To use SQL Server:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Frontend API URL
Update API calls in components to point to your backend URL. Default: http://localhost:5000

## 📝 Development Scripts

### Backend
```bash
# Build
dotnet build

# Run with hot reload
dotnet watch run

# Run tests (if applicable)
dotnet test
```

### Frontend
```bash
# Development server
npm run dev

# Build for production
npm run build

# Start production server
npm start

# Lint code
npm run lint
```

## 🏗️ Architecture

### Backend Pattern
- **Controllers**: Handle HTTP requests
- **Services**: Business logic and data processing
- **Models**: Domain entities
- **Data**: EF Core DbContext

### Frontend Pattern
- **App Router**: Next.js 13+ routing
- **Components**: Reusable UI components
- **Lib**: Utilities, theme, and data
- **Inline Styles**: Theme-based styling system

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 License

MIT License

## 🙋 Support

For issues and questions, please open an issue on GitHub.

---

**Built with ❤️ using .NET 10, Next.js 16, React 19, and TypeScript**
