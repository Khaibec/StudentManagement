export interface EnrollmentDto {
  id: number;
  studentId: number;
  studentCode: string;
  studentName: string;
  courseId: number;
  courseCode: string;
  courseTitle: string;
  credits: number;
  enrollmentDate: string;
  grade?: number;
}

export interface CreateEnrollmentDto {
  studentId: number;
  courseId: number;
}

export interface UpdateGradeDto {
  grade?: number;
}
