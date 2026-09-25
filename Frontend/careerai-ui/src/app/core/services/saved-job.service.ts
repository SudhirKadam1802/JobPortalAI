import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  SavedJob,
  SavedJobCheckResponse
} from '../models/saved-job.models';

@Injectable({
  providedIn: 'root'
})
export class SavedJobService {

  private readonly apiUrl = 'https://localhost:7184/api/SavedJob';

  constructor(private http: HttpClient) {}

  saveJob(jobId: string): Observable<SavedJob> {
    return this.http.post<SavedJob>(
      `${this.apiUrl}/${jobId}`,
      {}
    );
  }

  getMySavedJobs(): Observable<SavedJob[]> {
    return this.http.get<SavedJob[]>(
      `${this.apiUrl}/my`
    );
  }

  checkSavedJob(jobId: string): Observable<SavedJobCheckResponse> {
    return this.http.get<SavedJobCheckResponse>(
      `${this.apiUrl}/check/${jobId}`
    );
  }

  removeSavedJob(jobId: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/${jobId}`
    );
  }
}