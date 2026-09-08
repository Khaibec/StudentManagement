export interface ClassDto {
  id: number;
  name: string;
  description?: string;
  studentCount: number;
}

export interface CreateClassDto {
  name: string;
  description?: string;
}

export interface UpdateClassDto {
  name: string;
  description?: string;
}
