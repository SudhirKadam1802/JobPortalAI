import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { Resume } from '../models/resume.models';

export interface ResumeAnalysisResult {
  id: string;
  resumeId: string;
  score: number;
  summary: string;
  skills: string[];
  education: string;
  experience: string;
  suggestions: string[];
  analyzedAt?: string;
}

interface ResumeSessionState {
  resumes: Resume[];
  selectedResume: Resume | null;
  uploadMessage: string;
  errorMessage: string;
  analysisMessage: string;
  analysisError: string;
  analysis: ResumeAnalysisResult | null;
}

const STORAGE_KEY = 'careerai_resume_session';

function createEmptyState(): ResumeSessionState {
  return {
    resumes: [],
    selectedResume: null,
    uploadMessage: '',
    errorMessage: '',
    analysisMessage: '',
    analysisError: '',
    analysis: null,
  };
}

@Injectable({
  providedIn: 'root'
})
export class ResumeStateService {
  private readonly resetSubject = new Subject<void>();
  readonly reset$ = this.resetSubject.asObservable();

  selectedFile: File | null = null;
  sessionVersion = 0;

  private state = this.restoreState();

  get resumes(): Resume[] {
    return this.state.resumes;
  }

  set resumes(value: Resume[]) {
    this.state.resumes = value;
    this.persistState();
  }

  get selectedResume(): Resume | null {
    return this.state.selectedResume;
  }

  set selectedResume(value: Resume | null) {
    this.state.selectedResume = value;
    this.persistState();
  }

  get uploadMessage(): string {
    return this.state.uploadMessage;
  }

  set uploadMessage(value: string) {
    this.state.uploadMessage = value;
    this.persistState();
  }

  get errorMessage(): string {
    return this.state.errorMessage;
  }

  set errorMessage(value: string) {
    this.state.errorMessage = value;
    this.persistState();
  }

  get analysisMessage(): string {
    return this.state.analysisMessage;
  }

  set analysisMessage(value: string) {
    this.state.analysisMessage = value;
    this.persistState();
  }

  get analysisError(): string {
    return this.state.analysisError;
  }

  set analysisError(value: string) {
    this.state.analysisError = value;
    this.persistState();
  }

  get analysis(): ResumeAnalysisResult | null {
    return this.state.analysis;
  }

  set analysis(value: ResumeAnalysisResult | null) {
    this.state.analysis = value;
    this.persistState();
  }

  reset(): void {
    this.selectedFile = null;
    this.state = createEmptyState();
    this.sessionVersion++;
    sessionStorage.removeItem(STORAGE_KEY);
    this.resetSubject.next();
  }

  private persistState(): void {
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(this.state));
  }

  private restoreState(): ResumeSessionState {
    const storedState = sessionStorage.getItem(STORAGE_KEY);

    if (!storedState) {
      return createEmptyState();
    }

    try {
      const parsedState: unknown = JSON.parse(storedState);

      if (this.isResumeSessionState(parsedState)) {
        return parsedState;
      }

      console.error('Stored resume session state is invalid.');
    } catch (error) {
      console.error('Unable to restore stored resume session state:', error);
    }

    sessionStorage.removeItem(STORAGE_KEY);
    return createEmptyState();
  }

  private isResumeSessionState(
    value: unknown
  ): value is ResumeSessionState {
    if (typeof value !== 'object' || value === null) {
      return false;
    }

    const state = value as Partial<ResumeSessionState>;

    return Array.isArray(state.resumes) &&
      (state.selectedResume === null ||
        typeof state.selectedResume === 'object') &&
      typeof state.uploadMessage === 'string' &&
      typeof state.errorMessage === 'string' &&
      typeof state.analysisMessage === 'string' &&
      typeof state.analysisError === 'string' &&
      (state.analysis === null || typeof state.analysis === 'object');
  }
}
