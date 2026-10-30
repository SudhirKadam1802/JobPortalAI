import {
  DestroyRef,
  ElementRef,
  ChangeDetectorRef,
  Component,
  ViewChild
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import {
  DatePipe
} from '@angular/common';

import {
  finalize
} from 'rxjs';

import {
  ResumeService
} from '../../core/services/resume.service';
import {
  ResumeAnalysisResult,
  ResumeStateService
} from '../../core/services/resume-state.service';

import {
  Resume as ResumeModel
} from '../../core/models/resume.models';
import { Sidebar } from '../../shared/components/sidebar/sidebar';
import { Navbar } from '../../shared/components/navbar/navbar';


@Component({
  selector: 'app-resume',

  standalone: true,

  imports: [
    DatePipe,
    Sidebar,
    Navbar
  ],

  templateUrl: './resume.html',

  styleUrl: './resume.css'
})


export class Resume {

  @ViewChild('resumeFileInput')
  private resumeFileInput?: ElementRef<HTMLInputElement>;

  // =========================================================
  // RESUME DATA
  // =========================================================

  isUploading = false;

  isAnalyzing = false;

  private analysisLoadRequestId = 0;


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private resumeService: ResumeService,
    private resumeState: ResumeStateService,
    private changeDetectorRef: ChangeDetectorRef,
    destroyRef: DestroyRef
  ) {
    this.resumeState.reset$
      .pipe(takeUntilDestroyed(destroyRef))
      .subscribe(() => {
        if (this.resumeFileInput) {
          this.resumeFileInput.nativeElement.value = '';
        }

        this.isUploading = false;
        this.isAnalyzing = false;
        this.analysisLoadRequestId++;
      });
  }

  get resumes(): ResumeModel[] {
    return this.resumeState.resumes;
  }

  set resumes(value: ResumeModel[]) {
    this.resumeState.resumes = value;
  }

  get selectedFile(): File | null {
    return this.resumeState.selectedFile;
  }

  set selectedFile(value: File | null) {
    this.resumeState.selectedFile = value;
  }

  get selectedResume(): ResumeModel | null {
    return this.resumeState.selectedResume;
  }

  set selectedResume(value: ResumeModel | null) {
    this.resumeState.selectedResume = value;
  }

  get uploadMessage(): string {
    return this.resumeState.uploadMessage;
  }

  set uploadMessage(value: string) {
    this.resumeState.uploadMessage = value;
  }

  get errorMessage(): string {
    return this.resumeState.errorMessage;
  }

  set errorMessage(value: string) {
    this.resumeState.errorMessage = value;
  }

  get analysisMessage(): string {
    return this.resumeState.analysisMessage;
  }

  set analysisMessage(value: string) {
    this.resumeState.analysisMessage = value;
  }

  get analysisError(): string {
    return this.resumeState.analysisError;
  }

  set analysisError(value: string) {
    this.resumeState.analysisError = value;
  }

  get analysis(): ResumeAnalysisResult | null {
    return this.resumeState.analysis;
  }

  set analysis(value: ResumeAnalysisResult | null) {
    this.resumeState.analysis = value;
  }


  // =========================================================
  // SELECT PDF FILE
  // =========================================================

  onFileSelected(event: Event): void {

    const input =
      event.target as HTMLInputElement;


    if (
      !input.files ||
      input.files.length === 0
    ) {

      return;

    }


    const file =
      input.files[0];


    // Clear old messages

    this.uploadMessage = '';

    this.errorMessage = '';

    this.analysisMessage = '';

    this.analysisError = '';

    this.analysis = null;


    // =======================================================
    // PDF VALIDATION
    // =======================================================

    if (
      file.type !== 'application/pdf' &&
      !file.name
        .toLowerCase()
        .endsWith('.pdf')
    ) {

      this.errorMessage =
        'Please select a PDF file.';

      this.selectedFile = null;

      return;

    }


    // =======================================================
    // FILE SIZE VALIDATION
    // =======================================================

    if (
      file.size >
      5 * 1024 * 1024
    ) {

      this.errorMessage =
        'File size must be less than 5 MB.';

      this.selectedFile = null;

      return;

    }


    // =======================================================
    // STORE SELECTED FILE
    // =======================================================

    this.selectedFile = file;

    console.log(
      'File selected:',
      file.name
    );

  }


  // =========================================================
  // UPLOAD RESUME
  // =========================================================

  uploadResume(): void {

    if (!this.selectedFile) {

      this.errorMessage =
        'Please select a PDF file first.';

      return;

    }


    if (this.isUploading) {

      return;

    }


    const fileToUpload =
      this.selectedFile;
    const sessionVersion = this.resumeState.sessionVersion;


    this.isUploading = true;

    this.uploadMessage = '';

    this.errorMessage = '';

    this.analysisMessage = '';

    this.analysisError = '';

    this.analysis = null;


    console.log(
      'Uploading resume:',
      fileToUpload.name
    );


    this.resumeService

      .uploadResume(fileToUpload)

      .pipe(

        finalize(() => {

          console.log(
            'Upload request finished.'
          );

          this.isUploading = false;

          this.changeDetectorRef.detectChanges();

        })

      )

      .subscribe({

        next: (resume) => {

          if (sessionVersion !== this.resumeState.sessionVersion) {
            return;
          }

          console.log(
            'Resume uploaded successfully:',
            resume
          );


          this.uploadMessage =
            'Resume uploaded successfully.';

          this.selectedFile = null;

          this.analysisLoadRequestId++;
          this.resumes = [resume];
          this.selectedResume = resume;
          this.analysis = null;
          this.analysisMessage = '';
          this.analysisError = '';

          this.changeDetectorRef.detectChanges();

        },


        error: (error) => {

          if (sessionVersion !== this.resumeState.sessionVersion) {
            return;
          }

          console.error(
            'Resume upload failed:',
            error
          );


          this.errorMessage =
            error?.error?.message ??
            'Resume upload failed. Please try again.';

          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // SELECT UPLOADED RESUME
  // =========================================================

  selectResume(
    resume: ResumeModel
  ): void {

    this.selectedResume =
      resume;


    this.uploadMessage = '';

    this.errorMessage = '';

    this.analysisMessage = '';

    this.analysisError = '';

    this.analysis = null;

    this.loadSavedAnalysis(
      resume.id
    );


    this.changeDetectorRef.detectChanges();

  }


  // =========================================================
  // LOAD SAVED ANALYSIS
  // =========================================================

  loadSavedAnalysis(
    resumeId: string
  ): void {

    const requestId = ++this.analysisLoadRequestId;
    const sessionVersion = this.resumeState.sessionVersion;

    this.resumeService
      .getResumeAnalysis(resumeId)
      .subscribe({

        next: (result: any) => {

          if (
            requestId !== this.analysisLoadRequestId ||
            sessionVersion !== this.resumeState.sessionVersion ||
            this.selectedResume?.id !== resumeId
          ) {
            return;
          }

          this.setAnalysisResult(
            result,
            resumeId
          );

          this.analysisMessage =
            'Saved resume analysis loaded.';

          this.analysisError = '';

          this.changeDetectorRef.detectChanges();

        },

        error: (error) => {

          if (
            requestId !== this.analysisLoadRequestId ||
            sessionVersion !== this.resumeState.sessionVersion ||
            this.selectedResume?.id !== resumeId
          ) {
            return;
          }

          // A 404 means this resume has not been analyzed yet.
          if (error?.status === 404) {

            this.analysis = null;
            this.analysisMessage = '';
            this.analysisError = '';

            this.changeDetectorRef.detectChanges();

            return;

          }

          console.error(
            'Unable to load saved resume analysis:',
            error
          );

          this.analysis = null;
          this.analysisMessage = '';
          this.analysisError =
            'Unable to load the saved resume analysis.';

          this.changeDetectorRef.detectChanges();

        }

      });

  }


  // =========================================================
  // ANALYZE RESUME WITH AI
  // =========================================================

  analyzeResume(): void {

    // =======================================================
    // MAKE SURE RESUME IS SELECTED
    // =======================================================

    if (!this.selectedResume) {

      this.analysisError =
        'Please upload a resume first.';

      return;

    }


    // =======================================================
    // PREVENT DUPLICATE REQUESTS
    // =======================================================

    if (this.isAnalyzing) {

      return;

    }


    const resumeId =
      this.selectedResume.id;
    const sessionVersion = this.resumeState.sessionVersion;


    // =======================================================
    // START AI ANALYSIS
    // =======================================================

    this.isAnalyzing = true;

    this.analysisMessage = '';

    this.analysisError = '';

    this.analysis = null;


    console.log(
      'Starting AI resume analysis:',
      resumeId
    );


    this.changeDetectorRef.detectChanges();


    // =======================================================
    // CALL BACKEND
    // =======================================================

    this.resumeService

      .analyzeResume(resumeId)

      .pipe(

        finalize(() => {

          console.log(
            'AI analysis request finished.'
          );


          this.isAnalyzing = false;


          /*
           * Force Angular to update the UI.
           */
          this.changeDetectorRef.detectChanges();

        })

      )

      .subscribe({

        // ===================================================
        // SUCCESS
        // ===================================================

        next: (result: any) => {

          if (sessionVersion !== this.resumeState.sessionVersion) {
            return;
          }

          console.log(
            'Resume analysis completed:',
            result
          );


          this.setAnalysisResult(
            result,
            resumeId
          );


          console.log(
            'Normalized analysis:',
            this.analysis
          );


          // =================================================
          // SUCCESS MESSAGE
          // =================================================

          this.analysisMessage =
            'Resume analyzed successfully.';


          this.analysisError = '';


          // =================================================
          // FORCE UI UPDATE
          // =================================================

          this.changeDetectorRef.detectChanges();


          console.log(
            'Analysis displayed on UI:',
            this.analysis
          );

        },


        // ===================================================
        // ERROR
        // ===================================================

        error: (error) => {

          if (sessionVersion !== this.resumeState.sessionVersion) {
            return;
          }

          console.error(
            'Resume analysis failed:',
            error
          );

          this.analysisError = error?.name === 'TimeoutError'
            ? 'Resume analysis is taking longer than expected. Please try again.'
            : error?.error?.message ??
              'Resume analysis failed. Please try again.';


          this.analysisMessage = '';


          this.changeDetectorRef.detectChanges();

        }

      });

  }


  private setAnalysisResult(
    result: any,
    resumeId: string
  ): void {

    this.analysis = {

      id:
        result?.id ?? '',

      resumeId:
        result?.resumeId ?? resumeId,

      score:
        Number(result?.score ?? 0),

      summary:
        result?.summary ?? '',

      skills:
        Array.isArray(result?.skills)
          ? result.skills
          : this.convertToArray(
              result?.extractedSkills
            ),

      education:
        result?.education ??
        result?.extractedEducation ??
        '',

      experience:
        result?.experience ??
        result?.extractedExperience ??
        '',

      suggestions:
        Array.isArray(result?.suggestions)
          ? result.suggestions
          : this.convertToArray(
              result?.suggestions
            ),

      analyzedAt:
        result?.analyzedAt

    };

  }


  // =========================================================
  // CONVERT VALUE TO ARRAY
  // =========================================================

  private convertToArray(
    value: unknown
  ): string[] {

    if (Array.isArray(value)) {

      return value.map(
        item => String(item)
      );

    }


    if (
      typeof value === 'string' &&
      value.trim().length > 0
    ) {

      return value
        .split(',')
        .map(item => item.trim())
        .filter(item => item.length > 0);

    }


    return [];

  }

}