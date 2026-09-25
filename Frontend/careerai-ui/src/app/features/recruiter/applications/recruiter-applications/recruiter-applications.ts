import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import {
  RecruiterApplication,
  UpdateApplicationStatusRequest
} from '../../../../core/models/recruiter-application.models';

import { RecruiterApplicationService } from '../../../../core/services/recruiter-application.service';
import { Sidebar } from '../../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-recruiter-applications',
  standalone: true,
  imports: [
    CommonModule,
    Sidebar,
    Navbar
  ],
  templateUrl: './recruiter-applications.html',
  styleUrl: './recruiter-applications.css'
})
export class RecruiterApplications implements OnInit {

  jobId = '';

  applications: RecruiterApplication[] = [];

  isLoading = false;
  updatingApplicationId: string | null = null;

  errorMessage = '';
  successMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private recruiterApplicationService: RecruiterApplicationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {

    this.jobId =
      this.route.snapshot.paramMap.get('jobId') ?? '';

    if (!this.jobId) {

      this.errorMessage =
        'Job ID is missing.';

      return;
    }

    this.loadApplications();
  }

  loadApplications(): void {

    if (!this.jobId) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.recruiterApplicationService
      .getJobApplications(this.jobId)
      .pipe(
        finalize(() => {

          this.isLoading = false;

          this.cdr.detectChanges();

        })
      )
      .subscribe({

        next: (response) => {

          console.log(
            'Recruiter applications:',
            response
          );

          this.applications =
            response;

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Failed to load applications:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to load applications.';

          this.cdr.detectChanges();
        }

      });
  }

  updateStatus(
    application: RecruiterApplication,
    status: number
  ): void {

    if (
      this.updatingApplicationId
    ) {
      return;
    }

    if (
      application.status === status
    ) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    const request:
      UpdateApplicationStatusRequest = {
      status
    };

    this.updatingApplicationId =
      application.id;

    this.recruiterApplicationService
      .updateApplicationStatus(
        application.id,
        request
      )
      .pipe(
        finalize(() => {

          this.updatingApplicationId =
            null;

          this.cdr.detectChanges();

        })
      )
      .subscribe({

        next: (response) => {

          const index =
            this.applications.findIndex(
              item =>
                item.id === response.id
            );

          if (index !== -1) {

            this.applications[index] =
              response;

          }

          this.successMessage =
            'Application status updated successfully.';

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Failed to update application status:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to update application status.';

          this.cdr.detectChanges();
        }

      });
  }

  getStatusName(
    status: number
  ): string {

    switch (status) {

      case 1:
        return 'Applied';

      case 2:
        return 'Shortlisted';

      case 3:
        return 'Interview';

      case 4:
        return 'Rejected';

      case 5:
        return 'Hired';

      default:
        return 'Unknown';
    }
  }

  getStatusClass(
    status: number
  ): string {

    switch (status) {

      case 1:
        return 'status-applied';

      case 2:
        return 'status-shortlisted';

      case 3:
        return 'status-interview';

      case 4:
        return 'status-rejected';

      case 5:
        return 'status-hired';

      default:
        return 'status-unknown';
    }
  }

  goBack(): void {

    this.router.navigate([
      '/recruiter/jobs'
    ]);
  }

  refreshApplications(): void {

    this.loadApplications();
  }
}