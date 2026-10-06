import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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

  query: StudentQueryParameters = {
    searchTerm: '',
    classRoomId: undefined,
    gender: '',
    sortBy: 'FullName',
    sortOrder: 'asc',
    pageNumber: 1,
    pageSize: 5
  };

  showModal = false;
  isEditMode = false;
  selectedStudentId: number | null = null;
  studentForm: FormGroup;

  showDetailModal = false;
  selectedStudentDetail: StudentDetailDto | null = null;
  isLoadingDetail = false;

  showDeleteModal = false;
  studentToDelete: StudentDto | null = null;

  // Tính năng tra cứu học sinh theo ID
  showFindByIdModal = false;
  searchStudentId: number | null = null;
  foundStudent: StudentDetailDto | null = null;
  isSearchingById = false;
  findByIdError = '';

  constructor(
    private studentService: StudentService,
    private classService: ClassService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    // Khởi tạo FormGroup bằng FormBuilder (Mô hình Reactive Forms chuẩn của Angular)
    // Các Validators giúp kiểm tra dữ liệu hợp lệ ngay trên trình duyệt trước khi gửi về API
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
    this.classService.getAll().subscribe((res) => {
      if (res.success) this.classes = res.data;
    });
  }

  loadStudents(): void {
    this.isLoading = true;
    this.studentService.getAll(this.query).subscribe(
      (res) => {
        this.isLoading = false;
        if (res.success) {
          this.pagedResult = res.data;
          this.students = res.data.items;
        }
      },
      () => {
        this.isLoading = false;
        this.errorMessage = 'Không thể tải danh sách học sinh.';
      }
    );
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
    this.studentForm.reset({ gender: 'Nam', classRoomId: null });
    // Khi thêm mới thì cho phép nhập Mã học sinh
    this.studentForm.get('studentCode')?.enable();
    this.showModal = true;
  }

  openEditModal(s: StudentDto): void {
    this.isEditMode = true;
    this.selectedStudentId = s.id;
    const dob = s.dateOfBirth ? s.dateOfBirth.split('T')[0] : '';

    // patchValue() tự động đổ dữ liệu của học sinh vào các trường tương ứng trong form
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
    // Khóa trường Mã học sinh khi cập nhật (vì Mã học sinh là định danh không được sửa)
    this.studentForm.get('studentCode')?.disable();
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.studentForm.reset();
  }

  onSubmit(): void {
    // Nếu dữ liệu form chưa thỏa mãn các Validators (ví dụ để trống tên, sai định dạng email...)
    if (this.studentForm.invalid) {
      // Đánh dấu tất cả ô nhập là "touched" để kích hoạt hiển thị thông báo lỗi màu đỏ trên giao diện
      this.studentForm.markAllAsTouched();
      return;
    }

    // LƯU Ý QUAN TRỌNG: Dùng getRawValue() thay vì .value
    // Vì .value sẽ tự động BỎ QUA các ô nhập đang bị disable (ở đây là studentCode khi sửa),
    // trong khi getRawValue() sẽ lấy đầy đủ toàn bộ giá trị của form kể cả ô bị disable.
    const val = this.studentForm.getRawValue();
    if (this.isEditMode && this.selectedStudentId) {
      this.studentService.update(this.selectedStudentId, val).subscribe(
        () => {
          this.closeModal();
          this.showSuccess('Cập nhật học sinh thành công!');
          this.loadStudents();
        },
        (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi cập nhật học sinh.';
        }
      );
    } else {
      this.studentService.create(val).subscribe(
        () => {
          this.closeModal();
          this.showSuccess('Thêm mới học sinh thành công!');
          this.loadStudents();
        },
        (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi tạo học sinh.';
        }
      );
    }
  }

  viewDetails(s: StudentDto): void {
    this.isLoadingDetail = true;
    this.showDetailModal = true;
    this.studentService.getById(s.id).subscribe(
      (res) => {
        this.isLoadingDetail = false;
        if (res.success) {
          this.selectedStudentDetail = res.data;
        }
      },
      () => {
        this.isLoadingDetail = false;
        this.errorMessage = 'Không thể tải chi tiết học sinh.';
      }
    );
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
    this.studentService.delete(this.studentToDelete.id).subscribe(
      () => {
        this.closeDeleteModal();
        this.showSuccess('Đã xóa học sinh thành công!');
        this.loadStudents();
      },
      (err) => {
        this.closeDeleteModal();
        this.errorMessage = err.error?.message || 'Không thể xóa học sinh này.';
      }
    );
  }

  // Các phương thức hỗ trợ Tra cứu học sinh theo ID
  openFindByIdModal(): void {
    this.showFindByIdModal = true;
    this.searchStudentId = null;
    this.foundStudent = null;
    this.findByIdError = '';
  }

  closeFindByIdModal(): void {
    this.showFindByIdModal = false;
    this.searchStudentId = null;
    this.foundStudent = null;
    this.findByIdError = '';
  }

  findStudentById(): void {
    if (!this.searchStudentId || this.searchStudentId <= 0) {
      this.findByIdError = 'Vui lòng nhập ID học sinh hợp lệ (số nguyên dương).';
      return;
    }

    this.isSearchingById = true;
    this.findByIdError = '';
    this.foundStudent = null;

    this.studentService.getById(this.searchStudentId).subscribe(
      (res) => {
        this.isSearchingById = false;
        if (res.success && res.data) {
          this.foundStudent = res.data;
        } else {
          this.findByIdError = res.message || 'Không tìm thấy học sinh.';
        }
      },
      (err) => {
        this.isSearchingById = false;
        this.findByIdError = err.error?.message || `Không tìm thấy học sinh với ID = ${this.searchStudentId}.`;
      }
    );
  }

  viewDetailsFromFound(): void {
    if (this.foundStudent) {
      this.selectedStudentDetail = this.foundStudent;
      this.showDetailModal = true;
      this.closeFindByIdModal();
    }
  }

  private showSuccess(msg: string): void {
    this.successMessage = msg;
    setTimeout(() => {
      this.successMessage = '';
    }, 4000);
  }
}
