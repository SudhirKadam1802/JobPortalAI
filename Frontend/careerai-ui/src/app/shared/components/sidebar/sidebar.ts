import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  RouterLink,
  RouterLinkActive
} from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {

  constructor(
    private authService: AuthService
  ) {}

  get isRecruiter(): boolean {
    return this.authService.getUserRole() === 'Recruiter';
  }

  get isCandidate(): boolean {
    return this.authService.getUserRole() === 'Candidate';
  }

  logout(): void {

    this.authService.logout();

    window.location.href = '/login';
  }
}