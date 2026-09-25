
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';

import { Sidebar } from '../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../shared/components/navbar/navbar';

import { RecruiterDashboard } from '../../../core/models/recruiter-dashboard.models';
import { RecruiterDashboardService } from '../../../core/services/recruiter-dashboard.service';

@Component({
  selector: 'app-recruiter-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    Sidebar,
    Navbar
  ],
  templateUrl: './recruiter-dashboard.html',
  styleUrl: './recruiter-dashboard.css'
})
export class RecruiterDashboardComponent
  implements OnInit {

  dashboard: RecruiterDashboard | null = null;

  isLoading = false;

  errorMessage = '';

  constructor(
    private recruiterDashboardService:
      RecruiterDashboardService,

    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.recruiterDashboardService
      .getDashboard()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (response) => {
          console.log(
            'Recruiter dashboard response:',
            response
          );

          this.dashboard = response;

          this.cdr.detectChanges();
        },

        error: (error) => {
          console.error(
            'Failed to load recruiter dashboard:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to load recruiter dashboard.';

          this.cdr.detectChanges();
        }
      });
  }

  refreshDashboard(): void {
    this.loadDashboard();
  }
}