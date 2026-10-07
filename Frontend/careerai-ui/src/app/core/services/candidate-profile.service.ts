import { environment } from '../../../environments/environment.development';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CandidateProfile,
  UpdateCandidateProfileRequest
} from '../models/candidate-profile.models';

@Injectable({
  providedIn: 'root'
})
export class CandidateProfileService {

  private readonly apiUrl =
    `${environment.apiUrl}/CandidateProfile`;
  constructor(
    private http: HttpClient
  ) {}

  getMyProfile(): Observable<CandidateProfile> {
    return this.http.get<CandidateProfile>(
      `${this.apiUrl}/me`
    );
  }

  updateMyProfile(
    request: UpdateCandidateProfileRequest
  ): Observable<CandidateProfile> {
    return this.http.put<CandidateProfile>(
      `${this.apiUrl}/me`,
      request
    );
  }
}