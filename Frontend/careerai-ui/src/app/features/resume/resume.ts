import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

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
  Resume as ResumeModel
} from '../../core/models/resume.models';
import { Sidebar } from '../../shared/components/sidebar/sidebar';
import { Navbar } from '../../shared/components/navbar/navbar';


interface ResumeAnalysisResult {

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


export class Resume implements OnInit {


  // =========================================================
  // RESUME DATA
  // =========================================================

  resumes: ResumeModel[] = [];

  selectedFile: File | null = null;

  selectedResume: ResumeModel | null = null;


  // =========================================================
  // UPLOAD STATE
  // =========================================================

  isUploading = false;

  uploadMessage = '';

  errorMessage = '';


  // =========================================================
  // AI ANALYSIS STATE
  // =========================================================

  isAnalyzing = false;

  analysisMessage = '';

  analysisError = '';

  analysis: ResumeAnalysisResult | null = null;


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private resumeService: ResumeService,

    private changeDetectorRef: ChangeDetectorRef
  ) {}


  // =========================================================
  // INITIAL LOAD
  // =========================================================

  ngOnInit(): void {

    this.loadResumes();

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

          console.log(
            'Resume uploaded successfully:',
            resume
          );


          this.uploadMessage =
            'Resume uploaded successfully.';


          this.selectedFile = null;


          this.loadResumes();


          this.changeDetectorRef.detectChanges();

        },


        error: (error) => {

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
  // LOAD RESUMES
  // =========================================================

  loadResumes(): void {

    this.resumeService

      .getMyResumes()

      .subscribe({

        next: (resumes) => {

          console.log(
            'Resumes received from backend:',
            resumes
          );


          const sortedResumes =
            [...resumes].sort(
              (a, b) =>
                new Date(b.uploadedAt).getTime() -
                new Date(a.uploadedAt).getTime()
            );


          // Show only latest resume

          this.resumes =
            sortedResumes.slice(0, 1);


          // Automatically select latest resume

          if (
            this.resumes.length > 0
          ) {

            this.selectedResume =
              this.resumes[0];

            this.loadSavedAnalysis(
              this.selectedResume.id
            );

          }
          else {

            this.selectedResume =
              null;

          }


          this.changeDetectorRef.detectChanges();

        },


        error: (error) => {

          console.error(
            'Unable to load resumes:',
            error
          );


          this.errorMessage =
            'Unable to load your resume.';


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

    this.resumeService
      .getResumeAnalysis(resumeId)
      .subscribe({

        next: (result: any) => {

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

          console.error(
            'Resume analysis failed:',
            error
          );


          this.analysisError =
            error?.error?.message ??
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