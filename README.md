# Job Management for Label and Packaging

A simple Python-based job management system for tracking label printing and packaging operations.

## Features

- Create and manage jobs with unique identifiers
- Track job status through different stages (pending, in progress, labeling, packaging, completed, cancelled)
- Add and manage labels for jobs
- Set packaging types for jobs
- List and filter jobs by status
- Command-line interface for easy interaction

## Installation

1. Clone the repository:
```bash
git clone https://github.com/JorvikTo/jobganingforlabelandpackaging.git
cd jobganingforlabelandpackaging
```

2. Install dependencies:
```bash
pip install -r requirements.txt
```

## Usage

### Command Line Interface

**Note:** The CLI currently creates a new JobManager instance for each command, so jobs are not persisted between commands. For a persistent session or to work with multiple jobs, use the Python API or run the example script.

Create a new job:
```bash
python cli.py create JOB001 "Package 100 units"
```

List all jobs:
```bash
python cli.py list
```

List jobs by status:
```bash
python cli.py list --status pending
```

Show job details:
```bash
python cli.py show JOB001
```

Add a label to a job:
```bash
python cli.py add-label JOB001 urgent
```

Set packaging type:
```bash
python cli.py set-packaging JOB001 cardboard_box
```

Update job status:
```bash
python cli.py update-status JOB001 in_progress
```

### Python API

```python
from job_manager import JobManager, JobStatus

# Create a job manager
manager = JobManager()

# Create a new job
job = manager.create_job("JOB001", "Package 100 units")

# Add labels
job.add_label("urgent")
job.add_label("fragile")

# Set packaging type
job.set_packaging_type("cardboard_box")

# Update status
job.update_status(JobStatus.IN_PROGRESS)

# List all jobs
all_jobs = manager.list_jobs()

# List jobs by status
pending_jobs = manager.list_jobs(JobStatus.PENDING)
```

### Example Script

Run the included example script to see a complete demonstration:
```bash
python example.py
```

This script demonstrates creating jobs, adding labels, setting packaging types, updating statuses, and querying the system.

## Testing

Run the test suite:
```bash
pytest test_job_manager.py -v
```

## Job Statuses

- `pending` - Job is created and waiting to be processed
- `in_progress` - Job is currently being worked on
- `labeling` - Job is in the labeling phase
- `packaging` - Job is in the packaging phase
- `completed` - Job has been completed
- `cancelled` - Job has been cancelled

## License

MIT License
