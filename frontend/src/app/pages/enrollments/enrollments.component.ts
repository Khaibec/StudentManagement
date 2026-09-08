import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { EnrollmentService } from '../../services/enrollment.service';
import { StudentService } from '../../services/student.service';
import { CourseService } from '../../services/course.service';
import { AuthService } from '../../services/auth.service';
import { EnrollmentDto } from '../../models/enrollment.model';
import { StudentDto } from '../../models/student.model';
import { CourseDto } from '../../models/course.model';

@Component({
  selector: 'app-enrollments',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './enrollments.component.html',
  styleUrls: ['./enrollments.component.css']
})
export class EnrollmentsComponent implements OnInit {
  enrollments: EnrollmentDto[] = [];
  students: StudentDto[] = [];
  courses: CourseDto[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Filters
  filterStudentId: number | undefined = undefined;
  filterCourseId: number | undefined = undefined;

  // Register Modal state
  showEnrollModal = false;
  enrollForm: FormGroup;

  // Update Grade Modal state
  showGradeModal = false;
  selectedEnrollment: EnrollmentDto | null = null;
  gradeForm: FormGroup;

  // Cancel Modal state
  showCancelModal = false;
  enrollmentToCancel: EnrollmentDto | null = null;

  constructor(
    private enrollmentService: EnrollmentService,
    private studentService: StudentService,
    private courseService: CourseService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    this.enrollForm = this.fb.group({
      studentId: [null, [Validators.required]],
      courseId: [null, [Validators.required]]
    });

    this.gradeForm = this.fb.group({
      grade: [null, [Validators.min(0), Validators.max(10)]]
    });
  }

  ngOnInit(): void {
    this.loadDropdownData();
    this.loadEnrollments();
  }

  loadDropdownData(): void {
    this.studentService.getAll({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (res) => {
        if (res.success) this.students = res.data.items;
      }
    });

    this.courseService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.courses = res.data;
      }
    });
  }

  loadEnrollments(): void {
    this.isLoading = true;
    this.enrollmentService.getAll(this.filterStudentId, this.filterCourseId).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) {
          this.enrollments = res.data;
        }
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Không thể tải danh sách đăng ký.';
      }
    });
  }

  onFilterChange(): void {
    this.loadEnrollments();
  }

  openEnrollModal(): void {
    this.enrollForm.reset({ studentId: null, courseId: null });
    this.showEnrollModal = true;
  }

  closeEnrollModal(): void {
    this.showEnrollModal = false;
    this.enrollForm.reset();
  }

  onSubmitEnroll(): void {
    if (this.enrollForm.invalid) {
      this.enrollForm.markAllAsTouched();
      return;
    }

    this.enrollmentService.enroll(this.enrollForm.value).subscribe({
      next: () => {
        this.closeEnrollModal();
        this.showSuccess('Đăng ký môn học thành công!');
        this.loadEnrollments();
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Không thể đăng ký môn học.';
      }
    });
  }

  openGradeModal(e: EnrollmentDto): void {
    this.selectedEnrollment = e;
    this.gradeForm.patchValue({ grade: e.grade });
    this.showGradeModal = true;
  }

  closeGradeModal(): void {
    this.showGradeModal = false;
    this.selectedEnrollment = null;
    this.gradeForm.reset();
  }

  onSubmitGrade(): void {
    if (this.gradeForm.invalid || !this.selectedEnrollment) {
      this.gradeForm.markAllAsTouched();
      return;
    }

    this.enrollmentService.updateGrade(this.selectedEnrollment.id, this.gradeForm.value).subscribe({
      next: () => {
        this.closeGradeModal();
        this.showSuccess('Cập nhật điểm thành công!');
        this.loadEnrollments();
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Lỗi khi cập nhật điểm.';
      }
    });
  }

  openCancelModal(e: EnrollmentDto): void {
    this.enrollmentToCancel = e;
    this.showCancelModal = true;
  }

  closeCancelModal(): void {
    this.showCancelModal = false;
    this.enrollmentToCancel = null;
  }

  confirmCancel(): void {
    if (!this.enrollmentToCancel) return;
    this.enrollmentService.cancel(this.enrollmentToCancel.id).subscribe({
      next: () => {
        this.closeCancelModal();
        this.showSuccess('Đã hủy đăng ký môn học thành công!');
        this.loadEnrollments();
      },
      error: (err) => {
        this.closeCancelModal();
        this.errorMessage = err.error?.message || 'Không thể hủy đăng ký môn học này.';
      }
    });
  }

  private showSuccess(msg: string): void {
    this.successMessage = msg;
    setTimeout(() => {
      this.successMessage = '';
    }, 4000);
  }
}
