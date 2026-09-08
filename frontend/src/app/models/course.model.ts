export interface CourseDto {
  id: number;
  code: string;
  title: string;
  credits: number;
  description?: string;
  enrolledCount: number;
}

export interface CreateCourseDto {
  code: string;
  title: string;
  credits: number;
  description?: string;
}

export interface UpdateCourseDto {
  code: string;
  title: string;
  credits: number;
  description?: string;
}
