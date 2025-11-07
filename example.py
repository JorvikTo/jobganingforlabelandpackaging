#!/usr/bin/env python
"""
Example script demonstrating the Job Management System
"""

from job_manager import JobManager, JobStatus


def main():
    """Run a demonstration of the job management system"""
    
    print("=== Job Management System Demo ===\n")
    
    # Create a job manager
    manager = JobManager()
    
    # Create some jobs
    print("Creating jobs...")
    job1 = manager.create_job("JOB001", "Package 100 units of Product A")
    job2 = manager.create_job("JOB002", "Label 200 items of Product B")
    job3 = manager.create_job("JOB003", "Package and ship Product C")
    print(f"Created {manager.count_jobs()} jobs\n")
    
    # Add labels to jobs
    print("Adding labels...")
    job1.add_label("urgent")
    job1.add_label("fragile")
    job2.add_label("bulk_order")
    job3.add_label("priority")
    job3.add_label("international")
    print("Labels added\n")
    
    # Set packaging types
    print("Setting packaging types...")
    job1.set_packaging_type("cardboard_box")
    job2.set_packaging_type("bubble_wrap")
    job3.set_packaging_type("wooden_crate")
    print("Packaging types set\n")
    
    # Update job statuses
    print("Updating job statuses...")
    job1.update_status(JobStatus.IN_PROGRESS)
    job2.update_status(JobStatus.LABELING)
    job3.update_status(JobStatus.PENDING)
    print("Statuses updated\n")
    
    # List all jobs
    print("=== All Jobs ===")
    for job in manager.list_jobs():
        print(f"\nJob ID: {job.job_id}")
        print(f"  Description: {job.description}")
        print(f"  Status: {job.status.value}")
        print(f"  Labels: {', '.join(job.labels)}")
        print(f"  Packaging: {job.packaging_type}")
    
    # List jobs by status
    print("\n=== Pending Jobs ===")
    pending_jobs = manager.list_jobs(JobStatus.PENDING)
    print(f"Found {len(pending_jobs)} pending job(s)")
    for job in pending_jobs:
        print(f"  - {job.job_id}: {job.description}")
    
    print("\n=== In Progress Jobs ===")
    in_progress_jobs = manager.list_jobs(JobStatus.IN_PROGRESS)
    print(f"Found {len(in_progress_jobs)} in-progress job(s)")
    for job in in_progress_jobs:
        print(f"  - {job.job_id}: {job.description}")
    
    # Complete a job
    print("\n=== Completing Job JOB001 ===")
    job1.update_status(JobStatus.COMPLETED)
    print(f"Job {job1.job_id} is now {job1.status.value}")
    
    # Count jobs by status
    print("\n=== Job Statistics ===")
    print(f"Total jobs: {manager.count_jobs()}")
    print(f"Pending: {manager.count_jobs(JobStatus.PENDING)}")
    print(f"In Progress: {manager.count_jobs(JobStatus.IN_PROGRESS)}")
    print(f"Labeling: {manager.count_jobs(JobStatus.LABELING)}")
    print(f"Packaging: {manager.count_jobs(JobStatus.PACKAGING)}")
    print(f"Completed: {manager.count_jobs(JobStatus.COMPLETED)}")
    
    print("\n=== Demo Complete ===")


if __name__ == "__main__":
    main()
