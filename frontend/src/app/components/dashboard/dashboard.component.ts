import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { AuthService } from '../../services/auth.service';
import { DashboardStatsDto } from '../../models/dashboard.model';
import { AuthResponse } from '../../models/auth.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  stats: DashboardStatsDto | null = null;
  currentUser: AuthResponse | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private dashboardService: DashboardService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;
    this.loadStats();
  }

  loadStats(): void {
    this.isLoading = true;
    this.dashboardService.getStats().subscribe(
      (res) => {
        this.isLoading = false;
        if (res.success) {
          this.stats = res.data;
        }
      },
      () => {
        this.isLoading = false;
        this.errorMessage = 'Không thể nạp dữ liệu thống kê.';
      }
    );
  }
}
