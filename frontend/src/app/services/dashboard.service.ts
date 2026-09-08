import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';
import { DashboardStatsDto } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = 'http://localhost:5000/api/dashboard/stats';

  constructor(private http: HttpClient) {}

  getStats(): Observable<ApiResponse<DashboardStatsDto>> {
    return this.http.get<ApiResponse<DashboardStatsDto>>(this.apiUrl);
  }
}
