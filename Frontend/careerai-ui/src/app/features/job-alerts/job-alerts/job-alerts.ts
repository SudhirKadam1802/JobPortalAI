
import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import {
  JobAlert,
  CreateJobAlertRequest,
  UpdateJobAlertRequest
} from '../../../core/models/job-alert.models';

import {
  JobAlertService
} from '../../../core/services/job-alert.service';

import {
  Sidebar
} from '../../../shared/components/sidebar/sidebar';

import {
  Navbar
} from '../../../shared/components/navbar/navbar';


@Component({
  selector: 'app-job-alerts',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    Sidebar,
    Navbar
  ],

  templateUrl: './job-alerts.html',

  styleUrl: './job-alerts.css'
})
export class JobAlerts implements OnInit {

  // =========================================================
  // COMPONENT STATE
  // =========================================================

  alerts: JobAlert[] = [];

  isLoading = false;
  isCreating = false;
  isDeleting = false;

  errorMessage = '';
  successMessage = '';

  keyword = '';
  location = '';

  minimumSalary: number | null = null;

  isActive = true;


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private jobAlertService: JobAlertService,
    private cdr: ChangeDetectorRef
  ) {}


  // =========================================================
  // COMPONENT INITIALIZATION
  // =========================================================

  ngOnInit(): void {
    this.loadAlerts();
  }


  // =========================================================
  // LOAD MY JOB ALERTS
  // =========================================================

  loadAlerts(): void {

    this.isLoading = true;

    this.errorMessage = '';
    this.successMessage = '';

    // Immediately update the loading indicator.
    this.cdr.detectChanges();

    this.jobAlertService.getMyAlerts().subscribe({

      next: (response: JobAlert[]) => {

        // Update the alerts returned by the API.
        this.alerts = response ?? [];

        this.isLoading = false;

        this.errorMessage = '';

        // IMPORTANT:
        // Refresh the Angular view immediately after
        // the HTTP response is received.
        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(
          'Failed to load job alerts:',
          error
        );

        this.alerts = [];

        this.errorMessage =
          error?.error?.message ??
          'Failed to load job alerts.';

        this.isLoading = false;

        // Refresh the UI so the loading spinner disappears
        // and the error message becomes visible.
        this.cdr.detectChanges();
      }

    });

  }


  // =========================================================
  // CREATE JOB ALERT
  // =========================================================

  createAlert(): void {

    this.successMessage = '';
    this.errorMessage = '';

    // Validate at least one filter.
    if (
      !this.keyword.trim() &&
      !this.location.trim() &&
      this.minimumSalary === null
    ) {

      this.errorMessage =
        'Please enter at least one job alert filter.';

      this.cdr.detectChanges();

      return;
    }

    // Validate minimum salary.
    if (
      this.minimumSalary !== null &&
      this.minimumSalary < 0
    ) {

      this.errorMessage =
        'Minimum salary cannot be negative.';

      this.cdr.detectChanges();

      return;
    }

    // Prepare the request.
    const request: CreateJobAlertRequest = {

      keyword:
        this.keyword.trim() || null,

      location:
        this.location.trim() || null,

      minimumSalary:
        this.minimumSalary,

      isActive:
        this.isActive

    };

    this.isCreating = true;

    this.cdr.detectChanges();

    this.jobAlertService.createAlert(request).subscribe({

      next: (response: JobAlert) => {

        // Add the newly created alert to the top.
        this.alerts = [
          response,
          ...this.alerts
        ];

        // Reset the form.
        this.keyword = '';

        this.location = '';

        this.minimumSalary = null;

        this.isActive = true;

        this.successMessage =
          'Job alert created successfully.';

        this.errorMessage = '';

        this.isCreating = false;

        // Immediately update the UI.
        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(
          'Failed to create job alert:',
          error
        );

        this.errorMessage =
          error?.error?.message ??
          'Failed to create job alert.';

        this.isCreating = false;

        this.cdr.detectChanges();
      }

    });

  }


  // =========================================================
  // DELETE JOB ALERT
  // =========================================================

  deleteAlert(id: string): void {

    const confirmed = window.confirm(
      'Are you sure you want to delete this job alert?'
    );

    if (!confirmed) {
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';

    this.isDeleting = true;

    this.cdr.detectChanges();

    this.jobAlertService.deleteAlert(id).subscribe({

      next: () => {

        // Remove the deleted alert from the list.
        this.alerts = this.alerts.filter(
          alert => alert.id !== id
        );

        this.successMessage =
          'Job alert deleted successfully.';

        this.isDeleting = false;

        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(
          'Failed to delete job alert:',
          error
        );

        this.errorMessage =
          error?.error?.message ??
          'Failed to delete job alert.';

        this.isDeleting = false;

        this.cdr.detectChanges();
      }

    });

  }


  // =========================================================
  // ACTIVATE / DEACTIVATE JOB ALERT
  // =========================================================

  toggleAlert(alert: JobAlert): void {

    this.successMessage = '';
    this.errorMessage = '';

    const request: UpdateJobAlertRequest = {

      keyword:
        alert.keyword || null,

      location:
        alert.location || null,

      minimumSalary:
        alert.minimumSalary,

      isActive:
        !alert.isActive

    };

    this.jobAlertService
      .updateAlert(alert.id, request)
      .subscribe({

        next: (response: JobAlert) => {

          // Find the alert being updated.
          const index = this.alerts.findIndex(
            item => item.id === alert.id
          );

          // Replace the existing alert with
          // the latest response from the API.
          if (index !== -1) {

            this.alerts = this.alerts.map(
              (item, i) =>
                i === index ? response : item
            );

          }

          this.successMessage =
            response.isActive
              ? 'Job alert activated.'
              : 'Job alert deactivated.';

          this.errorMessage = '';

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Failed to update job alert:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to update job alert.';

          this.cdr.detectChanges();
        }

      });

  }


  // =========================================================
  // REFRESH JOB ALERTS
  // =========================================================

  refreshAlerts(): void {

    this.successMessage = '';
    this.errorMessage = '';

    this.loadAlerts();

  }

}