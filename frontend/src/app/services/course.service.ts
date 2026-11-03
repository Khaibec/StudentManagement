import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';
import { CourseDto, CreateCourseDto, UpdateCourseDto } from '../models/course.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private readonly apiUrl = `${environment.apiUrl}/courses`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<CourseDto[]>> {
    return this.http.get<ApiResponse<CourseDto[]>>(this.apiUrl);
  }

  getById(id: number): Observable<ApiResponse<CourseDto>> {
    return this.http.get<ApiResponse<CourseDto>>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateCourseDto): Observable<ApiResponse<CourseDto>> {
    return this.http.post<ApiResponse<CourseDto>>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateCourseDto): Observable<ApiResponse<CourseDto>> {
    return this.http.put<ApiResponse<CourseDto>>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
