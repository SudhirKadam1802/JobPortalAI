
import {
  Component,
  OnInit,
  HostListener,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Notification } from '../../../core/models/notification.models';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar implements OnInit {

  notifications: Notification[] = [];
  unreadCount = 0;
  showNotifications = false;
  loadingNotifications = false;

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadNotifications();
  }

  get isRecruiter(): boolean {
    return this.authService.getUserRole() === 'Recruiter';
  }

  get userRoleName(): string {
    return this.isRecruiter ? 'Recruiter' : 'Candidate';
  }

  get userRoleDescription(): string {
    return this.isRecruiter ? 'Hiring Manager' : 'Job Seeker';
  }

  loadNotifications(): void {
    this.loadingNotifications = true;
    this.cdr.detectChanges();

    this.notificationService.getMyNotifications().subscribe({

      next: (response) => {
        console.log('Notifications received:', response);

        this.notifications = response;
        this.updateUnreadCount();

        this.loadingNotifications = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Error loading notifications:', error);

        this.loadingNotifications = false;

        this.cdr.detectChanges();
      }

    });
  }

  updateUnreadCount(): void {
    this.unreadCount = this.notifications.filter(
      notification => !notification.isRead
    ).length;
  }

  toggleNotifications(): void {
    this.showNotifications = !this.showNotifications;

    if (this.showNotifications) {
      this.loadNotifications();
    }
  }

  markAsRead(notification: Notification): void {

    if (notification.isRead) {
      return;
    }

    this.notificationService.markAsRead(notification.id).subscribe({

      next: () => {
        notification.isRead = true;

        this.updateUnreadCount();

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Error marking notification as read:', error);
      }

    });
  }

  @HostListener('document:click', ['$event'])
  closeNotifications(event: MouseEvent): void {

    const target = event.target as HTMLElement;

    if (!target.closest('.notification-container')) {
      this.showNotifications = false;
    }
  }

  logout(): void {
    this.authService.logout();
    window.location.href = '/login';
  }
}