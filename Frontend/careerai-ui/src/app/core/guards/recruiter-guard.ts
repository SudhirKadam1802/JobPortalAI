import { inject } from '@angular/core';

import {
  CanActivateFn,
  Router
} from '@angular/router';

import { AuthService } from '../services/auth.service';

export const recruiterGuard: CanActivateFn = () => {

  const authService = inject(AuthService);

  const router = inject(Router);

  // User is not logged in
  if (!authService.isLoggedIn()) {

    return router.createUrlTree([
      '/login'
    ]);

  }

  // Check logged-in user's role
  if (
    authService.getUserRole() ===
    'Recruiter'
  ) {

    return true;

  }

  // Logged in but not a recruiter
  return router.createUrlTree([
    '/dashboard'
  ]);
};