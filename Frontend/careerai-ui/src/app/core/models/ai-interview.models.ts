export interface StartInterviewRequest {
  jobTitle: string;
  numberOfQuestions: number;
}

export interface StartInterviewResponse {
  interviewId: string;
  questionId: string;
  question: string;
  questionNumber: number;
  totalQuestions: number;
}

export interface SubmitAnswerRequest {
  questionId: string;
  answer: string;
}

export interface SubmitAnswerResponse {
  questionId: string;
  score: number;
  feedback: string;
  isInterviewCompleted: boolean;
  nextQuestionId?: string | null;
  nextQuestion?: string | null;
  nextQuestionNumber?: number | null;
  totalQuestions?: number | null;
}

export interface InterviewResultResponse {
  interviewId: string;
  totalScore: number;
  maximumScore: number;
  percentage: number;
  strengths: string;
  improvements: string;
}
