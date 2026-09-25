import {
  ChangeDetectorRef,
  Component
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Sidebar } from '../../shared/components/sidebar/sidebar';
import { Navbar } from '../../shared/components/navbar/navbar';
import { AIInterviewService } from '../../core/services/ai-interview.service';
import {
  InterviewResultResponse,
  StartInterviewResponse,
  SubmitAnswerResponse
} from '../../core/models/ai-interview.models';

type InterviewStage = 'setup' | 'question' | 'feedback' | 'result';

@Component({
  selector: 'app-ai-interview',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Sidebar,
    Navbar
  ],
  templateUrl: './ai-interview.html',
  styleUrl: './ai-interview.css'
})
export class AiInterview {
  stage: InterviewStage = 'setup';
  jobTitle = '';
  numberOfQuestions = 5;
  answer = '';
  errorMessage = '';
  isLoading = false;
  isSubmitting = false;

  interviewId = '';
  questionId = '';
  question = '';
  questionNumber = 0;
  totalQuestions = 0;
  lastResponse: SubmitAnswerResponse | null = null;
  result: InterviewResultResponse | null = null;

  constructor(
    private interviewService: AIInterviewService,
    private cdr: ChangeDetectorRef
  ) {}

  startInterview(): void {
    this.errorMessage = '';
    const trimmedTitle = this.jobTitle.trim();

    if (!trimmedTitle) {
      this.errorMessage = 'Enter a job title to start the interview.';
      return;
    }

    if (this.numberOfQuestions < 1 || this.numberOfQuestions > 10) {
      this.errorMessage = 'Choose between 1 and 10 questions.';
      return;
    }

    this.isLoading = true;
    this.interviewService.startInterview({
      jobTitle: trimmedTitle,
      numberOfQuestions: this.numberOfQuestions
    }).subscribe({
      next: (response) => {
        this.applyQuestion(response);
        this.stage = 'question';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error?.error?.message ??
          'Unable to start the AI interview. Check that Ollama is available.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  submitAnswer(): void {
    const trimmedAnswer = this.answer.trim();

    if (!trimmedAnswer || this.isSubmitting || !this.interviewId) {
      return;
    }

    this.errorMessage = '';
    this.isSubmitting = true;

    this.interviewService.submitAnswer(
      this.interviewId,
      {
        questionId: this.questionId,
        answer: trimmedAnswer
      }
    ).subscribe({
      next: (response) => {
        this.lastResponse = response;
        this.answer = '';
        this.isSubmitting = false;

        if (response.isInterviewCompleted) {
          this.loadResult();
        } else {
          this.stage = 'feedback';
          this.cdr.detectChanges();
        }
      },
      error: (error) => {
        this.errorMessage =
          error?.error?.message ??
          'Unable to evaluate this answer. Please try again.';
        this.isSubmitting = false;
        this.cdr.detectChanges();
      }
    });
  }

  continueToNextQuestion(): void {
    if (!this.lastResponse?.nextQuestionId || !this.lastResponse.nextQuestion) {
      return;
    }

    this.questionId = this.lastResponse.nextQuestionId;
    this.question = this.lastResponse.nextQuestion;
    this.questionNumber = this.lastResponse.nextQuestionNumber ?? this.questionNumber + 1;
    this.totalQuestions = this.lastResponse.totalQuestions ?? this.totalQuestions;
    this.lastResponse = null;
    this.errorMessage = '';
    this.stage = 'question';
  }

  restartInterview(): void {
    this.stage = 'setup';
    this.interviewId = '';
    this.questionId = '';
    this.question = '';
    this.questionNumber = 0;
    this.totalQuestions = 0;
    this.answer = '';
    this.lastResponse = null;
    this.result = null;
    this.errorMessage = '';
  }

  private loadResult(): void {
    this.interviewService.getResult(this.interviewId).subscribe({
      next: (result) => {
        this.result = result;
        this.stage = 'result';
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error?.error?.message ??
          'Interview completed, but the final result could not be loaded.';
        this.isSubmitting = false;
        this.cdr.detectChanges();
      }
    });
  }

  private applyQuestion(response: StartInterviewResponse): void {
    this.interviewId = response.interviewId;
    this.questionId = response.questionId;
    this.question = response.question;
    this.questionNumber = response.questionNumber;
    this.totalQuestions = response.totalQuestions;
    this.answer = '';
    this.lastResponse = null;
    this.result = null;
  }
}
