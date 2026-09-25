export interface CandidateProfile {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string | null;
  location: string | null;
  bio: string | null;
  dateOfBirth: string | null;
}

export interface UpdateCandidateProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber: string | null;
  location: string | null;
  bio: string | null;
  dateOfBirth: string | null;
}