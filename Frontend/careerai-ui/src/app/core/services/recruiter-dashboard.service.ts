import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RecruiterDashboard } from '../models/recruiter-dashboard.models';

@Injectable({
  providedIn: 'root'
})
export class RecruiterDashboardService {

  private readonly apiUrl =
    'https://localhost:7184/api/RecruiterDashboard';

  constructor(
    private http: HttpClient
  ) {}

  getDashboard(): Observable<RecruiterDashboard> {
    return this.http.get<RecruiterDashboard>(
      this.apiUrl
    );
  }
}