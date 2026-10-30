import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';
import { ResumeStateService } from './resume-state.service';

describe('AuthService', () => {
  let authService: AuthService;
  let resumeState: ResumeStateService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    sessionStorage.clear();
    localStorage.removeItem('careerai_token');

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ],
    });

    authService = TestBed.inject(AuthService);
    resumeState = TestBed.inject(ResumeStateService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
    sessionStorage.clear();
    localStorage.removeItem('careerai_token');
  });

  it('clears all temporary resume state on logout', () => {
    const resume = {
      id: 'resume-a',
      candidateId: 'candidate',
      fileName: 'resume-a.pdf',
      filePath: 'resumes/resume-a.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-29T10:00:00Z',
    };

    resumeState.selectedFile = new File(['resume A'], 'resume-a.pdf', {
      type: 'application/pdf',
    });
    resumeState.resumes = [resume];
    resumeState.selectedResume = resume;
    resumeState.uploadMessage = 'Resume uploaded successfully.';
    resumeState.errorMessage = 'Previous upload error';
    resumeState.analysis = {
      id: 'analysis-a',
      resumeId: resume.id,
      score: 85,
      summary: 'Resume A analysis',
      skills: ['Angular'],
      education: 'Degree',
      experience: 'Experience',
      suggestions: ['Add metrics'],
    };
    resumeState.analysisMessage = 'Resume analyzed successfully.';
    resumeState.analysisError = 'Previous analysis error';

    localStorage.setItem('careerai_token', 'test-token');
    authService.logout();

    expect(resumeState.selectedFile).toBeNull();
    expect(resumeState.resumes).toEqual([]);
    expect(resumeState.selectedResume).toBeNull();
    expect(resumeState.uploadMessage).toBe('');
    expect(resumeState.errorMessage).toBe('');
    expect(resumeState.analysis).toBeNull();
    expect(resumeState.analysisMessage).toBe('');
    expect(resumeState.analysisError).toBe('');
    expect(sessionStorage.getItem('careerai_resume_session')).toBeNull();
    expect(localStorage.getItem('careerai_token')).toBeNull();
  });

  it('starts a new resume session after a successful login', () => {
    resumeState.selectedResume = {
      id: 'resume-a',
      candidateId: 'candidate',
      fileName: 'resume-a.pdf',
      filePath: 'resumes/resume-a.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-29T10:00:00Z',
    };
    resumeState.analysisMessage = 'Resume analyzed successfully.';

    authService.login({
      email: 'candidate@example.com',
      password: 'password',
    }).subscribe();

    httpTestingController
      .expectOne(`${environment.apiUrl}/Auth/login`)
      .flush({
        token: 'header.eyJyb2xlIjoiQ2FuZGlkYXRlIn0.sig',
      });

    expect(resumeState.selectedResume).toBeNull();
    expect(resumeState.analysisMessage).toBe('');
    expect(sessionStorage.getItem('careerai_resume_session')).toBeNull();
  });
});
