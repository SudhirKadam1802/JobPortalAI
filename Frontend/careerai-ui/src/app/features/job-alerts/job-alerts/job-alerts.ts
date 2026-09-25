import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import {
  JobAlert,
  CreateJobAlertRequest
} from '../../../core/models/job-alert.models';
import { JobAlertService } from '../../../core/services/job-alert.service';
import { Sidebar } from '../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../shared/components/navbar/navbar';

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

  constructor(
    private jobAlertService: JobAlertService
  ) {}

  ngOnInit(): void {
    this.loadAlerts();
  }

  loadAlerts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.jobAlertService.getMyAlerts().subscribe({
      next: (response) => {
        this.alerts = response;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load job alerts:', error);

        this.errorMessage =
          error?.error?.message ??
          'Failed to load job alerts.';

        this.isLoading = false;
      }
    });
  }

  createAlert(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (
      !this.keyword.trim() &&
      !this.location.trim() &&
      this.minimumSalary === null
    ) {
      this.errorMessage =
        'Please enter at least one job alert filter.';

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

    const request: CreateJobAlertRequest = {
      keyword: this.keyword.trim() || null,
      location: this.location.trim() || null,
      minimumSalary: this.minimumSalary,
      isActive: this.isActive
    };

    this.isCreating = true;

    this.jobAlertService.createAlert(request).subscribe({
      next: (response) => {
        this.alerts = [
          response,
          ...this.alerts
        ];

        this.keyword = '';
        this.location = '';
        this.minimumSalary = null;
        this.isActive = true;

        this.successMessage =
          'Job alert created successfully.';

        this.isCreating = false;
      },

      error: (error) => {
        console.error('Failed to create job alert:', error);

        this.errorMessage =
          error?.error?.message ??
          'Failed to create job alert.';

        this.isCreating = false;
      }
    });
  }

  deleteAlert(id: string): void {
    const confirmed =
      window.confirm(
        'Are you sure you want to delete this job alert?'
      );

    if (!confirmed) {
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';
    this.isDeleting = true;

    this.jobAlertService.deleteAlert(id).subscribe({
      next: () => {
        this.alerts =
          this.alerts.filter(
            alert => alert.id !== id
          );

        this.successMessage =
          'Job alert deleted successfully.';

        this.isDeleting = false;
      },

      error: (error) => {
        console.error('Failed to delete job alert:', error);

        this.errorMessage =
          error?.error?.message ??
          'Failed to delete job alert.';

        this.isDeleting = false;
      }
    });
  }

  toggleAlert(alert: JobAlert): void {
    const request = {
      keyword: alert.keyword || null,
      location: alert.location || null,
      minimumSalary: alert.minimumSalary,
      isActive: !alert.isActive
    };

    this.jobAlertService
      .updateAlert(alert.id, request)
      .subscribe({
        next: (response) => {
          const index =
            this.alerts.findIndex(
              item => item.id === alert.id
            );

          if (index !== -1) {
            this.alerts[index] = response;
          }

          this.successMessage =
            response.isActive
              ? 'Job alert activated.'
              : 'Job alert deactivated.';
        },

        error: (error) => {
          console.error(
            'Failed to update job alert:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to update job alert.';
        }
      });
  }

  refreshAlerts(): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.loadAlerts();
  }
}