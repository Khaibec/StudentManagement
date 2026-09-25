import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClassService } from '../../services/class.service';
import { AuthService } from '../../services/auth.service';
import { ClassDto } from '../../models/class.model';

@Component({
  selector: 'app-classes',
  templateUrl: './classes.component.html',
  styleUrls: ['./classes.component.css']
})
export class ClassesComponent implements OnInit {
  classes: ClassDto[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  showModal = false;
  isEditMode = false;
  selectedClassId: number | null = null;
  classForm: FormGroup;

  showDeleteModal = false;
  classToDelete: ClassDto | null = null;

  constructor(
    private classService: ClassService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    this.classForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', [Validators.maxLength(200)]]
    });
  }

  ngOnInit(): void {
    this.loadClasses();
  }

  loadClasses(): void {
    this.isLoading = true;
    this.classService.getAll().subscribe(
      (res) => {
        this.isLoading = false;
        if (res.success) {
          this.classes = res.data;
        }
      },
      () => {
        this.isLoading = false;
        this.errorMessage = 'Không thể tải danh sách lớp học.';
      }
    );
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedClassId = null;
    this.classForm.reset();
    this.showModal = true;
  }

  openEditModal(c: ClassDto): void {
    this.isEditMode = true;
    this.selectedClassId = c.id;
    this.classForm.patchValue({
      name: c.name,
      description: c.description
    });
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.classForm.reset();
  }

  onSubmit(): void {
    if (this.classForm.invalid) {
      this.classForm.markAllAsTouched();
      return;
    }

    const formValue = this.classForm.value;
    if (this.isEditMode && this.selectedClassId) {
      this.classService.update(this.selectedClassId, formValue).subscribe(
        () => {
          this.closeModal();
          this.showSuccess('Cập nhật lớp học thành công!');
          this.loadClasses();
        },
        (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi cập nhật lớp học.';
        }
      );
    } else {
      this.classService.create(formValue).subscribe(
        () => {
          this.closeModal();
          this.showSuccess('Thêm mới lớp học thành công!');
          this.loadClasses();
        },
        (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi tạo lớp học.';
        }
      );
    }
  }

  openDeleteModal(c: ClassDto): void {
    this.classToDelete = c;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.classToDelete = null;
  }

  confirmDelete(): void {
    if (!this.classToDelete) return;
    this.classService.delete(this.classToDelete.id).subscribe(
      () => {
        this.closeDeleteModal();
        this.showSuccess('Đã xóa lớp học thành công!');
        this.loadClasses();
      },
      (err) => {
        this.closeDeleteModal();
        this.errorMessage = err.error?.message || 'Không thể xóa lớp học này.';
      }
    );
  }

  private showSuccess(msg: string): void {
    this.successMessage = msg;
    setTimeout(() => {
      this.successMessage = '';
    }, 4000);
  }
}
