import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';
import { ClassDto, CreateClassDto, UpdateClassDto } from '../models/class.model';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private readonly apiUrl = 'http://localhost:5000/api/classes';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<ClassDto[]>> {
    return this.http.get<ApiResponse<ClassDto[]>>(this.apiUrl);
  }

  getById(id: number): Observable<ApiResponse<ClassDto>> {
    return this.http.get<ApiResponse<ClassDto>>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateClassDto): Observable<ApiResponse<ClassDto>> {
    return this.http.post<ApiResponse<ClassDto>>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateClassDto): Observable<ApiResponse<ClassDto>> {
    return this.http.put<ApiResponse<ClassDto>>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
