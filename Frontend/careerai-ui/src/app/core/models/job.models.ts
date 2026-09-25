export interface Job {
  id: string;
  recruiterId: string;

  title: string;
  description: string;
  location: string;

  employmentType: string;

  minimumExperience: number;
  maximumExperience: number;

  minimumSalary?: number | null;
  maximumSalary?: number | null;

  requiredEducation?: string | null;

  applicationDeadline: string;

  createdAt: string;
  updatedAt?: string | null;

  recruiter?: Recruiter;
  jobSkills?: JobSkill[];
}


export interface Recruiter {
  id: string;
  userId: string;

  companyName: string;

  companyDescription?: string | null;

  companyWebsite?: string | null;

  location?: string | null;
}


export interface JobSkill {
  id: string;

  jobId: string;

  skillId: string;

  skill?: Skill;
}


export interface Skill {
  id: string;

  name: string;
}