"""
Job Management System for Label and Packaging

This module provides a simple system for managing jobs related to label printing
and packaging operations.
"""

from datetime import datetime
from enum import Enum
from typing import Dict, List, Optional


class JobStatus(Enum):
    """Status of a job in the system"""
    PENDING = "pending"
    IN_PROGRESS = "in_progress"
    LABELING = "labeling"
    PACKAGING = "packaging"
    COMPLETED = "completed"
    CANCELLED = "cancelled"


class Job:
    """Represents a job for label and packaging operations"""
    
    def __init__(self, job_id: str, description: str):
        self.job_id = job_id
        self.description = description
        self.status = JobStatus.PENDING
        self.created_at = datetime.now()
        self.updated_at = datetime.now()
        self.labels: List[str] = []
        self.packaging_type: Optional[str] = None
        
    def add_label(self, label: str) -> None:
        """Add a label to the job"""
        if label not in self.labels:
            self.labels.append(label)
            self.updated_at = datetime.now()
    
    def remove_label(self, label: str) -> None:
        """Remove a label from the job"""
        if label in self.labels:
            self.labels.remove(label)
            self.updated_at = datetime.now()
    
    def set_packaging_type(self, packaging_type: str) -> None:
        """Set the packaging type for the job"""
        self.packaging_type = packaging_type
        self.updated_at = datetime.now()
    
    def update_status(self, status: JobStatus) -> None:
        """Update the job status"""
        self.status = status
        self.updated_at = datetime.now()
    
    def __repr__(self) -> str:
        return f"Job(id={self.job_id}, status={self.status.value}, labels={self.labels})"


class JobManager:
    """Manages all jobs in the system"""
    
    def __init__(self):
        self.jobs: Dict[str, Job] = {}
    
    def create_job(self, job_id: str, description: str) -> Job:
        """Create a new job"""
        if job_id in self.jobs:
            raise ValueError(f"Job with ID {job_id} already exists")
        
        job = Job(job_id, description)
        self.jobs[job_id] = job
        return job
    
    def get_job(self, job_id: str) -> Optional[Job]:
        """Get a job by ID"""
        return self.jobs.get(job_id)
    
    def delete_job(self, job_id: str) -> bool:
        """Delete a job by ID"""
        if job_id in self.jobs:
            del self.jobs[job_id]
            return True
        return False
    
    def list_jobs(self, status: Optional[JobStatus] = None) -> List[Job]:
        """List all jobs, optionally filtered by status"""
        if status:
            return [job for job in self.jobs.values() if job.status == status]
        return list(self.jobs.values())
    
    def count_jobs(self, status: Optional[JobStatus] = None) -> int:
        """Count jobs, optionally filtered by status"""
        return len(self.list_jobs(status))
