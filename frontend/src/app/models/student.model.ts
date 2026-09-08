export interface StudentDto {
  id: number;
  studentCode: string;
  fullName: string;
  dateOfBirth: string;
  gender: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  classRoomId?: number;
  classRoomName?: string;
  enrolledCoursesCount: number;
}

export interface StudentCourseDto {
  enrollmentId: number;
  courseId: number;
  courseCode: string;
  courseTitle: string;
  credits: number;
  enrollmentDate: string;
  grade?: number;
}

export interface StudentDetailDto extends StudentDto {
  enrolledCourses: StudentCourseDto[];
}

export interface CreateStudentDto {
  studentCode: string;
  fullName: string;
  dateOfBirth: string;
  gender: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  classRoomId?: number;
}

export interface UpdateStudentDto {
  fullName: string;
  dateOfBirth: string;
  gender: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  classRoomId?: number;
}

export interface StudentQueryParameters {
  searchTerm?: string;
  classRoomId?: number;
  gender?: string;
  sortBy?: string;
  sortOrder?: string;
  pageNumber: number;
  pageSize: number;
}
