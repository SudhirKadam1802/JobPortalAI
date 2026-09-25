import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  CommonModule
} from '@angular/common';

import {
  FormsModule
} from '@angular/forms';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import {
  JobService
} from '../../../core/services/job.service';

import {
  Job
} from '../../../core/models/job.models';

import {
  ApplicationService
} from '../../../core/services/application.service';

import {
  Application
} from '../../../core/models/application.models';

import {
  SavedJobService
} from '../../../core/services/saved-job.service';
import { Sidebar } from '../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../shared/components/navbar/navbar';


@Component({
  selector: 'app-job-details',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    Sidebar,
    Navbar
  ],

  templateUrl: './job-details.html',

  styleUrl: './job-details.css'
})
export class JobDetails implements OnInit {

  // =========================================================
  // JOB DATA
  // =========================================================

  job: Job | null = null;


  // =========================================================
  // JOB UI STATE
  // =========================================================

  isLoading = false;

  errorMessage = '';


  // =========================================================
  // APPLICATION STATE
  // =========================================================

  isApplying = false;

  isCheckingApplication = false;

  hasApplied = false;

  existingApplication: Application | null = null;

  showCoverLetter = false;

  coverLetter = '';

  applicationMessage = '';

  applicationError = '';


  // =========================================================
  // SAVED JOB STATE
  // =========================================================

  isCheckingSavedJob = false;

  isSavingJob = false;

  isJobSaved = false;

  savedJobMessage = '';

  savedJobError = '';


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private route: ActivatedRoute,

    private router: Router,

    private jobService: JobService,

    private applicationService: ApplicationService,

    private savedJobService: SavedJobService,

    private changeDetectorRef: ChangeDetectorRef
  ) {}


  // =========================================================
  // INITIAL LOAD
  // =========================================================

  ngOnInit(): void {

    this.loadJob();

  }


  // =========================================================
  // LOAD JOB
  // =========================================================

  loadJob(): void {

    const jobId =
      this.route.snapshot.paramMap.get('id');


    // =======================================================
    // CHECK JOB ID
    // =======================================================

    if (!jobId) {

      this.errorMessage =
        'Job ID is missing.';

      this.isLoading = false;

      this.changeDetectorRef.detectChanges();

      return;

    }


    // =======================================================
    // RESET STATE
    // =======================================================

    this.isLoading = true;

    this.errorMessage = '';

    this.job = null;

    this.applicationMessage = '';

    this.applicationError = '';

    this.hasApplied = false;

    this.existingApplication = null;

    this.isJobSaved = false;

    this.savedJobMessage = '';

    this.savedJobError = '';


    console.log(
      'Loading job details:',
      jobId
    );


    this.changeDetectorRef.detectChanges();


    // =======================================================
    // GET JOB
    // =======================================================

    this.jobService
      .getJobById(jobId)
      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (job) => {

          console.log(
            'Job details received:',
            job
          );


          this.job = job;

          this.isLoading = false;

          this.errorMessage = '';


          console.log(
            'Job assigned to UI:',
            this.job
          );


          this.changeDetectorRef.detectChanges();


          // Check existing application
          this.checkExistingApplication(job.id);

          // Check saved job
          this.checkSavedJob(job.id);

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          console.error(
            'Unable to load job details:',
            error
          );


          this.job = null;

          this.isLoading = false;


          this.errorMessage =
            error?.error?.message ??
            'Unable to load job details. Please try again.';


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // CHECK EXISTING APPLICATION
  // =========================================================

  checkExistingApplication(
    jobId: string
  ): void {

    this.isCheckingApplication = true;

    this.applicationError = '';


    console.log(
      'Checking existing applications for job:',
      jobId
    );


    this.applicationService
      .getMyApplications()
      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (applications) => {

          console.log(
            'My applications received:',
            applications
          );


          const existingApplication =
            applications.find(
              application =>
                application.jobId.toLowerCase() ===
                jobId.toLowerCase()
            );


          if (existingApplication) {

            this.hasApplied = true;

            this.existingApplication =
              existingApplication;

            console.log(
              'Candidate has already applied:',
              existingApplication
            );

          }
          else {

            this.hasApplied = false;

            this.existingApplication = null;

            console.log(
              'Candidate has not applied for this job.'
            );

          }


          this.isCheckingApplication = false;


          this.changeDetectorRef.detectChanges();

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          console.error(
            'Unable to check existing applications:',
            error
          );


          this.isCheckingApplication = false;


          /*
           * Do not block the Apply button if checking
           * applications fails.
           *
           * The backend will still protect against
           * duplicate applications.
           */

          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // CHECK SAVED JOB
  // =========================================================

  checkSavedJob(
    jobId: string
  ): void {

    this.isCheckingSavedJob = true;

    this.savedJobError = '';

    console.log(
      'Checking saved job:',
      jobId
    );


    this.savedJobService
      .checkSavedJob(jobId)
      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (response) => {

          console.log(
            'Saved job check response:',
            response
          );


          this.isJobSaved =
            response.isSaved;

          this.isCheckingSavedJob = false;

          this.changeDetectorRef.detectChanges();

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          console.error(
            'Unable to check saved job:',
            error
          );


          this.isJobSaved = false;

          this.isCheckingSavedJob = false;

          this.savedJobError =
            error?.error?.message ??
            'Unable to check saved job status.';


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // TOGGLE SAVED JOB
  // =========================================================

  toggleSavedJob(): void {

    if (!this.job) {
      return;
    }


    if (this.isSavingJob) {
      return;
    }


    this.isSavingJob = true;

    this.savedJobMessage = '';

    this.savedJobError = '';


    // =======================================================
    // REMOVE SAVED JOB
    // =======================================================

    if (this.isJobSaved) {

      console.log(
        'Removing saved job:',
        this.job.id
      );


      this.savedJobService
        .removeSavedJob(this.job.id)
        .subscribe({

          next: (response) => {

            console.log(
              'Saved job removed:',
              response
            );


            this.isJobSaved = false;

            this.isSavingJob = false;

            this.savedJobMessage =
              'Job removed from saved jobs.';


            this.changeDetectorRef.detectChanges();

          },


          error: (error) => {

            console.error(
              'Unable to remove saved job:',
              error
            );


            this.isSavingJob = false;

            this.savedJobError =
              error?.error?.message ??
              'Unable to remove saved job.';


            this.changeDetectorRef.detectChanges();

          }

        });

      return;

    }


    // =======================================================
    // SAVE JOB
    // =======================================================

    console.log(
      'Saving job:',
      this.job.id
    );


    this.savedJobService
      .saveJob(this.job.id)
      .subscribe({

        next: (savedJob) => {

          console.log(
            'Job saved successfully:',
            savedJob
          );


          this.isJobSaved = true;

          this.isSavingJob = false;

          this.savedJobMessage =
            'Job saved successfully!';


          this.savedJobError = '';


          this.changeDetectorRef.detectChanges();

        },


        error: (error) => {

          console.error(
            'Unable to save job:',
            error
          );


          this.isSavingJob = false;


          if (error?.error?.message) {

            this.savedJobError =
              error.error.message;

          }
          else if (error?.status === 401) {

            this.savedJobError =
              'You are not authorized. Please login again as a Candidate.';

          }
          else if (error?.status === 404) {

            this.savedJobError =
              'The job was not found.';

          }
          else if (error?.status >= 500) {

            this.savedJobError =
              'A server error occurred while saving the job.';

          }
          else {

            this.savedJobError =
              `Unable to save job. HTTP Status: ${
                error?.status ?? 'Unknown'
              }`;

          }


          /*
           * Backend duplicate protection
           */

          if (
            error?.error?.message ===
            'You have already saved this job.'
          ) {

            this.isJobSaved = true;

          }


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // START APPLICATION
  // =========================================================

  startApplication(): void {

    if (!this.job) {

      return;

    }


    // Don't allow duplicate application

    if (this.hasApplied) {

      this.applicationMessage =
        'You have already applied for this job.';

      this.applicationError = '';

      this.changeDetectorRef.detectChanges();

      return;

    }


    if (this.isApplying) {

      return;

    }


    this.applicationMessage = '';

    this.applicationError = '';

    this.coverLetter = '';

    this.showCoverLetter = true;


    this.changeDetectorRef.detectChanges();

  }


  // =========================================================
  // SUBMIT APPLICATION
  // =========================================================

  submitApplication(): void {

    if (!this.job) {

      return;

    }


    if (this.isApplying) {

      return;

    }


    // =======================================================
    // DUPLICATE PROTECTION
    // =======================================================

    if (this.hasApplied) {

      this.applicationMessage =
        'You have already applied for this job.';

      this.applicationError = '';

      this.showCoverLetter = false;

      this.changeDetectorRef.detectChanges();

      return;

    }


    // =======================================================
    // CREATE REQUEST
    // =======================================================

    const request = {

      jobId: this.job.id,

      coverLetter:
        this.coverLetter.trim() || null

    };


    // =======================================================
    // START SUBMITTING
    // =======================================================

    this.isApplying = true;

    this.applicationMessage = '';

    this.applicationError = '';


    console.log(
      'Submitting job application:',
      request
    );


    this.changeDetectorRef.detectChanges();


    // =======================================================
    // CALL APPLICATION API
    // =======================================================

    this.applicationService
      .applyForJob(request)
      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (application) => {

          console.log(
            'Application submitted successfully:',
            application
          );


          this.isApplying = false;

          this.hasApplied = true;

          this.existingApplication =
            application;

          this.applicationMessage =
            'Application submitted successfully!';

          this.applicationError = '';

          this.showCoverLetter = false;

          this.coverLetter = '';


          this.changeDetectorRef.detectChanges();

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          console.error(
            'Application submission failed:',
            error
          );


          console.error(
            'HTTP Status:',
            error?.status
          );


          console.error(
            'Backend Error:',
            error?.error
          );


          this.isApplying = false;


          this.applicationMessage = '';


          // =================================================
          // BACKEND MESSAGE
          // =================================================

          if (error?.error?.message) {

            this.applicationError =
              error.error.message;

          }


          else if (
            typeof error?.error === 'string'
          ) {

            this.applicationError =
              error.error;

          }


          else if (error?.status === 401) {

            this.applicationError =
              'You are not authorized. Please login again as a Candidate.';

          }


          else if (error?.status === 400) {

            this.applicationError =
              'The application request was rejected by the server.';

          }


          else if (error?.status === 409) {

            this.applicationError =
              'You have already applied for this job.';

          }


          else if (error?.status === 404) {

            this.applicationError =
              'The job was not found.';

          }


          else if (error?.status >= 500) {

            this.applicationError =
              'A server error occurred while submitting your application.';

          }


          else {

            this.applicationError =
              `Application failed. HTTP Status: ${
                error?.status ?? 'Unknown'
              }`;

          }


          /*
           * If backend tells us that the application
           * already exists, update the UI state.
           */

          if (
            error?.error?.message ===
            'You have already applied for this job.'
          ) {

            this.hasApplied = true;

            this.showCoverLetter = false;

          }


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // CANCEL APPLICATION
  // =========================================================

  cancelApplication(): void {

    this.showCoverLetter = false;

    this.coverLetter = '';

    this.applicationMessage = '';

    this.applicationError = '';


    this.changeDetectorRef.detectChanges();

  }


  // =========================================================
  // GO BACK TO JOBS
  // =========================================================

  goBack(): void {

    this.router.navigate([
      '/jobs'
    ]);

  }

}