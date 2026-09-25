import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import {
  CommonModule
} from '@angular/common';

import {
  JobService
} from '../../../core/services/job.service';

import {
  Job
} from '../../../core/models/job.models';

import { Router } from '@angular/router';

import {
  Sidebar
} from '../../../shared/components/sidebar/sidebar';

import {
  Navbar
} from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-job-list',

  standalone: true,

  imports: [
    CommonModule,
    Sidebar,
    Navbar
  ],

  templateUrl: './job-list.html',

  styleUrl: './job-list.css'
})

export class JobList implements OnInit {

  // =========================================================
  // JOB DATA
  // =========================================================

  jobs: Job[] = [];

  // =========================================================
  // UI STATE
  // =========================================================

  isLoading = false;

  errorMessage = '';

  searchTerm = '';

  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private jobService: JobService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  // =========================================================
  // INITIAL LOAD
  // =========================================================

  ngOnInit(): void {

    this.loadJobs();

  }

  // =========================================================
  // LOAD ALL JOBS
  // =========================================================

  loadJobs(): void {

    this.isLoading = true;

    this.errorMessage = '';

    console.log(
      'Loading jobs from backend...'
    );

    this.jobService

      .getAllJobs()

      .subscribe({

        next: (jobs) => {

          console.log(
            'Jobs received from backend:',
            jobs
          );

          // =================================================
          // SORT NEWEST JOBS FIRST
          // =================================================

          this.jobs =
            [...jobs].sort(
              (a, b) =>
                new Date(b.createdAt).getTime() -
                new Date(a.createdAt).getTime()
            );

          this.isLoading = false;

          // Trigger Angular change detection
          this.cdr.detectChanges();

        },

        error: (error) => {

          console.error(
            'Unable to load jobs:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Unable to load jobs. Please try again.';

          this.isLoading = false;

          // Trigger Angular change detection
          this.cdr.detectChanges();

        }

      });

  }

  // =========================================================
  // FILTERED JOBS
  // =========================================================

  get filteredJobs(): Job[] {

    const search =
      this.searchTerm
        .trim()
        .toLowerCase();

    // =======================================================
    // SHOW ALL JOBS
    // =======================================================

    if (!search) {

      return this.jobs;

    }

    // =======================================================
    // SEARCH
    // =======================================================

    return this.jobs.filter(
      job => {

        const title =
          job.title
            ?.toLowerCase() ?? '';

        const location =
          job.location
            ?.toLowerCase() ?? '';

        const employmentType =
          job.employmentType
            ?.toLowerCase() ?? '';

        const company =
          job.recruiter?.companyName
            ?.toLowerCase() ?? '';

        const description =
          job.description
            ?.toLowerCase() ?? '';

        return (

          title.includes(search)

          ||

          location.includes(search)

          ||

          employmentType.includes(search)

          ||

          company.includes(search)

          ||

          description.includes(search)

        );

      }
    );

  }

  // =========================================================
  // SEARCH INPUT
  // =========================================================

  onSearch(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.searchTerm =
      input.value;

  }

  // =========================================================
  // VIEW JOB DETAILS
  // =========================================================

  viewJobDetails(jobId: string): void {

    this.router.navigate([
      '/jobs',
      jobId
    ]);

  }

}