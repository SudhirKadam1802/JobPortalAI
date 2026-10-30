import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { vi } from 'vitest';
import { Resume as ResumeModel } from '../../core/models/resume.models';
import { AuthService } from '../../core/services/auth.service';
import { ResumeService } from '../../core/services/resume.service';
import { ResumeStateService } from '../../core/services/resume-state.service';
import { NotificationService } from '../../core/services/notification.service';
import { Resume } from './resume';

describe('Resume', () => {
  let component: Resume;
  let fixture: ComponentFixture<Resume>;
  let uploadResponse: Subject<ResumeModel>;
  let analysisResponse: Subject<unknown>;
  let resumeService: {
    uploadResume: ReturnType<typeof vi.fn>;
    getResumeAnalysis: ReturnType<typeof vi.fn>;
    analyzeResume: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    sessionStorage.clear();
    uploadResponse = new Subject<ResumeModel>();
    analysisResponse = new Subject<unknown>();
    resumeService = {
      uploadResume: vi.fn(() => uploadResponse),
      getResumeAnalysis: vi.fn(() => analysisResponse),
      analyzeResume: vi.fn(() => analysisResponse),
    };

    await TestBed.configureTestingModule({
      imports: [Resume],
      providers: [
        { provide: ResumeService, useValue: resumeService },
        { provide: AuthService, useValue: { getUserRole: () => null } },
        {
          provide: NotificationService,
          useValue: { getMyNotifications: () => of([]) }
        }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Resume);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    sessionStorage.clear();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('preserves the uploaded resume and analysis across route recreation and refresh', () => {
    const uploadedResume: ResumeModel = {
      id: 'resume-a',
      candidateId: 'candidate',
      fileName: 'resume-a.pdf',
      filePath: 'resumes/resume-a.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-29T10:00:00Z',
    };

    component.selectedFile = new File(['resume A'], 'resume-a.pdf', {
      type: 'application/pdf',
    });
    component.uploadResume();
    uploadResponse.next(uploadedResume);

    component.analyzeResume();
    analysisResponse.next({
      id: 'analysis-a',
      resumeId: uploadedResume.id,
      score: 85,
      summary: 'Resume A analysis',
      skills: ['Angular'],
      education: 'Degree',
      experience: 'Experience',
      suggestions: ['Add metrics'],
    });

    fixture.destroy();
    fixture = TestBed.createComponent(Resume);
    component = fixture.componentInstance;

    expect(component.selectedResume).toEqual(uploadedResume);
    expect(component.resumes).toEqual([uploadedResume]);
    expect(component.analysis?.score).toBe(85);
    expect(component.analysisMessage).toBe('Resume analyzed successfully.');

    const refreshedState = new ResumeStateService();
    expect(refreshedState.selectedResume).toEqual(uploadedResume);
    expect(refreshedState.analysis?.score).toBe(85);
    expect(refreshedState.selectedFile).toBeNull();
  });

  it('keeps a selected local PDF when the route component is recreated', () => {
    const selectedFile = new File(['resume'], 'selected.pdf', {
      type: 'application/pdf',
    });
    component.selectedFile = selectedFile;

    fixture.destroy();
    fixture = TestBed.createComponent(Resume);
    component = fixture.componentInstance;

    expect(component.selectedFile).toBe(selectedFile);
  });

  it('clears the native file input when resume state is reset', () => {
    fixture.detectChanges();

    const input = fixture.nativeElement.querySelector(
      'input[type="file"]'
    ) as HTMLInputElement;
    let inputValue = 'C:\\fakepath\\resume-a.pdf';
    Object.defineProperty(input, 'value', {
      configurable: true,
      get: () => inputValue,
      set: (value: string) => {
        inputValue = value;
      },
    });

    TestBed.inject(ResumeStateService).reset();

    expect(input.value).toBe('');
  });

  it('analyzes a newly uploaded resume without reusing the previous analysis', () => {
    const state = TestBed.inject(ResumeStateService);
    state.selectedResume = {
      id: 'resume-a',
      candidateId: 'candidate',
      fileName: 'resume-a.pdf',
      filePath: 'resumes/resume-a.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-29T10:00:00Z',
    };
    state.analysis = {
      id: 'analysis-a',
      resumeId: 'resume-a',
      score: 85,
      summary: 'Resume A analysis',
      skills: [],
      education: '',
      experience: '',
      suggestions: [],
    };
    state.reset();

    const uploadedResume: ResumeModel = {
      id: 'resume-b',
      candidateId: 'candidate',
      fileName: 'resume-b.pdf',
      filePath: 'resumes/resume-b.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-30T10:00:00Z',
    };
    component.selectedFile = new File(['resume B'], 'resume-b.pdf', {
      type: 'application/pdf',
    });
    component.uploadResume();
    uploadResponse.next(uploadedResume);

    component.analyzeResume();
    analysisResponse.next({
      id: 'analysis-b',
      resumeId: uploadedResume.id,
      score: 73,
      summary: 'Resume B analysis',
      skills: ['TypeScript'],
      education: '',
      experience: '',
      suggestions: [],
    });

    expect(component.selectedResume).toEqual(uploadedResume);
    expect(component.analysis?.resumeId).toBe(uploadedResume.id);
    expect(component.analysis?.score).toBe(73);
  });

  it('keeps the previous analysis visible when a retry fails', () => {
    const state = TestBed.inject(ResumeStateService);
    state.selectedResume = {
      id: 'resume-a',
      candidateId: 'candidate',
      fileName: 'resume-a.pdf',
      filePath: 'resumes/resume-a.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-29T10:00:00Z',
    };
    state.analysis = {
      id: 'analysis-a',
      resumeId: 'resume-a',
      score: 85,
      summary: 'Existing analysis',
      skills: ['Angular'],
      education: 'Degree',
      experience: 'Experience',
      suggestions: ['Add metrics'],
    };

    component.analyzeResume();
    analysisResponse.error({
      status: 504,
      error: { message: 'AI request timed out.' },
    });

    expect(component.analysis?.score).toBe(85);
    expect(component.analysisError).toBe('AI request timed out.');
  });
});
