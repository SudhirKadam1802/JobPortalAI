// =========================================================
// CREATE APPLICATION REQUEST
// =========================================================

export interface CreateApplicationRequest {

  jobId: string;

  coverLetter: string | null;

}


// =========================================================
// APPLICATION RESPONSE
// =========================================================

export interface Application {

  id: string;

  candidateId: string;

  jobId: string;


  // Candidate information

  candidateName?: string | null;

  candidateEmail?: string | null;


  // Job information

  jobTitle?: string | null;


  // Application information

  status: number;

  statusName?: string | null;

  coverLetter?: string | null;

  appliedAt: string;

  updatedAt?: string | null;

}