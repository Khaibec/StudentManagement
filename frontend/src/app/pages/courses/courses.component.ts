import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CourseService } from '../../services/course.service';
import { AuthService } from '../../services/auth.service';
import { CourseDto } from '../../models/course.model';

@Component({
  selector: 'app-courses',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './courses.component.html',
  styleUrls: ['./courses.component.css']
})
export class CoursesComponent implements OnInit {
  courses: CourseDto[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  showModal = false;
  isEditMode = false;
  selectedCourseId: number | null = null;
  courseForm: FormGroup;

  showDeleteModal = false;
  courseToDelete: CourseDto | null = null;

  constructor(
    private courseService: CourseService,
    public authService: AuthService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.courseForm = this.fb.group({
      code: ['', [Validators.required, Validators.maxLength(20)]],
      title: ['', [Validators.required, Validators.maxLength(150)]],
      credits: [3, [Validators.required, Validators.min(1), Validators.max(10)]],
      description: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.isLoading = true;
    this.courseService.getAll().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) {
          this.courses = res.data;
        }
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Không thể tải danh sách môn học.';
        this.cdr.detectChanges();
      }
    });
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedCourseId = null;
    this.courseForm.reset({ credits: 3 });
    this.showModal = true;
  }

  openEditModal(c: CourseDto): void {
    this.isEditMode = true;
    this.selectedCourseId = c.id;
    this.courseForm.patchValue({
      code: c.code,
      title: c.title,
      credits: c.credits,
      description: c.description
    });
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.courseForm.reset({ credits: 3 });
  }

  onSubmit(): void {
    if (this.courseForm.invalid) {
      this.courseForm.markAllAsTouched();
      return;
    }

    const val = this.courseForm.value;
    if (this.isEditMode && this.selectedCourseId) {
      this.courseService.update(this.selectedCourseId, val).subscribe({
        next: () => {
          this.closeModal();
          this.showSuccess('Cập nhật môn học thành công!');
          this.loadCourses();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi cập nhật môn học.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.courseService.create(val).subscribe({
        next: () => {
          this.closeModal();
          this.showSuccess('Thêm mới môn học thành công!');
          this.loadCourses();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi tạo môn học.';
          this.cdr.detectChanges();
        }
      });
    }
  }

  openDeleteModal(c: CourseDto): void {
    this.courseToDelete = c;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.courseToDelete = null;
  }

  confirmDelete(): void {
    if (!this.courseToDelete) return;
    this.courseService.delete(this.courseToDelete.id).subscribe({
      next: () => {
        this.closeDeleteModal();
        this.showSuccess('Đã xóa môn học thành công!');
        this.loadCourses();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.closeDeleteModal();
        this.errorMessage = err.error?.message || 'Không thể xóa môn học này.';
        this.cdr.detectChanges();
      }
    });
  }

  private showSuccess(msg: string): void {
    this.successMessage = msg;
    setTimeout(() => {
      this.successMessage = '';
      this.cdr.detectChanges();
    }, 4000);
  }
}
