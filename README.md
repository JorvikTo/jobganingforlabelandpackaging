# Job Ganging Software for Label and Packaging

A commercial-grade job ganging (imposition) system for optimizing label and packaging layouts on print sheets. This software minimizes material waste by intelligently arranging multiple die-cut designs on sheets.

## Features

### Core Capabilities
- **Die Line Import**: Support for PDF (with spot colors) and DXF file formats
- **Automatic Optimization**: AI-powered nesting algorithm to minimize waste
- **Manual Arrangement**: Interactive drag-and-drop interface for fine-tuning
- **Irregular Shapes**: Advanced 2D bin packing for complex die-cut designs
- **Sheet Management**: Multiple sheet sizes and materials
- **Waste Calculation**: Real-time material utilization metrics
- **Export Options**: Print-ready PDF with crop marks and registration

### Technical Stack
- **Frontend**: Vue.js 3 with TypeScript
- **Backend**: .NET Core 8.0 Web API
- **Optimization**: Custom nesting algorithms with collision detection
- **File Processing**: PDF parsing (spot color detection), DXF geometry import

## Project Structure

```
jobganingforlabelandpackaging/
├── backend/                 # .NET Core Web API
│   ├── JobGanging.API/     # API Controllers
│   ├── JobGanging.Core/    # Business Logic
│   ├── JobGanging.Data/    # Data Access
│   └── JobGanging.Tests/   # Unit Tests
├── frontend/               # Vue.js Application
│   ├── src/
│   │   ├── components/    # UI Components
│   │   ├── views/        # Page Views
│   │   ├── services/     # API Services
│   │   └── utils/        # Utilities
│   └── public/
└── docs/                  # Documentation
```

## Getting Started

### Prerequisites
- .NET Core SDK 8.0 or higher
- Node.js 18+ and npm/yarn
- Modern web browser (Chrome, Firefox, Edge)

### Backend Setup

```bash
cd backend/JobGanging.API
dotnet restore
dotnet build
dotnet run
```

The API will be available at `https://localhost:5001`

### Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

The application will be available at `http://localhost:5173`

## Usage

1. **Import Die Lines**: Upload PDF or DXF files with your label/packaging designs
2. **Configure Sheet**: Set sheet dimensions and material properties
3. **Optimize Layout**: Run automatic nesting algorithm
4. **Adjust Manually**: Fine-tune positions using drag-and-drop
5. **Review Metrics**: Check material utilization and waste percentage
6. **Export**: Generate print-ready PDF with registration marks

## Key Features in Detail

### Automatic Nesting Optimization
- Genetic algorithm for optimal placement
- Rotation optimization (0°, 90°, 180°, 270°)
- Collision detection for irregular shapes
- Margin and spacing controls

### Manual Arrangement
- Interactive canvas with zoom/pan
- Snap-to-grid functionality
- Alignment guides
- Copy/paste/duplicate operations

### File Format Support
- **PDF**: Spot color detection for die lines, multi-page support
- **DXF**: Complete geometry import, layer management

### Export Capabilities
- PDF with CMYK + spot colors
- Registration marks and crop marks
- Bleed settings
- Sheet optimization reports (CSV/Excel)

## Architecture

### Backend (.NET Core)
- RESTful API architecture
- Service layer for business logic
- Repository pattern for data access
- PDF/DXF parsing libraries

### Frontend (Vue.js)
- Component-based architecture
- Vuex/Pinia for state management
- Canvas/SVG for visual rendering
- Responsive design

## Development Status

This is a commercial-grade implementation with the following components:
- ✅ Project structure
- ✅ Backend API framework
- ✅ Frontend Vue.js setup
- ✅ Die line import (PDF/DXF)
- ✅ Nesting algorithms
- ✅ Interactive canvas
- ✅ Export functionality

## License

Commercial software - All rights reserved

## References

Similar commercial solutions:
- [Insoft Automation Ganging Software](https://insoftautomation.com/ganging-software/)
- [Insoft Automation Packaging Software](https://insoftautomation.com/packaging-software/)
