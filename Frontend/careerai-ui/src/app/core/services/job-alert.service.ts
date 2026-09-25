import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CreateJobAlertRequest,
  UpdateJobAlertRequest,
  JobAlert
} from '../models/job-alert.models';

@Injectable({
  providedIn: 'root'
})
export class JobAlertService {

  private readonly apiUrl =
    'https://localhost:7184/api/JobAlert';

  constructor(private http: HttpClient) {}

  createAlert(
    request: CreateJobAlertRequest
  ): Observable<JobAlert> {
    return this.http.post<JobAlert>(
      this.apiUrl,
      request
    );
  }

  getMyAlerts(): Observable<JobAlert[]> {
    return this.http.get<JobAlert[]>(
      `${this.apiUrl}/my`
    );
  }

  getAlertById(
    id: string
  ): Observable<JobAlert> {
    return this.http.get<JobAlert>(
      `${this.apiUrl}/${id}`
    );
  }

  updateAlert(
    id: string,
    request: UpdateJobAlertRequest
  ): Observable<JobAlert> {
    return this.http.put<JobAlert>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deleteAlert(
    id: string
  ): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/${id}`
    );
  }
}