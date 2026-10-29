import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { vi } from 'vitest';
import { Resume as ResumeModel } from '../../core/models/resume.models';
import { ResumeService } from '../../core/services/resume.service';
import { Resume } from './resume';

describe('Resume', () => {
  let component: Resume;
  let fixture: ComponentFixture<Resume>;
  let resumesResponse: Subject<ResumeModel[]>;
  let uploadResponse: Subject<ResumeModel>;
  let resumeService: {
    uploadResume: ReturnType<typeof vi.fn>;
    getMyResumes: ReturnType<typeof vi.fn>;
    getResumeAnalysis: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    resumesResponse = new Subject<ResumeModel[]>();
    uploadResponse = new Subject<ResumeModel>();
    resumeService = {
      uploadResume: vi.fn(() => uploadResponse),
      getMyResumes: vi.fn(() => resumesResponse),
      getResumeAnalysis: vi.fn(() => new Subject()),
    };

    await TestBed.configureTestingModule({
      imports: [Resume],
      providers: [
        { provide: ResumeService, useValue: resumeService }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Resume);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('keeps the uploaded resume selected when an older resume list request finishes later', () => {
    fixture.detectChanges();

    const previousResume: ResumeModel = {
      id: 'previous-resume',
      candidateId: 'candidate',
      fileName: 'previous.pdf',
      filePath: 'resumes/previous.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-28T10:00:00Z',
    };
    const uploadedResume: ResumeModel = {
      ...previousResume,
      id: 'new-resume',
      fileName: 'new.pdf',
      filePath: 'resumes/new.pdf',
      uploadedAt: '2026-10-29T10:00:00Z',
    };

    component.selectedFile = new File(['resume'], 'new.pdf', {
      type: 'application/pdf',
    });
    component.uploadResume();
    uploadResponse.next(uploadedResume);

    resumesResponse.next([previousResume]);

    expect(component.selectedResume).toEqual(uploadedResume);
    expect(component.resumes).toEqual([uploadedResume]);
    expect(resumeService.getMyResumes).toHaveBeenCalledTimes(1);
  });

  it('selects the most recently uploaded resume when reopening the page', () => {
    fixture.detectChanges();

    const olderResume: ResumeModel = {
      id: 'older-resume',
      candidateId: 'candidate',
      fileName: 'older.pdf',
      filePath: 'resumes/older.pdf',
      fileType: '.pdf',
      uploadedAt: '2026-10-28T10:00:00Z',
    };
    const latestResume: ResumeModel = {
      ...olderResume,
      id: 'latest-resume',
      fileName: 'latest.pdf',
      filePath: 'resumes/latest.pdf',
      uploadedAt: '2026-10-29T10:00:00Z',
    };

    resumesResponse.next([olderResume, latestResume]);

    expect(component.selectedResume).toEqual(latestResume);
    expect(component.resumes).toEqual([latestResume]);
    expect(resumeService.getResumeAnalysis).toHaveBeenCalledWith(
      latestResume.id
    );
  });
});
