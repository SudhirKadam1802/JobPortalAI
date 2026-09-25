import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../core/models/auth.models';

function matchingPasswords(
  control: AbstractControl
): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;

  return password === confirmPassword
    ? null
    : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  registerForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group(
      {
        firstName: ['', [Validators.required, Validators.maxLength(100)]],
        lastName: ['', [Validators.required, Validators.maxLength(100)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]],
        role: [1, [Validators.required]]
      },
      { validators: matchingPasswords }
    );
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const value = this.registerForm.value;
    const request: RegisterRequest = {
      firstName: value.firstName.trim(),
      lastName: value.lastName.trim(),
      email: value.email.trim(),
      password: value.password,
      role: Number(value.role)
    };

    this.authService.register(request).subscribe({
      next: () => {
        this.successMessage = 'Account created successfully. Redirecting...';
        const role = this.authService.getUserRole();

        if (role === 'Recruiter') {
          window.location.href = '/recruiter/dashboard';
          return;
        }

        if (role === 'Candidate') {
          window.location.href = '/dashboard';
          return;
        }

        this.isLoading = false;
        this.errorMessage = 'Registration succeeded, but the user role is unknown.';
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error?.error?.message ??
          error?.message ??
          'Registration failed. Please try again.';
      }
    });
  }
}
