
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { Job } from '../../../../core/models/job.models';

import {
  JobService,
  CreateJobRequest,
  UpdateJobRequest
} from '../../../../core/services/job.service';

import { Sidebar } from '../../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-recruiter-jobs',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Sidebar,
    Navbar
  ],
  templateUrl: './recruiter-jobs.html',
  styleUrl: './recruiter-jobs.css'
})
export class RecruiterJobs implements OnInit {

  jobs: Job[] = [];

  isLoading = false;
  isSaving = false;
  isDeleting = false;

  errorMessage = '';
  successMessage = '';

  isFormOpen = false;
  isEditMode = false;

  editingJobId: string | null = null;

  // ========================================
  // Job Form Fields
  // ========================================

  companyName = '';

  title = '';
  description = '';
  location = '';
  employmentType = '';

  minimumExperience = 0;
  maximumExperience = 0;

  minimumSalary: number | null = null;
  maximumSalary: number | null = null;

  requiredEducation = '';
  applicationDeadline = '';

  // ========================================
  // Constructor
  // ========================================

  constructor(
    private jobService: JobService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  // ========================================
  // Initialize
  // ========================================

  ngOnInit(): void {
    this.loadJobs();
  }

  // ========================================
  // Load Recruiter Jobs
  // ========================================

  loadJobs(): void {

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.jobService
      .getAllJobs()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({

        next: (response) => {

          console.log(
            'Recruiter jobs response:',
            response
          );

          this.jobs = response;

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Failed to load jobs:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to load jobs.';

          this.cdr.detectChanges();
        }

      });
  }

  // ========================================
  // Open Create Job Form
  // ========================================

  openCreateForm(): void {

    this.resetForm();

    this.errorMessage = '';
    this.successMessage = '';

    this.isEditMode = false;
    this.isFormOpen = true;

    this.cdr.detectChanges();
  }

  // ========================================
  // Open Edit Job Form
  // ========================================

  openEditForm(job: Job): void {

    this.errorMessage = '';
    this.successMessage = '';

    this.isEditMode = true;
    this.isFormOpen = true;

    this.editingJobId = job.id;

    // Populate company name from the selected job
    this.companyName = job.companyName ?? '';

    this.title = job.title;
    this.description = job.description;
    this.location = job.location;
    this.employmentType = job.employmentType;

    this.minimumExperience = job.minimumExperience;
    this.maximumExperience = job.maximumExperience;

    this.minimumSalary = job.minimumSalary ?? null;
    this.maximumSalary = job.maximumSalary ?? null;

    this.requiredEducation = job.requiredEducation ?? '';

    this.applicationDeadline =
      job.applicationDeadline
        ? job.applicationDeadline.substring(0, 10)
        : '';

    this.cdr.detectChanges();
  }

  // ========================================
  // Close Form
  // ========================================

  closeForm(): void {

    if (this.isSaving) {
      return;
    }

    this.isFormOpen = false;

    this.resetForm();

    this.cdr.detectChanges();
  }

  // ========================================
  // Create / Update Job
  // ========================================

  saveJob(): void {

    this.errorMessage = '';
    this.successMessage = '';

    // Company Name Validation
    if (!this.companyName.trim()) {

      this.errorMessage =
        'Company name is required.';

      return;
    }

    if (this.companyName.trim().length > 150) {

      this.errorMessage =
        'Company name cannot exceed 150 characters.';

      return;
    }

    // Job Title Validation
    if (!this.title.trim()) {

      this.errorMessage =
        'Job title is required.';

      return;
    }

    // Description Validation
    if (!this.description.trim()) {

      this.errorMessage =
        'Job description is required.';

      return;
    }

    // Location Validation
    if (!this.location.trim()) {

      this.errorMessage =
        'Location is required.';

      return;
    }

    // Employment Type Validation
    if (!this.employmentType.trim()) {

      this.errorMessage =
        'Employment type is required.';

      return;
    }

    // Experience Validation
    if (this.minimumExperience < 0) {

      this.errorMessage =
        'Minimum experience cannot be negative.';

      return;
    }

    if (
      this.maximumExperience <
      this.minimumExperience
    ) {

      this.errorMessage =
        'Maximum experience cannot be less than minimum experience.';

      return;
    }

    // Minimum Salary Validation
    if (
      this.minimumSalary !== null &&
      this.minimumSalary < 0
    ) {

      this.errorMessage =
        'Minimum salary cannot be negative.';

      return;
    }

    // Maximum Salary Validation
    if (
      this.maximumSalary !== null &&
      this.maximumSalary < 0
    ) {

      this.errorMessage =
        'Maximum salary cannot be negative.';

      return;
    }

    // Salary Range Validation
    if (
      this.minimumSalary !== null &&
      this.maximumSalary !== null &&
      this.maximumSalary < this.minimumSalary
    ) {

      this.errorMessage =
        'Maximum salary cannot be less than minimum salary.';

      return;
    }

    // Application Deadline Validation
    if (!this.applicationDeadline) {

      this.errorMessage =
        'Application deadline is required.';

      return;
    }

    // ========================================
    // Prepare API Request
    // ========================================

    const request:
      CreateJobRequest | UpdateJobRequest = {

      companyName:
        this.companyName.trim(),

      title:
        this.title.trim(),

      description:
        this.description.trim(),

      location:
        this.location.trim(),

      employmentType:
        this.employmentType.trim(),

      minimumExperience:
        this.minimumExperience,

      maximumExperience:
        this.maximumExperience,

      minimumSalary:
        this.minimumSalary,

      maximumSalary:
        this.maximumSalary,

      requiredEducation:
        this.requiredEducation.trim() || null,

      applicationDeadline:
        this.applicationDeadline
    };

    this.isSaving = true;

    this.cdr.detectChanges();

    // ========================================
    // Update Existing Job
    // ========================================

    if (
      this.isEditMode &&
      this.editingJobId
    ) {

      this.jobService
        .updateJob(
          this.editingJobId,
          request as UpdateJobRequest
        )
        .pipe(
          finalize(() => {
            this.isSaving = false;
            this.cdr.detectChanges();
          })
        )
        .subscribe({

          next: (response) => {

            const index =
              this.jobs.findIndex(
                job => job.id === response.id
              );

            if (index !== -1) {
              this.jobs[index] = response;
            }

            this.successMessage =
              'Job updated successfully.';

            this.isFormOpen = false;

            this.resetForm();

            this.cdr.detectChanges();
          },

          error: (error) => {

            console.error(
              'Failed to update job:',
              error
            );

            this.errorMessage =
              error?.error?.message ??
              'Failed to update job.';

            this.cdr.detectChanges();
          }

        });

    } else {

      // ========================================
      // Create New Job
      // ========================================

      this.jobService
        .createJob(
          request as CreateJobRequest
        )
        .pipe(
          finalize(() => {
            this.isSaving = false;
            this.cdr.detectChanges();
          })
        )
        .subscribe({

          next: (response) => {

            this.jobs = [
              response,
              ...this.jobs
            ];

            this.successMessage =
              'Job created successfully.';

            this.isFormOpen = false;

            this.resetForm();

            this.cdr.detectChanges();
          },

          error: (error) => {

            console.error(
              'Failed to create job:',
              error
            );

            this.errorMessage =
              error?.error?.message ??
              'Failed to create job.';

            this.cdr.detectChanges();
          }

        });
    }
  }

  // ========================================
  // Delete Job
  // ========================================

  deleteJob(job: Job): void {

    const confirmed =
      window.confirm(
        `Are you sure you want to delete "${job.title}"?`
      );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.isDeleting = true;

    this.cdr.detectChanges();

    this.jobService
      .deleteJob(job.id)
      .pipe(
        finalize(() => {
          this.isDeleting = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({

        next: () => {

          this.jobs =
            this.jobs.filter(
              item => item.id !== job.id
            );

          this.successMessage =
            'Job deleted successfully.';

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Failed to delete job:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to delete job.';

          this.cdr.detectChanges();
        }

      });
  }

  // ========================================
  // View Applications
  // ========================================

  viewApplications(jobId: string): void {

    this.router.navigate([
      '/recruiter/jobs',
      jobId,
      'applications'
    ]);
  }

  // ========================================
  // Refresh Jobs
  // ========================================

  refreshJobs(): void {
    this.loadJobs();
  }

  // ========================================
  // Reset Form
  // ========================================

  resetForm(): void {

    this.editingJobId = null;

    this.companyName = '';

    this.title = '';
    this.description = '';
    this.location = '';
    this.employmentType = '';

    this.minimumExperience = 0;
    this.maximumExperience = 0;

    this.minimumSalary = null;
    this.maximumSalary = null;

    this.requiredEducation = '';
    this.applicationDeadline = '';
  }
}