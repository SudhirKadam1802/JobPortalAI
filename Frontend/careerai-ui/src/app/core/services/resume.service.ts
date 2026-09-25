import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Resume } from '../models/resume.models';

@Injectable({
  providedIn: 'root'
})
export class ResumeService {

  private readonly apiUrl =
    'https://localhost:7184/api/Resume';

  constructor(
    private http: HttpClient
  ) {}

  uploadResume(file: File): Observable<Resume> {

    const formData = new FormData();

    formData.append('file', file);

    return this.http.post<Resume>(
      this.apiUrl,
      formData
    );
  }

  getMyResumes(): Observable<Resume[]> {

    return this.http.get<Resume[]>(
      `${this.apiUrl}/my`
    );
  }

  getResumeById(id: string): Observable<Resume> {

    return this.http.get<Resume>(
      `${this.apiUrl}/${id}`
    );
  }

  analyzeResume(id: string): Observable<any> {

    return this.http.post<any>(
      `${this.apiUrl}/${id}/analyze`,
      {}
    );
  }

  getResumeAnalysis(id: string): Observable<any> {

    return this.http.get<any>(
      `${this.apiUrl}/${id}/analysis`
    );
  }
}