import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  InterviewResultResponse,
  StartInterviewRequest,
  StartInterviewResponse,
  SubmitAnswerRequest,
  SubmitAnswerResponse
} from '../models/ai-interview.models';

@Injectable({
  providedIn: 'root'
})
export class AIInterviewService {
  private readonly apiUrl = 'https://localhost:7184/api/AIInterview';

  constructor(private http: HttpClient) {}

  startInterview(
    request: StartInterviewRequest
  ): Observable<StartInterviewResponse> {
    return this.http.post<StartInterviewResponse>(
      `${this.apiUrl}/start`,
      request
    );
  }

  submitAnswer(
    interviewId: string,
    request: SubmitAnswerRequest
  ): Observable<SubmitAnswerResponse> {
    return this.http.post<SubmitAnswerResponse>(
      `${this.apiUrl}/${interviewId}/answer`,
      request
    );
  }

  getResult(interviewId: string): Observable<InterviewResultResponse> {
    return this.http.get<InterviewResultResponse>(
      `${this.apiUrl}/${interviewId}/result`
    );
  }
}
