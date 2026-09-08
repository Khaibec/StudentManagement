import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';
import { CreateEnrollmentDto, EnrollmentDto, UpdateGradeDto } from '../models/enrollment.model';

@Injectable({
  providedIn: 'root'
})
export class EnrollmentService {
  private readonly apiUrl = 'http://localhost:5000/api/enrollments';

  constructor(private http: HttpClient) {}

  getAll(studentId?: number, courseId?: number): Observable<ApiResponse<EnrollmentDto[]>> {
    let params = new HttpParams();
    if (studentId && studentId > 0) {
      params = params.set('studentId', studentId.toString());
    }
    if (courseId && courseId > 0) {
      params = params.set('courseId', courseId.toString());
    }
    return this.http.get<ApiResponse<EnrollmentDto[]>>(this.apiUrl, { params });
  }

  enroll(dto: CreateEnrollmentDto): Observable<ApiResponse<EnrollmentDto>> {
    return this.http.post<ApiResponse<EnrollmentDto>>(this.apiUrl, dto);
  }

  updateGrade(id: number, dto: UpdateGradeDto): Observable<ApiResponse<EnrollmentDto>> {
    return this.http.put<ApiResponse<EnrollmentDto>>(`${this.apiUrl}/${id}/grade`, dto);
  }

  cancel(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
