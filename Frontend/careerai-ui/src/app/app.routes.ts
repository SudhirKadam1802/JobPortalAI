import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth-guard';
import { recruiterGuard } from './core/guards/recruiter-guard';

export const routes: Routes = [

  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login')
        .then(m => m.Login)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register')
        .then(m => m.Register)
  },

  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard')
        .then(m => m.Dashboard)
  },

  {
    path: 'resume',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/resume/resume')
        .then(m => m.Resume)
  },

  {
    path: 'jobs',
    loadComponent: () =>
      import('./features/jobs/job-list/job-list')
        .then(m => m.JobList)
  },

  {
    path: 'jobs/:id',
    loadComponent: () =>
      import('./features/jobs/job-details/job-details')
        .then(m => m.JobDetails)
  },

  {
    path: 'applications',
    loadComponent: () =>
      import('./features/applications/my-applications/my-applications')
        .then(m => m.MyApplications)
  },

  {
    path: 'saved-jobs',
    loadComponent: () =>
      import('./features/saved-jobs/saved-jobs/saved-jobs')
        .then(m => m.SavedJobs)
  },

  {
    path: 'job-alerts',
    loadComponent: () =>
      import('./features/job-alerts/job-alerts/job-alerts')
        .then(m => m.JobAlerts)
  },

  {
    path: 'profile',
    loadComponent: () =>
      import('./features/candidate-profile/candidate-profile/candidate-profile')
        .then(m => m.CandidateProfileComponent)
  },
  {
    path: 'ai-chat',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/ai-chat/ai-chat')
        .then(m => m.AiChat)
  },
  {
    path: 'interview',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/ai-interview/ai-interview')
        .then(m => m.AiInterview)
  },
  {
  path: 'recruiter/dashboard',
  canActivate: [recruiterGuard],
  loadComponent: () =>
    import('./features/recruiter/dashboard/recruiter-dashboard')
      .then(m => m.RecruiterDashboardComponent)
},

  {
    path: 'recruiter/jobs',
    canActivate: [recruiterGuard],
    loadComponent: () =>
      import('./features/recruiter/jobs/recruiter-jobs/recruiter-jobs')
        .then(m => m.RecruiterJobs)
  },
  {
  path: 'recruiter/jobs/:jobId/applications',
  canActivate: [recruiterGuard],
  loadComponent: () =>
    import('./features/recruiter/applications/recruiter-applications/recruiter-applications')
      .then(m => m.RecruiterApplications)
},
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  }

];