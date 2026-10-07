import { environment } from '../../../environments/environment.development';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  RecruiterApplication,
  UpdateApplicationStatusRequest
} from '../models/recruiter-application.models';

@Injectable({
  providedIn: 'root'
})
export class RecruiterApplicationService {

  private readonly apiUrl =
    `${environment.apiUrl}/Application`;

  constructor(
    private http: HttpClient
  ) {}

  getJobApplications(
    jobId: string
  ): Observable<RecruiterApplication[]> {

    return this.http.get<RecruiterApplication[]>(
      `${this.apiUrl}/job/${jobId}`
    );
  }

  updateApplicationStatus(
    applicationId: string,
    request: UpdateApplicationStatusRequest
  ): Observable<RecruiterApplication> {

    return this.http.put<RecruiterApplication>(
      `${this.apiUrl}/${applicationId}/status`,
      request
    );
  }

  getApplicationById(
    applicationId: string
  ): Observable<RecruiterApplication> {

    return this.http.get<RecruiterApplication>(
      `${this.apiUrl}/${applicationId}`
    );
  }
}