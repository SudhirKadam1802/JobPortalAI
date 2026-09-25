export interface SavedJob {
  id: string;
  jobId: string;
  jobTitle: string;
  location: string;
  employmentType: string;
  minimumExperience: number;
  maximumExperience: number;
  minimumSalary?: number | null;
  maximumSalary?: number | null;
  applicationDeadline: string;
  savedAt: string;
}

export interface SavedJobCheckResponse {
  jobId: string;
  isSaved: boolean;
}