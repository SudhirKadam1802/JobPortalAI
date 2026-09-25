import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  CommonModule
} from '@angular/common';

import {
  Router
} from '@angular/router';

import {
  SavedJobService
} from '../../../core/services/saved-job.service';

import {
  SavedJob
} from '../../../core/models/saved-job.models';
import { Sidebar } from '../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../shared/components/navbar/navbar';


@Component({
  selector: 'app-saved-jobs',

  standalone: true,

  imports: [
    CommonModule,
    Sidebar,
    Navbar
  ],

  templateUrl: './saved-jobs.html',

  styleUrl: './saved-jobs.css'
})
export class SavedJobs implements OnInit {

  // =========================================================
  // DATA
  // =========================================================

  savedJobs: SavedJob[] = [];


  // =========================================================
  // UI STATE
  // =========================================================

  isLoading = false;

  errorMessage = '';

  removingJobId: string | null = null;

  successMessage = '';


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private savedJobService: SavedJobService,

    private router: Router,

    private changeDetectorRef: ChangeDetectorRef
  ) {}


  // =========================================================
  // INITIAL LOAD
  // =========================================================

  ngOnInit(): void {

    this.loadSavedJobs();

  }


  // =========================================================
  // LOAD SAVED JOBS
  // =========================================================

  loadSavedJobs(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.successMessage = '';


    this.savedJobService
      .getMySavedJobs()
      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (savedJobs) => {

          console.log(
            'Saved jobs received:',
            savedJobs
          );


          this.savedJobs = [...savedJobs];

          this.isLoading = false;

          this.errorMessage = '';


          this.changeDetectorRef.detectChanges();

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          console.error(
            'Unable to load saved jobs:',
            error
          );


          this.savedJobs = [];

          this.isLoading = false;


          this.errorMessage =
            error?.error?.message ??
            'Unable to load your saved jobs. Please try again.';


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // REMOVE SAVED JOB
  // =========================================================

  removeSavedJob(
    jobId: string
  ): void {

    if (this.removingJobId) {
      return;
    }


    this.removingJobId = jobId;

    this.errorMessage = '';

    this.successMessage = '';


    console.log(
      'Removing saved job:',
      jobId
    );


    this.savedJobService
      .removeSavedJob(jobId)
      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (response) => {

          console.log(
            'Saved job removed:',
            response
          );


          this.savedJobs =
            this.savedJobs.filter(
              job => job.jobId !== jobId
            );


          this.removingJobId = null;

          this.successMessage =
            'Job removed from saved jobs.';


          this.changeDetectorRef.detectChanges();

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          console.error(
            'Unable to remove saved job:',
            error
          );


          this.removingJobId = null;


          this.errorMessage =
            error?.error?.message ??
            'Unable to remove the saved job. Please try again.';


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // VIEW JOB
  // =========================================================

  viewJob(
    jobId: string
  ): void {

    this.router.navigate([
      '/jobs',
      jobId
    ]);

  }


  // =========================================================
  // BROWSE JOBS
  // =========================================================

  browseJobs(): void {

    this.router.navigate([
      '/jobs'
    ]);

  }


  // =========================================================
  // REFRESH
  // =========================================================

  refreshSavedJobs(): void {

    if (this.isLoading) {
      return;
    }

    this.loadSavedJobs();

  }

}