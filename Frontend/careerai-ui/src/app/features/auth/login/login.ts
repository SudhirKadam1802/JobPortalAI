import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  Router,
  RouterLink
} from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  loginForm: FormGroup;

  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {

    this.loginForm = this.fb.group({

      email: [
        '',
        [
          Validators.required,
          Validators.email
        ]
      ],

      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6)
        ]
      ]

    });
  }

  onSubmit(): void {

    this.errorMessage = '';

    if (this.loginForm.invalid) {

      this.loginForm.markAllAsTouched();

      return;
    }

    this.isLoading = true;

    this.authService
      .login(this.loginForm.value)
      .subscribe({

        next: () => {

          const role =
            this.authService.getUserRole();

          console.log(
            'Login successful. Role:',
            role
          );

          if (role === 'Recruiter') {

            this.isLoading = false;

            window.location.href =
              '/recruiter/dashboard';

            return;
          }

          if (role === 'Candidate') {

            this.isLoading = false;

            window.location.href =
              '/dashboard';

            return;
          }

          this.isLoading = false;

          this.errorMessage =
            'Unknown user role.';

        },

        error: (error) => {

          console.error(
            'Login error:',
            error
          );

          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ??
            error?.message ??
            'Login failed. Please check your email and password.';
        }

      });
  }
}