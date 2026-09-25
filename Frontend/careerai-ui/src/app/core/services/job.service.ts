import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Job } from '../models/job.models';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  private readonly apiUrl =
    'https://localhost:7184/api/Job';

  constructor(
    private http: HttpClient
  ) {}


  // =========================================================
  // GET ALL JOBS
  // =========================================================

  getAllJobs(): Observable<Job[]> {

    return this.http.get<Job[]>(
      this.apiUrl
    );

  }


  // =========================================================
  // GET JOB BY ID
  // =========================================================

  getJobById(id: string): Observable<Job> {

    return this.http.get<Job>(
      `${this.apiUrl}/${id}`
    );

  }


  // =========================================================
  // CREATE JOB
  // =========================================================

  createJob(
    request: CreateJobRequest
  ): Observable<Job> {

    return this.http.post<Job>(
      this.apiUrl,
      request
    );

  }


  // =========================================================
  // UPDATE JOB
  // =========================================================

  updateJob(
    id: string,
    request: UpdateJobRequest
  ): Observable<Job> {

    return this.http.put<Job>(
      `${this.apiUrl}/${id}`,
      request
    );

  }


  // =========================================================
  // DELETE JOB
  // =========================================================

  deleteJob(
    id: string
  ): Observable<void> {

    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );

  }

}


// =========================================================
// CREATE JOB REQUEST
// =========================================================

export interface CreateJobRequest {

  title: string;

  description: string;

  location: string;

  employmentType: string;

  minimumExperience: number;

  maximumExperience: number;

  minimumSalary: number | null;

  maximumSalary: number | null;

  requiredEducation: string | null;

  applicationDeadline: string;

}


// =========================================================
// UPDATE JOB REQUEST
// =========================================================

export interface UpdateJobRequest {

  title: string;

  description: string;

  location: string;

  employmentType: string;

  minimumExperience: number;

  maximumExperience: number;

  minimumSalary: number | null;

  maximumSalary: number | null;

  requiredEducation: string | null;

  applicationDeadline: string;

}