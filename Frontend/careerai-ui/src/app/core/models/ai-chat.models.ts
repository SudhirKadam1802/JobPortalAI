export interface AIMessage {
  id: string;
  conversationId: string;
  role: string;
  content: string;
  createdAt: string;
}

export interface AIConversation {
  id: string;
  title: string;
  createdAt: string;
  updatedAt?: string | null;
  messages: AIMessage[];
}

export interface CreateConversationRequest {
  title?: string | null;
}

export interface SendMessageRequest {
  message: string;
}
