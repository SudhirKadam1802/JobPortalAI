
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  AIConversation,
  AIMessage,
  CreateConversationRequest,
  SendMessageRequest
} from '../models/ai-chat.models';

@Injectable({
  providedIn: 'root'
})
export class AIChatService {
  private readonly apiUrl = 'https://localhost:7184/api/AIChat';

  constructor(private http: HttpClient) {}

  getConversations(): Observable<AIConversation[]> {
    return this.http.get<AIConversation[]>(
      `${this.apiUrl}/conversations`
    );
  }

  getConversation(id: string): Observable<AIConversation> {
    return this.http.get<AIConversation>(
      `${this.apiUrl}/conversations/${id}`
    );
  }

  createConversation(title?: string): Observable<AIConversation> {
    const request: CreateConversationRequest = {
      title: title?.trim() || null
    };

    return this.http.post<AIConversation>(
      `${this.apiUrl}/conversations`,
      request
    );
  }

  sendMessage(
    conversationId: string,
    message: string
  ): Observable<AIMessage> {
    const request: SendMessageRequest = {
      message
    };

    return this.http.post<AIMessage>(
      `${this.apiUrl}/conversations/${conversationId}/messages`,
      request
    );
  }

  // Delete a specific AI Chat conversation
  deleteConversation(
    conversationId: string
  ): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/conversations/${conversationId}`
    );
  }
}