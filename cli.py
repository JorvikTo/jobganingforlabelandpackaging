"""
Command-line interface for the Job Management System
"""

import argparse
from job_manager import Job, JobManager, JobStatus


def main():
    """Main CLI entry point"""
    parser = argparse.ArgumentParser(
        description="Job Management System for Label and Packaging"
    )
    
    subparsers = parser.add_subparsers(dest="command", help="Available commands")
    
    # Create job command
    create_parser = subparsers.add_parser("create", help="Create a new job")
    create_parser.add_argument("job_id", help="Unique job identifier")
    create_parser.add_argument("description", help="Job description")
    
    # List jobs command
    list_parser = subparsers.add_parser("list", help="List all jobs")
    list_parser.add_argument(
        "--status",
        choices=[s.value for s in JobStatus],
        help="Filter by status"
    )
    
    # Show job command
    show_parser = subparsers.add_parser("show", help="Show job details")
    show_parser.add_argument("job_id", help="Job identifier")
    
    # Add label command
    label_parser = subparsers.add_parser("add-label", help="Add label to job")
    label_parser.add_argument("job_id", help="Job identifier")
    label_parser.add_argument("label", help="Label to add")
    
    # Set packaging command
    package_parser = subparsers.add_parser("set-packaging", help="Set packaging type")
    package_parser.add_argument("job_id", help="Job identifier")
    package_parser.add_argument("packaging_type", help="Type of packaging")
    
    # Update status command
    status_parser = subparsers.add_parser("update-status", help="Update job status")
    status_parser.add_argument("job_id", help="Job identifier")
    status_parser.add_argument(
        "status",
        choices=[s.value for s in JobStatus],
        help="New status"
    )
    
    args = parser.parse_args()
    
    # Create a job manager instance
    manager = JobManager()
    
    if args.command == "create":
        job = manager.create_job(args.job_id, args.description)
        print(f"Created job: {job.job_id}")
        print(f"Description: {job.description}")
        print(f"Status: {job.status.value}")
    
    elif args.command == "list":
        status_filter = JobStatus(args.status) if args.status else None
        jobs = manager.list_jobs(status_filter)
        
        if not jobs:
            print("No jobs found")
        else:
            print(f"Total jobs: {len(jobs)}")
            for job in jobs:
                print(f"\n{job.job_id}:")
                print(f"  Status: {job.status.value}")
                print(f"  Description: {job.description}")
                print(f"  Labels: {', '.join(job.labels) if job.labels else 'None'}")
                print(f"  Packaging: {job.packaging_type or 'Not set'}")
    
    elif args.command == "show":
        job = manager.get_job(args.job_id)
        if job:
            print(f"Job ID: {job.job_id}")
            print(f"Description: {job.description}")
            print(f"Status: {job.status.value}")
            print(f"Labels: {', '.join(job.labels) if job.labels else 'None'}")
            print(f"Packaging: {job.packaging_type or 'Not set'}")
            print(f"Created: {job.created_at}")
            print(f"Updated: {job.updated_at}")
        else:
            print(f"Job {args.job_id} not found")
    
    elif args.command == "add-label":
        job = manager.get_job(args.job_id)
        if job:
            job.add_label(args.label)
            print(f"Added label '{args.label}' to job {args.job_id}")
        else:
            print(f"Job {args.job_id} not found")
    
    elif args.command == "set-packaging":
        job = manager.get_job(args.job_id)
        if job:
            job.set_packaging_type(args.packaging_type)
            print(f"Set packaging type to '{args.packaging_type}' for job {args.job_id}")
        else:
            print(f"Job {args.job_id} not found")
    
    elif args.command == "update-status":
        job = manager.get_job(args.job_id)
        if job:
            new_status = JobStatus(args.status)
            job.update_status(new_status)
            print(f"Updated job {args.job_id} status to {new_status.value}")
        else:
            print(f"Job {args.job_id} not found")
    
    else:
        parser.print_help()


if __name__ == "__main__":
    main()
