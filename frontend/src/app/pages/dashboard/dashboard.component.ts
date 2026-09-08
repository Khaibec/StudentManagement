import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container py-4">
      <div class="row">
        <div class="col-12">
          <div class="p-4 mb-4 bg-white rounded-3 shadow-sm border-start border-4 border-primary">
            <h2 class="fw-bold mb-2">Xin chào, {{ authService.currentUser()?.fullName }}! 👋</h2>
            <p class="text-muted mb-0">
              Bạn đang đăng nhập với vai trò:
              <span class="badge" [ngClass]="authService.isAdmin() ? 'bg-danger' : 'bg-success'">
                {{ authService.currentUser()?.role }}
              </span>
            </p>
          </div>
        </div>
      </div>
      <div class="row g-3">
        <div class="col-md-6 col-lg-3">
          <div class="card p-3 shadow-sm border-0 bg-white">
            <div class="d-flex align-items-center gap-3">
              <div class="p-3 bg-primary bg-opacity-10 text-primary rounded-circle">
                <i class="bi bi-people fs-4"></i>
              </div>
              <div>
                <div class="text-muted small">Quản lý</div>
                <h5 class="fw-bold mb-0">Học sinh</h5>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-6 col-lg-3">
          <div class="card p-3 shadow-sm border-0 bg-white">
            <div class="d-flex align-items-center gap-3">
              <div class="p-3 bg-success bg-opacity-10 text-success rounded-circle">
                <i class="bi bi-building fs-4"></i>
              </div>
              <div>
                <div class="text-muted small">Quản lý</div>
                <h5 class="fw-bold mb-0">Lớp học</h5>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-6 col-lg-3">
          <div class="card p-3 shadow-sm border-0 bg-white">
            <div class="d-flex align-items-center gap-3">
              <div class="p-3 bg-warning bg-opacity-10 text-warning rounded-circle">
                <i class="bi bi-journal-bookmark fs-4"></i>
              </div>
              <div>
                <div class="text-muted small">Quản lý</div>
                <h5 class="fw-bold mb-0">Môn học</h5>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-6 col-lg-3">
          <div class="card p-3 shadow-sm border-0 bg-white">
            <div class="d-flex align-items-center gap-3">
              <div class="p-3 bg-info bg-opacity-10 text-info rounded-circle">
                <i class="bi bi-pencil-square fs-4"></i>
              </div>
              <div>
                <div class="text-muted small">Nghiệp vụ</div>
                <h5 class="fw-bold mb-0">Đăng ký môn</h5>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent {
  constructor(public authService: AuthService) {}
}
