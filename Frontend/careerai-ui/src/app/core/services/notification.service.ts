
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Notification } from '../models/notification.models';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private apiUrl = 'https://localhost:7184/api/Notification';

  constructor(private http: HttpClient) {}

  // Get all notifications for the logged-in user
  getMyNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>(
      `${this.apiUrl}/my`
    );
  }

  // Mark a notification as read
  markAsRead(id: string): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/${id}/read`,
      {}
    );
  }
}