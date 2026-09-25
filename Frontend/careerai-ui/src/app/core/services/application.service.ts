import { Injectable } from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  Application,
  CreateApplicationRequest
} from '../models/application.models';


@Injectable({
  providedIn: 'root'
})
export class ApplicationService {

  // =========================================================
  // API URL
  // =========================================================

  private readonly apiUrl =
    'https://localhost:7184/api/Application';


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(
    private http: HttpClient
  ) {}


  // =========================================================
  // APPLY FOR JOB
  // =========================================================

  applyForJob(
    request: CreateApplicationRequest
  ): Observable<Application> {

    return this.http.post<Application>(
      this.apiUrl,
      request
    );

  }


  // =========================================================
  // GET MY APPLICATIONS
  // =========================================================

  getMyApplications(): Observable<Application[]> {

    return this.http.get<Application[]>(
      `${this.apiUrl}/my`
    );

  }


  // =========================================================
  // GET APPLICATION BY ID
  // =========================================================

  getApplicationById(
    id: string
  ): Observable<Application> {

    return this.http.get<Application>(
      `${this.apiUrl}/${id}`
    );

  }

}