import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import {
  Job
} from '../../../../core/models/job.models';

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

  constructor(
    private jobService: JobService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadJobs();
  }

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

  openCreateForm(): void {

    this.resetForm();

    this.errorMessage = '';

    this.isEditMode = false;
    this.isFormOpen = true;

    this.cdr.detectChanges();
  }

  openEditForm(job: Job): void {

    this.errorMessage = '';

    this.isEditMode = true;
    this.isFormOpen = true;

    this.editingJobId = job.id;

    this.title =
      job.title;

    this.description =
      job.description;

    this.location =
      job.location;

    this.employmentType =
      job.employmentType;

    this.minimumExperience =
      job.minimumExperience;

    this.maximumExperience =
      job.maximumExperience;

    this.minimumSalary =
      job.minimumSalary ?? null;

    this.maximumSalary =
      job.maximumSalary ?? null;

    this.requiredEducation =
      job.requiredEducation ?? '';

    this.applicationDeadline =
      job.applicationDeadline
        ? job.applicationDeadline.substring(0, 10)
        : '';

    this.cdr.detectChanges();
  }

  closeForm(): void {

    if (this.isSaving) {
      return;
    }

    this.isFormOpen = false;

    this.resetForm();

    this.cdr.detectChanges();
  }

  saveJob(): void {

    this.errorMessage = '';
    this.successMessage = '';

    if (!this.title.trim()) {

      this.errorMessage =
        'Job title is required.';

      return;
    }

    if (!this.description.trim()) {

      this.errorMessage =
        'Job description is required.';

      return;
    }

    if (!this.location.trim()) {

      this.errorMessage =
        'Location is required.';

      return;
    }

    if (!this.employmentType.trim()) {

      this.errorMessage =
        'Employment type is required.';

      return;
    }

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

    if (
      this.minimumSalary !== null &&
      this.minimumSalary < 0
    ) {

      this.errorMessage =
        'Minimum salary cannot be negative.';

      return;
    }

    if (
      this.maximumSalary !== null &&
      this.maximumSalary < 0
    ) {

      this.errorMessage =
        'Maximum salary cannot be negative.';

      return;
    }

    if (
      this.minimumSalary !== null &&
      this.maximumSalary !== null &&
      this.maximumSalary <
      this.minimumSalary
    ) {

      this.errorMessage =
        'Maximum salary cannot be less than minimum salary.';

      return;
    }

    if (!this.applicationDeadline) {

      this.errorMessage =
        'Application deadline is required.';

      return;
    }

    const request:
      CreateJobRequest | UpdateJobRequest = {

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
                job =>
                  job.id === response.id
              );

            if (index !== -1) {

              this.jobs[index] =
                response;
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
              item =>
                item.id !== job.id
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

  viewApplications(jobId: string): void {

    this.router.navigate([
      '/recruiter/jobs',
      jobId,
      'applications'
    ]);
  }

  refreshJobs(): void {

    this.loadJobs();
  }

  resetForm(): void {

    this.editingJobId = null;

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