import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClassService } from '../../services/class.service';
import { AuthService } from '../../services/auth.service';
import { ClassDto } from '../../models/class.model';

@Component({
  selector: 'app-classes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './classes.component.html',
  styleUrls: ['./classes.component.css']
})
export class ClassesComponent implements OnInit {
  classes: ClassDto[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Modal / Form state
  showModal = false;
  isEditMode = false;
  selectedClassId: number | null = null;
  classForm: FormGroup;

  // Delete modal state
  showDeleteModal = false;
  classToDelete: ClassDto | null = null;

  constructor(
    private classService: ClassService,
    public authService: AuthService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
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
    this.classService.getAll().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) {
          this.classes = res.data;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Không thể tải danh sách lớp học.';
        this.cdr.detectChanges();
      }
    });
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
      this.classService.update(this.selectedClassId, formValue).subscribe({
        next: (res) => {
          this.closeModal();
          this.showSuccess('Cập nhật lớp học thành công!');
          this.loadClasses();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi cập nhật lớp học.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.classService.create(formValue).subscribe({
        next: (res) => {
          this.closeModal();
          this.showSuccess('Thêm mới lớp học thành công!');
          this.loadClasses();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Lỗi khi tạo lớp học.';
          this.cdr.detectChanges();
        }
      });
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
    this.classService.delete(this.classToDelete.id).subscribe({
      next: () => {
        this.closeDeleteModal();
        this.showSuccess('Đã xóa lớp học thành công!');
        this.loadClasses();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.closeDeleteModal();
        this.errorMessage = err.error?.message || 'Không thể xóa lớp học này.';
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
