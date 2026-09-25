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
  ApplicationService
} from '../../../core/services/application.service';

import {
  Application
} from '../../../core/models/application.models';
import { Sidebar } from '../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../shared/components/navbar/navbar';


@Component({
  selector: 'app-my-applications',

  standalone: true,

  imports: [
    CommonModule,
    Sidebar,
    Navbar
  ],

  templateUrl: './my-applications.html',

  styleUrl: './my-applications.css'
})
export class MyApplications implements OnInit {

  // =========================================================
  // APPLICATIONS
  // =========================================================

  applications: Application[] = [];


  // =========================================================
  // UI STATE
  // =========================================================

  isLoading = false;

  errorMessage = '';


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private applicationService: ApplicationService,

    private router: Router,

    private changeDetectorRef: ChangeDetectorRef
  ) {}


  // =========================================================
  // INITIAL LOAD
  // =========================================================

  ngOnInit(): void {

    this.loadApplications();

  }


  // =========================================================
  // LOAD MY APPLICATIONS
  // =========================================================

  loadApplications(): void {

    this.isLoading = true;

    this.errorMessage = '';

    console.log(
      'Loading my applications...'
    );


    this.applicationService
      .getMyApplications()
      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (applications) => {

          console.log(
            'Applications received from backend:',
            applications
          );


          this.applications =
            [...applications].sort(
              (a, b) =>
                new Date(b.appliedAt).getTime() -
                new Date(a.appliedAt).getTime()
            );


          this.isLoading = false;

          this.errorMessage = '';


          console.log(
            'Applications displayed on UI:',
            this.applications
          );


          this.changeDetectorRef.detectChanges();

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          console.error(
            'Unable to load applications:',
            error
          );


          this.applications = [];

          this.isLoading = false;


          this.errorMessage =
            error?.error?.message ??
            'Unable to load your applications. Please try again.';


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // GET STATUS TEXT
  // =========================================================

  getStatusText(
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


  // =========================================================
  // GET STATUS CSS CLASS
  // =========================================================

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
  // GO TO JOBS
  // =========================================================

  browseJobs(): void {

    this.router.navigate([
      '/jobs'
    ]);

  }


  // =========================================================
  // REFRESH
  // =========================================================

  refreshApplications(): void {

    if (this.isLoading) {

      return;

    }


    this.loadApplications();

  }

}