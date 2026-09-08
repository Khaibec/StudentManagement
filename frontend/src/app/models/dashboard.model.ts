import { EnrollmentDto } from './enrollment.model';
import { StudentDto } from './student.model';

export interface DashboardStatsDto {
  totalStudents: number;
  totalClasses: number;
  totalCourses: number;
  totalEnrollments: number;
  recentStudents: StudentDto[];
  recentEnrollments: EnrollmentDto[];
}
