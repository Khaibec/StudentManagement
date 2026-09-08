import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../services/dashboard.service';
import { AuthService } from '../../services/auth.service';
import { DashboardStatsDto } from '../../models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  stats: DashboardStatsDto | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private dashboardService: DashboardService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.isLoading = true;
    this.dashboardService.getStats().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) {
          this.stats = res.data;
        }
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Không thể tải dữ liệu thống kê bảng điều khiển.';
      }
    });
  }
}
