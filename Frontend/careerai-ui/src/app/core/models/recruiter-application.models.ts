export interface RecruiterApplication {
  id: string;

  candidateId: string;
  jobId: string;

  candidateName?: string | null;
  candidateEmail?: string | null;

  jobTitle?: string | null;

  status: number;
  statusName?: string | null;

  coverLetter?: string | null;

  appliedAt: string;
  updatedAt?: string | null;
}

export interface UpdateApplicationStatusRequest {
  status: number;
}