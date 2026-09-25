import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import {
  AuthResponse,
  LoginRequest,
  RegisterRequest
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiUrl =
    'https://localhost:7184/api/Auth';

  constructor(
    private http: HttpClient
  ) {}

  login(
    request: LoginRequest
  ): Observable<AuthResponse> {

    console.log('Login request:', request);

    return this.http
      .post<AuthResponse>(
        `${this.apiUrl}/login`,
        request
      )
      .pipe(
        tap(response => {

          console.log('Login response:', response);

          if (!response || !response.token) {
            console.error(
              'Login response does not contain a token.',
              response
            );

            throw new Error(
              'Login response does not contain a token.'
            );
          }

          localStorage.setItem(
            'careerai_token',
            response.token
          );

          console.log(
            'Token stored successfully.'
          );

          console.log(
            'User role:',
            this.getUserRole()
          );
        })
      );
  }

  register(
    request: RegisterRequest
  ): Observable<AuthResponse> {

    return this.http
      .post<AuthResponse>(
        `${this.apiUrl}/register`,
        request
      )
      .pipe(
        tap(response => {

          console.log(
            'Register response:',
            response
          );

          if (!response || !response.token) {
            throw new Error(
              'Registration response does not contain a token.'
            );
          }

          localStorage.setItem(
            'careerai_token',
            response.token
          );
        })
      );
  }

  getToken(): string | null {

    return localStorage.getItem(
      'careerai_token'
    );
  }

  isLoggedIn(): boolean {

    return !!this.getToken();
  }

  getUserRole(): string | null {

    const token = this.getToken();

    if (!token) {
      return null;
    }

    try {

      const parts = token.split('.');

      if (parts.length !== 3) {
        console.error(
          'Invalid JWT format.'
        );

        return null;
      }

      const payload = parts[1];

      const decodedPayload =
        JSON.parse(
          atob(
            payload
              .replace(/-/g, '+')
              .replace(/_/g, '/')
          )
        );

      const role =
        decodedPayload.role ??
        decodedPayload[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ??
        null;

      return role;

    } catch (error) {

      console.error(
        'Failed to decode JWT:',
        error
      );

      return null;
    }
  }

  logout(): void {

    localStorage.removeItem(
      'careerai_token'
    );
  }
}