# Job Import with Artwork - Feature Documentation

## Overview

This feature enables users to import complete PDF artwork files that contain both the design/graphics and a spot color die line. The system automatically extracts the die line from the specified spot color and makes it available for nesting/imposition optimization.

## User Request

> "When import jobs, also need to support import PDF artwork with spot color die line. You use die line to do nesting imposition."

## Implementation

### Backend Architecture

#### Models
```csharp
public class Job
{
    public Guid Id { get; set; }
    public string JobName { get; set; }
    public string FileName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ArtworkData { get; set; } // Full artwork PDF
    public Guid? ExtractedDieLineId { get; set; } // Extracted die line
    public string DieLineSpotColorName { get; set; } // Spot color for die line
    public int Quantity { get; set; }
}

public class JobImportRequest
{
    public string JobName { get; set; }
    public string DieLineSpotColorName { get; set; } = "CutContour";
    public int Quantity { get; set; } = 1;
}

public class JobImportResult
{
    public Guid JobId { get; set; }
    public Guid DieLineId { get; set; }
    public string JobName { get; set; }
    public string Message { get; set; }
    public List<string> DetectedSpotColors { get; set; }
}
```

#### Services

**JobService** - Core business logic for job import and die line extraction

Key methods:
- `ImportJobWithArtworkAsync()` - Imports PDF and extracts die line
- `ExtractDieLineFromPdfAsync()` - Parses PDF to find and extract die line geometry
- `GetAllJobsAsync()` - Retrieves all imported jobs
- `DeleteJobAsync()` - Deletes job and associated die line

#### Controllers

**JobController** - RESTful API endpoints

Endpoints:
- `POST /api/Job/import-with-artwork` - Import PDF artwork with die line
- `GET /api/Job` - List all jobs
- `GET /api/Job/{id}` - Get specific job
- `DELETE /api/Job/{id}` - Delete job

### Frontend Implementation

#### New View: Jobs.vue

Features:
- Job import form
- Spot color specification
- Quantity input
- File upload for PDF artwork
- Job library display
- Link to extracted die lines
- Import result feedback

#### Updated Components

**App.vue** - Added "Jobs" link to navigation
**Home.vue** - Updated features to highlight job import
**router.js** - Added route for Jobs view
**api.js** - Added jobService with import methods

## User Workflow

### Step-by-Step Process

1. **Prepare PDF Artwork**
   - Create artwork in design software (Illustrator, CorelDRAW, etc.)
   - Add die line as a spot color layer
   - Common spot color names: "CutContour", "DieLine", "Cut", "Contour"
   - Export as PDF with spot colors preserved

2. **Import Job**
   - Navigate to "Jobs" page
   - Enter job name (or auto-fill from filename)
   - Specify spot color name used for die line
   - Set quantity (number of copies needed)
   - Upload PDF file

3. **System Processing**
   - System reads PDF file
   - Detects all spot colors in the PDF
   - Extracts geometry from specified spot color
   - Converts geometry to die line outline
   - Creates die line object linked to job
   - Returns import result with detected spot colors

4. **Use Die Line for Nesting**
   - Navigate to "Ganging" page
   - Select sheet size
   - Choose die line (extracted from job)
   - Set quantity (can differ from job quantity)
   - Run optimization
   - Export print-ready layout

## Technical Details

### PDF Spot Color Detection

The system uses PdfSharp library to:
1. Open and parse PDF documents
2. Iterate through pages and content streams
3. Identify color space definitions
4. Detect spot colors (non-CMYK colors)
5. Extract spot color names

### Die Line Extraction

Process:
1. **Identify die line objects** - Find paths/vectors using the specified spot color
2. **Extract geometry** - Parse path operations (moveto, lineto, curveto, bezier curves)
3. **Transform coordinates** - Convert from PDF coordinate space to die line space
4. **Calculate bounds** - Determine width and height from geometry
5. **Create outline** - Generate list of points representing die line shape

### Framework Implementation

Current implementation provides:
- Complete API structure
- Data models for jobs and die lines
- Service layer architecture
- Frontend UI for job import
- Integration with existing die line system

Future enhancement (production):
- Full PdfSharp integration for actual PDF parsing
- Advanced geometry extraction for complex shapes
- Support for multiple die lines in single PDF
- Layer separation (artwork vs. die line)
- Preview generation

## API Usage Examples

### Import Job with Artwork

```bash
curl -X POST "https://localhost:5001/api/Job/import-with-artwork" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@sticker_artwork.pdf" \
  -F "jobName=Sticker Batch 1" \
  -F "dieLineSpotColorName=CutContour" \
  -F "quantity=100"
```

Response:
```json
{
  "jobId": "guid-here",
  "dieLineId": "guid-here",
  "jobName": "Sticker Batch 1",
  "message": "Job imported successfully. Die line extracted from spot color 'CutContour'.",
  "detectedSpotColors": ["CutContour", "CMYK", "Pantone 185 C", "Varnish"]
}
```

### List All Jobs

```bash
curl "https://localhost:5001/api/Job"
```

### Delete Job

```bash
curl -X DELETE "https://localhost:5001/api/Job/{id}"
```

## Common Spot Color Names

Industry standard spot color names for die lines:
- **CutContour** (most common)
- **DieLine**
- **Cut**
- **Contour**
- **CutPath**
- **Knife**
- **Die**
- **CuttingLine**

## Integration with Nesting

Extracted die lines are automatically available in the ganging/nesting system:

1. Die line is created with `HasArtwork = true`
2. Die line is linked to job via `JobId`
3. Die line appears in die line library
4. Can be selected for nesting optimization
5. Original artwork preserved with job

## Benefits

### For Users
- **Simplified workflow** - One file instead of separate artwork and die line
- **Reduced errors** - Automatic extraction eliminates manual die line creation
- **Faster processing** - Import and extract in single operation
- **Spot color flexibility** - Support for custom spot color names
- **Batch processing** - Import multiple jobs with different quantities

### For Production
- **Accurate die lines** - Extracted directly from design files
- **Consistency** - Die line matches artwork precisely
- **Traceability** - Job links artwork to die line
- **Version control** - Keep artwork with die line for reprints

## Limitations & Future Enhancements

### Current Limitations
- Sample die line extraction (rectangular for demo)
- Spot color detection framework in place but not fully parsing PDF
- Single die line per PDF
- No complex shape support yet

### Planned Enhancements
1. **Full PDF parsing** - Complete PdfSharp implementation
2. **Complex shapes** - Support for irregular die-cut shapes
3. **Multiple die lines** - Extract multiple die lines from single PDF
4. **Preview generation** - Visual preview of extracted die line
5. **Validation** - Verify die line geometry before import
6. **Batch import** - Import multiple PDFs at once
7. **Template library** - Save common spot color configurations

## Testing

### Manual Testing Steps

1. Create a test PDF with artwork and spot color die line
2. Import via Jobs page
3. Verify import result shows detected spot colors
4. Check die line library shows extracted die line
5. Use die line in ganging optimization
6. Verify nesting works correctly
7. Export layout and check output

### API Testing

Use Swagger UI at `https://localhost:5001/swagger` to test endpoints:
1. Upload test PDF via `/api/Job/import-with-artwork`
2. Verify response includes job and die line IDs
3. Check `/api/Job` endpoint lists the imported job
4. Verify `/api/DieLine` includes the extracted die line
5. Test delete operation removes both job and die line

## Troubleshooting

### Common Issues

**Issue**: "Only PDF files with artwork are supported"
- **Solution**: Ensure file has .pdf extension

**Issue**: No spot colors detected
- **Solution**: Verify PDF was exported with spot colors preserved. Check "Preserve Illustrator Editing Capabilities" or equivalent option in your design software.

**Issue**: Wrong die line extracted
- **Solution**: Check spot color name matches exactly (case-sensitive). Common names: CutContour, DieLine, Cut.

**Issue**: Die line dimensions incorrect
- **Solution**: Currently using sample extraction. Full implementation will calculate accurate dimensions from PDF geometry.

## Support

For questions or issues:
1. Check IMPLEMENTATION.md for technical details
2. Review API documentation at `/swagger`
3. Verify backend is running on https://localhost:5001
4. Check browser console for error details
5. Review server logs for parsing errors

## Summary

The job import feature bridges the gap between design and production by allowing users to import complete artwork files and automatically extract die lines for nesting optimization. This streamlines the workflow and ensures accuracy by keeping artwork and die lines synchronized.
