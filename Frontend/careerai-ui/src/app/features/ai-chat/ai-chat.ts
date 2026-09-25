
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Sidebar } from '../../shared/components/sidebar/sidebar';
import { Navbar } from '../../shared/components/navbar/navbar';

import { AIChatService } from '../../core/services/ai-chat.service';

import {
  AIConversation,
  AIMessage
} from '../../core/models/ai-chat.models';

interface MarkdownInline {
  type: 'text' | 'strong' | 'code';
  value: string;
}

interface MarkdownBlock {
  type:
    | 'heading'
    | 'paragraph'
    | 'unordered-list'
    | 'ordered-list'
    | 'code'
    | 'divider';

  level?: number;
  tokens?: MarkdownInline[];
  items?: MarkdownInline[][];
  code?: string;
  language?: string;
}

@Component({
  selector: 'app-ai-chat',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Sidebar,
    Navbar
  ],
  templateUrl: './ai-chat.html',
  styleUrl: './ai-chat.css'
})
export class AiChat implements OnInit {

  conversations: AIConversation[] = [];

  selectedConversation: AIConversation | null = null;

  draftMessage = '';

  isLoading = false;

  isSending = false;

  errorMessage = '';

  // Delete conversation state
  isDeleting = false;

  deletingConversationId: string | null = null;

  successMessage = '';

  private readonly markdownCache =
    new Map<string, MarkdownBlock[]>();

  constructor(
    private aiChatService: AIChatService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadConversations();
  }

  // ========================================
  // Load Conversations
  // ========================================

  loadConversations(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.aiChatService.getConversations().subscribe({

      next: (conversations) => {

        this.conversations = conversations;

        this.selectedConversation =
          conversations[0] ?? null;

        this.isLoading = false;

        this.cdr.detectChanges();

      },

      error: (error) => {

        this.errorMessage =
          error?.error?.message ??
          'Unable to load your career conversations.';

        this.isLoading = false;

        this.cdr.detectChanges();

      }

    });

  }

  // ========================================
  // Select Conversation
  // ========================================

  selectConversation(
    conversation: AIConversation
  ): void {

    this.selectedConversation = conversation;

    this.errorMessage = '';
    this.successMessage = '';

  }

  // ========================================
  // Delete Conversation
  // ========================================

  deleteConversation(
    conversation: AIConversation
  ): void {

    if (this.isDeleting || this.isSending) {
      return;
    }

    const confirmed = confirm(
      `Are you sure you want to delete "${conversation.title}"?`
    );

    if (!confirmed) {
      return;
    }

    this.isDeleting = true;

    this.deletingConversationId = conversation.id;

    this.errorMessage = '';
    this.successMessage = '';

    this.aiChatService
      .deleteConversation(conversation.id)
      .subscribe({

        next: () => {

          // Remove the conversation from history
          this.conversations = this.conversations.filter(
            item => item.id !== conversation.id
          );

          // If the deleted conversation was selected,
          // select the next available conversation
          if (
            this.selectedConversation?.id === conversation.id
          ) {

            this.selectedConversation =
              this.conversations[0] ?? null;

          }

          // Clear cached Markdown for deleted messages
          conversation.messages.forEach(message => {
            this.markdownCache.delete(message.id);
          });

          this.successMessage =
            'Conversation deleted successfully.';

          this.isDeleting = false;

          this.deletingConversationId = null;

          this.cdr.detectChanges();

        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to delete this conversation.';

          this.isDeleting = false;

          this.deletingConversationId = null;

          this.cdr.detectChanges();

        }

      });

  }

  // ========================================
  // Create Conversation
  // ========================================

  createConversation(): void {

    if (this.isSending || this.isDeleting) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.aiChatService.createConversation().subscribe({

      next: (conversation) => {

        this.conversations = [
          conversation,
          ...this.conversations
        ];

        this.selectedConversation = conversation;

        this.draftMessage = '';

        this.cdr.detectChanges();

      },

      error: (error) => {

        this.errorMessage =
          error?.error?.message ??
          'Unable to start a new career conversation.';

        this.cdr.detectChanges();

      }

    });

  }

  // ========================================
  // Send Message
  // ========================================

  sendMessage(): void {

    const message = this.draftMessage.trim();

    if (!message || this.isSending || this.isDeleting) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.isSending = true;

    if (this.selectedConversation) {

      this.sendToConversation(
        this.selectedConversation,
        message
      );

      return;

    }

    this.aiChatService.createConversation().subscribe({

      next: (conversation) => {

        this.conversations = [
          conversation,
          ...this.conversations
        ];

        this.selectedConversation = conversation;

        this.sendToConversation(
          conversation,
          message
        );

      },

      error: (error) => {

        this.isSending = false;

        this.errorMessage =
          error?.error?.message ??
          'Unable to start a new career conversation.';

        this.cdr.detectChanges();

      }

    });

  }

  // ========================================
  // Send Message To Conversation
  // ========================================

  private sendToConversation(
    conversation: AIConversation,
    message: string
  ): void {

    const userMessage: AIMessage = {

      id: `local-${Date.now()}`,

      conversationId: conversation.id,

      role: 'User',

      content: message,

      createdAt: new Date().toISOString()

    };

    conversation.messages = [
      ...conversation.messages,
      userMessage
    ];

    this.draftMessage = '';

    this.cdr.detectChanges();

    this.aiChatService
      .sendMessage(conversation.id, message)
      .subscribe({

        next: (assistantMessage) => {

          conversation.messages = [
            ...conversation.messages,
            assistantMessage
          ];

          conversation.updatedAt =
            assistantMessage.createdAt;

          this.isSending = false;

          this.cdr.detectChanges();

        },

        error: (error) => {

          conversation.messages =
            conversation.messages.filter(
              item => item.id !== userMessage.id
            );

          this.isSending = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to get a response from CareerAI.';

          this.cdr.detectChanges();

        }

      });

  }

  // ========================================
  // Check Assistant Message
  // ========================================

  isAssistant(message: AIMessage): boolean {

    return message.role.toLowerCase() === 'assistant';

  }

  // ========================================
  // Markdown Rendering
  // ========================================

  getMarkdownBlocks(
    message: AIMessage
  ): MarkdownBlock[] {

    const cached = this.markdownCache.get(message.id);

    if (cached) {
      return cached;
    }

    const blocks = this.parseMarkdown(message.content);

    this.markdownCache.set(message.id, blocks);

    return blocks;

  }

  private parseMarkdown(
    content: string
  ): MarkdownBlock[] {

    const lines = content
      .replace(/\r\n?/g, '\n')
      .split('\n');

    const blocks: MarkdownBlock[] = [];

    let paragraphLines: string[] = [];

    let index = 0;

    const flushParagraph = (): void => {

      if (paragraphLines.length === 0) {
        return;
      }

      blocks.push({

        type: 'paragraph',

        tokens: this.parseInline(
          paragraphLines.join('\n')
        )

      });

      paragraphLines = [];

    };

    while (index < lines.length) {

      const rawLine = lines[index];

      const line =
        this.unescapeMarkdownOutsideCode(rawLine);

      // Code blocks
      const fence = line.match(
        /^\s*```(.*)$/
      );

      if (fence) {

        flushParagraph();

        const codeLines: string[] = [];

        const language = fence[1].trim();

        index++;

        while (
          index < lines.length &&
          !/^\s*```\s*$/.test(lines[index])
        ) {

          codeLines.push(lines[index]);

          index++;

        }

        if (index < lines.length) {
          index++;
        }

        blocks.push({

          type: 'code',

          code: codeLines.join('\n'),

          language

        });

        continue;

      }

      // Empty lines
      if (!line.trim()) {

        flushParagraph();

        index++;

        continue;

      }

      // Headings
      const heading = line.match(
        /^\s{0,3}(#{1,6})\s+(.+?)\s*#*\s*$/
      );

      if (heading) {

        flushParagraph();

        blocks.push({

          type: 'heading',

          level: heading[1].length,

          tokens: this.parseInline(heading[2])

        });

        index++;

        continue;

      }

      // Horizontal divider
      if (
        /^\s*(---+|___+|\*\s*\*\s*\*+)\s*$/.test(line)
      ) {

        flushParagraph();

        blocks.push({
          type: 'divider'
        });

        index++;

        continue;

      }

      // Lists
      const unordered = line.match(
        /^\s*[-*+]\s+(.+)$/
      );

      const ordered = line.match(
        /^\s*\d+[.)]\s+(.+)$/
      );

      if (unordered || ordered) {

        flushParagraph();

        const items: MarkdownInline[][] = [];

        const listType =
          unordered
            ? 'unordered-list'
            : 'ordered-list';

        while (index < lines.length) {

          const listLine =
            this.unescapeMarkdownOutsideCode(
              lines[index]
            );

          const item = listLine.match(

            listType === 'unordered-list'
              ? /^\s*[-*+]\s+(.+)$/
              : /^\s*\d+[.)]\s+(.+)$/

          );

          if (!item) {
            break;
          }

          items.push(
            this.parseInline(item[1])
          );

          index++;

        }

        blocks.push({

          type: listType,

          items

        });

        continue;

      }

      paragraphLines.push(line);

      index++;

    }

    flushParagraph();

    return blocks;

  }

  // ========================================
  // Parse Inline Markdown
  // ========================================

  private parseInline(
    value: string
  ): MarkdownInline[] {

    const tokens: MarkdownInline[] = [];

    const pattern =
      /(\*\*|__)(.+?)\1|`([^`]+)`/g;

    let lastIndex = 0;

    let match: RegExpExecArray | null;

    while (
      (match = pattern.exec(value)) !== null
    ) {

      if (match.index > lastIndex) {

        tokens.push({

          type: 'text',

          value: value.slice(
            lastIndex,
            match.index
          )

        });

      }

      tokens.push({

        type: match[3]
          ? 'code'
          : 'strong',

        value: match[3] ?? match[2]

      });

      lastIndex = pattern.lastIndex;

    }

    if (lastIndex < value.length) {

      tokens.push({

        type: 'text',

        value: value.slice(lastIndex)

      });

    }

    return tokens.length > 0
      ? tokens
      : [{ type: 'text', value }];

  }

  // ========================================
  // Unescape Markdown
  // ========================================

  private unescapeMarkdownOutsideCode(
    value: string
  ): string {

    return value.replace(
      /\\([\\`*_{}\[\]()#+.!-])/g,
      '$1'
    );

  }

  // ========================================
  // Track Conversation
  // ========================================

  trackConversation(
    _index: number,
    conversation: AIConversation
  ): string {

    return conversation.id;

  }

  // ========================================
  // Track Message
  // ========================================

  trackMessage(
    _index: number,
    message: AIMessage
  ): string {

    return message.id;

  }

}