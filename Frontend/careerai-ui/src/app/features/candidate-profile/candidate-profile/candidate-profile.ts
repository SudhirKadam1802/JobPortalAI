import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import { CandidateProfileService } from '../../../core/services/candidate-profile.service';

import {
  CandidateProfile,
  UpdateCandidateProfileRequest
} from '../../../core/models/candidate-profile.models';
import { Sidebar } from '../../../shared/components/sidebar/sidebar';
import { Navbar } from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-candidate-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Sidebar,
    Navbar
  ],
  templateUrl: './candidate-profile.html',
  styleUrl: './candidate-profile.css'
})
export class CandidateProfileComponent implements OnInit {

  profile: CandidateProfile | null = null;

  firstName = '';
  lastName = '';
  email = '';
  phoneNumber = '';
  location = '';
  bio = '';
  dateOfBirth = '';

  isLoading = false;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  constructor(
    private candidateProfileService: CandidateProfileService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.candidateProfileService
      .getMyProfile()
      .pipe(
        finalize(() => {

          this.isLoading = false;

          this.cdr.detectChanges();

        })
      )
      .subscribe({

        next: (response) => {

          console.log(
            'Candidate profile response:',
            response
          );

          /*
           * Store complete profile response.
           */
          this.profile = response;

          /*
           * Fill form fields.
           */
          this.firstName =
            response.firstName ?? '';

          this.lastName =
            response.lastName ?? '';

          this.email =
            response.email ?? '';

          this.phoneNumber =
            response.phoneNumber ?? '';

          this.location =
            response.location ?? '';

          this.bio =
            response.bio ?? '';

          this.dateOfBirth =
            response.dateOfBirth
              ? response.dateOfBirth.substring(0, 10)
              : '';

          /*
           * Force Angular to update the UI.
           */
          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Failed to load candidate profile:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to load candidate profile.';

          this.cdr.detectChanges();
        }

      });
  }

  saveProfile(): void {

    this.errorMessage = '';
    this.successMessage = '';

    /*
     * Validate first name.
     */
    if (!this.firstName.trim()) {

      this.errorMessage =
        'First name is required.';

      return;
    }

    /*
     * Validate last name.
     */
    if (!this.lastName.trim()) {

      this.errorMessage =
        'Last name is required.';

      return;
    }

    const request: UpdateCandidateProfileRequest = {

      firstName:
        this.firstName.trim(),

      lastName:
        this.lastName.trim(),

      phoneNumber:
        this.phoneNumber.trim() || null,

      location:
        this.location.trim() || null,

      bio:
        this.bio.trim() || null,

      dateOfBirth:
        this.dateOfBirth || null
    };

    this.isSaving = true;

    this.candidateProfileService
      .updateMyProfile(request)
      .pipe(
        finalize(() => {

          this.isSaving = false;

          this.cdr.detectChanges();

        })
      )
      .subscribe({

        next: (response) => {

          console.log(
            'Updated candidate profile:',
            response
          );

          /*
           * Update complete profile.
           */
          this.profile = response;

          /*
           * Update form fields.
           */
          this.firstName =
            response.firstName ?? '';

          this.lastName =
            response.lastName ?? '';

          this.email =
            response.email ?? '';

          this.phoneNumber =
            response.phoneNumber ?? '';

          this.location =
            response.location ?? '';

          this.bio =
            response.bio ?? '';

          this.dateOfBirth =
            response.dateOfBirth
              ? response.dateOfBirth.substring(0, 10)
              : '';

          this.successMessage =
            'Profile updated successfully.';

          /*
           * Force UI update.
           */
          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Failed to update candidate profile:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to update candidate profile.';

          this.cdr.detectChanges();
        }

      });
  }

  refreshProfile(): void {

    this.errorMessage = '';
    this.successMessage = '';

    this.loadProfile();
  }
}