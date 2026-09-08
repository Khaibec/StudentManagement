import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';
import { PagedResult } from '../models/paged-result.model';
import {
  CreateStudentDto,
  StudentDetailDto,
  StudentDto,
  StudentQueryParameters,
  UpdateStudentDto
} from '../models/student.model';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private readonly apiUrl = 'http://localhost:5000/api/students';

  constructor(private http: HttpClient) {}

  getAll(query: StudentQueryParameters): Observable<ApiResponse<PagedResult<StudentDto>>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber.toString())
      .set('pageSize', query.pageSize.toString());

    if (query.searchTerm && query.searchTerm.trim()) {
      params = params.set('searchTerm', query.searchTerm.trim());
    }
    if (query.classRoomId && query.classRoomId > 0) {
      params = params.set('classRoomId', query.classRoomId.toString());
    }
    if (query.gender && query.gender.trim()) {
      params = params.set('gender', query.gender.trim());
    }
    if (query.sortBy && query.sortBy.trim()) {
      params = params.set('sortBy', query.sortBy.trim());
    }
    if (query.sortOrder && query.sortOrder.trim()) {
      params = params.set('sortOrder', query.sortOrder.trim());
    }

    return this.http.get<ApiResponse<PagedResult<StudentDto>>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<ApiResponse<StudentDetailDto>> {
    return this.http.get<ApiResponse<StudentDetailDto>>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateStudentDto): Observable<ApiResponse<StudentDto>> {
    return this.http.post<ApiResponse<StudentDto>>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateStudentDto): Observable<ApiResponse<StudentDto>> {
    return this.http.put<ApiResponse<StudentDto>>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
