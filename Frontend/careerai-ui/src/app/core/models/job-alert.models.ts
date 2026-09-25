export interface CreateJobAlertRequest {
  keyword: string | null;
  location: string | null;
  minimumSalary: number | null;
  isActive: boolean;
}

export interface UpdateJobAlertRequest {
  keyword: string | null;
  location: string | null;
  minimumSalary: number | null;
  isActive: boolean;
}

export interface JobAlert {
  id: string;
  keyword: string;
  location: string | null;
  minimumSalary: number | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}