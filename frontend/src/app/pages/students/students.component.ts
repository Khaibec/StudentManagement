import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { StudentService } from '../../services/student.service';
import { ClassService } from '../../services/class.service';
import { AuthService } from '../../services/auth.service';
import {
  StudentDetailDto,
  StudentDto,
  StudentQueryParameters
} from '../../models/student.model';
import { ClassDto } from '../../models/class.model';
import { PagedResult } from '../../models/paged-result.model';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './students.component.html',
  styleUrls: ['./students.component.css']
})
export class StudentsComponent implements OnInit {
  students: StudentDto[] = [];
  classes: ClassDto[] = [];
  pagedResult: PagedResult<StudentDto> | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Filter, Sort & Pagination parameters
  query: StudentQueryParameters = {
    searchTerm: '',
    classRoomId: undefined,
    gender: '',
    sortBy: 'FullName',
    sortOrder: 'asc',
    pageNumber: 1,
    pageSize: 5
  };

  // Add/Edit Modal state
  showModal = false;
  isEditMode = false;
  selectedStudentId: number | null = null;
  studentForm: FormGroup;

  // View Details Modal state
  showDetailModal = false;
  selectedStudentDetail: StudentDetailDto | null = null;
  isLoadingDetail = false;

  // Delete Modal state
  showDeleteModal = false;
  studentToDelete: StudentDto | null = null;

  constructor(
    private studentService: StudentService,
    private classService: ClassService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    this.studentForm = this.fb.group({
      studentCode: ['', [Validators.required, Validators.maxLength(20)]],
      fullName: ['', [Validators.required, Validators.maxLength(100)]],
      dateOfBirth: ['', [Validators.required]],
      gender: ['Nam', [Validators.required]],
      email: ['', [Validators.email, Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      address: ['', [Validators.maxLength(250)]],
      classRoomId: [null]
    });
  }

  ngOnInit(): void {
    this.loadClasses();
    this.loadStudents();
  }

  loadClasses(): void {
    this.classService.getAll().subscribe({
      next: (res) => {
        if (res.success) {
          this.classes = res.data;
        }
      }
    });
  }

  loadStudents(): void {
    this.isLoading = true;
    this.studentService.getAll(this.query).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) {
          this.pagedResult = res.data;
          this.students = res.data.items;
        }
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Không thể tải danh sách học sinh.';
      }
    });
  }

  onSearch(): void {
    this.query.pageNumber = 1;
    this.loadStudents();
  }

  onFilterChange(): void {
    this.query.pageNumber = 1;
    this.loadStudents();
  }

  onSortChange(column: string): void {
    if (this.query.sortBy === column) {
      this.query.sortOrder = this.query.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.query.sortBy = column;
      this.query.sortOrder = 'asc';
    }
    this.loadStudents();
  }

  changePage(page: number): void {
    if (this.pagedResult && page >= 1 && page <= this.pagedResult.totalPages) {
      this.query.pageNumber = page;
      this.loadStudents();
    }
  }

  changePageSize(size: any): void {
    this.query.pageSize = Number(size);
    this.query.pageNumber = 1;
    this.loadStudents();
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedStudentId = null;
    this.studentForm.reset({
      gender: 'Nam',
      classRoomId: null
    });
    this.studentForm.get('studentCode')?.enable();
    this.showModal = true;
  }

  openEditModal(s: StudentDto): void {
    this.isEditMode = true;
    this.selectedStudentId = s.id;
    // Format date string to YYYY-MM-DD for HTML5 date input
    const dob = s.dateOfBirth ? s.dateOfBirth.split('T')[0] : '';

    this.studentForm.patchValue({
      studentCode: s.studentCode,
      fullName: s.fullName,
      dateOfBirth: dob,
      gender: s.gender,
      email: s.email,
      phoneNumber: s.phoneNumber,
      address: s.address,
      classRoomId: s.classRoomId
    });
    this.studentForm.get('studentCode')?.disable(); // Không cho đổi mã học sinh khi sửa
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.studentForm.reset();
  }

  onSubmit(): void {
    if (this.studentForm.invalid) {
      this.studentForm.markAllAsTouched();
      return;
    }

    const val = this.studentForm.getRawValue();
    if (this.isEditMode && this.selectedStudentId) {
      this.studentService.update(this.selectedStudentId, val).subscribe({
        next: () => {
          this.closeModal();
          this.showSuccess('Cập nhật học sinh thành công!');
          this.loadStudents();
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi cập nhật học sinh.';
        }
      });
    } else {
      this.studentService.create(val).subscribe({
        next: () => {
          this.closeModal();
          this.showSuccess('Thêm mới học sinh thành công!');
          this.loadStudents();
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi tạo học sinh.';
        }
      });
    }
  }

  // Xem chi tiết học sinh & các môn đã học
  viewDetails(s: StudentDto): void {
    this.isLoadingDetail = true;
    this.showDetailModal = true;
    this.studentService.getById(s.id).subscribe({
      next: (res) => {
        this.isLoadingDetail = false;
        if (res.success) {
          this.selectedStudentDetail = res.data;
        }
      },
      error: () => {
        this.isLoadingDetail = false;
        this.errorMessage = 'Không thể tải chi tiết học sinh.';
      }
    });
  }

  closeDetailModal(): void {
    this.showDetailModal = false;
    this.selectedStudentDetail = null;
  }

  openDeleteModal(s: StudentDto): void {
    this.studentToDelete = s;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.studentToDelete = null;
  }

  confirmDelete(): void {
    if (!this.studentToDelete) return;
    this.studentService.delete(this.studentToDelete.id).subscribe({
      next: () => {
        this.closeDeleteModal();
        this.showSuccess('Đã xóa học sinh thành công!');
        this.loadStudents();
      },
      error: (err) => {
        this.closeDeleteModal();
        this.errorMessage = err.error?.message || 'Không thể xóa học sinh này.';
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
