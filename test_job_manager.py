"""
Unit tests for the Job Management System
"""

import pytest
from datetime import datetime
from job_manager import Job, JobManager, JobStatus


class TestJob:
    """Tests for the Job class"""
    
    def test_job_creation(self):
        """Test creating a new job"""
        job = Job("JOB001", "Package 100 units")
        
        assert job.job_id == "JOB001"
        assert job.description == "Package 100 units"
        assert job.status == JobStatus.PENDING
        assert job.labels == []
        assert job.packaging_type is None
        assert isinstance(job.created_at, datetime)
    
    def test_add_label(self):
        """Test adding labels to a job"""
        job = Job("JOB001", "Package 100 units")
        
        job.add_label("urgent")
        assert "urgent" in job.labels
        
        job.add_label("fragile")
        assert len(job.labels) == 2
        
        # Adding duplicate should not increase count
        job.add_label("urgent")
        assert len(job.labels) == 2
    
    def test_remove_label(self):
        """Test removing labels from a job"""
        job = Job("JOB001", "Package 100 units")
        
        job.add_label("urgent")
        job.add_label("fragile")
        assert len(job.labels) == 2
        
        job.remove_label("urgent")
        assert len(job.labels) == 1
        assert "fragile" in job.labels
        assert "urgent" not in job.labels
    
    def test_set_packaging_type(self):
        """Test setting packaging type"""
        job = Job("JOB001", "Package 100 units")
        
        job.set_packaging_type("cardboard_box")
        assert job.packaging_type == "cardboard_box"
    
    def test_update_status(self):
        """Test updating job status"""
        job = Job("JOB001", "Package 100 units")
        
        assert job.status == JobStatus.PENDING
        
        job.update_status(JobStatus.IN_PROGRESS)
        assert job.status == JobStatus.IN_PROGRESS
        
        job.update_status(JobStatus.COMPLETED)
        assert job.status == JobStatus.COMPLETED


class TestJobManager:
    """Tests for the JobManager class"""
    
    def test_create_job(self):
        """Test creating a job through the manager"""
        manager = JobManager()
        
        job = manager.create_job("JOB001", "Package 100 units")
        assert job.job_id == "JOB001"
        assert len(manager.jobs) == 1
    
    def test_create_duplicate_job(self):
        """Test that creating a duplicate job raises an error"""
        manager = JobManager()
        
        manager.create_job("JOB001", "Package 100 units")
        
        with pytest.raises(ValueError):
            manager.create_job("JOB001", "Another job")
    
    def test_get_job(self):
        """Test retrieving a job"""
        manager = JobManager()
        
        manager.create_job("JOB001", "Package 100 units")
        job = manager.get_job("JOB001")
        
        assert job is not None
        assert job.job_id == "JOB001"
        
        # Test non-existent job
        assert manager.get_job("JOB999") is None
    
    def test_delete_job(self):
        """Test deleting a job"""
        manager = JobManager()
        
        manager.create_job("JOB001", "Package 100 units")
        assert len(manager.jobs) == 1
        
        result = manager.delete_job("JOB001")
        assert result is True
        assert len(manager.jobs) == 0
        
        # Test deleting non-existent job
        result = manager.delete_job("JOB999")
        assert result is False
    
    def test_list_jobs(self):
        """Test listing all jobs"""
        manager = JobManager()
        
        manager.create_job("JOB001", "Package 100 units")
        manager.create_job("JOB002", "Label 200 items")
        
        jobs = manager.list_jobs()
        assert len(jobs) == 2
    
    def test_list_jobs_by_status(self):
        """Test listing jobs filtered by status"""
        manager = JobManager()
        
        job1 = manager.create_job("JOB001", "Package 100 units")
        job2 = manager.create_job("JOB002", "Label 200 items")
        job3 = manager.create_job("JOB003", "Pack and ship")
        
        job1.update_status(JobStatus.COMPLETED)
        job2.update_status(JobStatus.IN_PROGRESS)
        # job3 remains PENDING
        
        pending_jobs = manager.list_jobs(JobStatus.PENDING)
        assert len(pending_jobs) == 1
        assert pending_jobs[0].job_id == "JOB003"
        
        completed_jobs = manager.list_jobs(JobStatus.COMPLETED)
        assert len(completed_jobs) == 1
        assert completed_jobs[0].job_id == "JOB001"
    
    def test_count_jobs(self):
        """Test counting jobs"""
        manager = JobManager()
        
        assert manager.count_jobs() == 0
        
        job1 = manager.create_job("JOB001", "Package 100 units")
        job2 = manager.create_job("JOB002", "Label 200 items")
        
        assert manager.count_jobs() == 2
        
        job1.update_status(JobStatus.COMPLETED)
        assert manager.count_jobs(JobStatus.COMPLETED) == 1
        assert manager.count_jobs(JobStatus.PENDING) == 1
