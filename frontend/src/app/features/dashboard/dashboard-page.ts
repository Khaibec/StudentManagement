import { DatePipe } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/auth.service';
import { Course, Enrollment, SchoolClass, Student } from '../../core/models';

type Section = 'students' | 'classes' | 'courses' | 'enrollments';
const apiUrl = 'http://localhost:5073/api';

@Component({ imports: [ReactiveFormsModule, DatePipe], templateUrl: './dashboard-page.html', styleUrl: './dashboard-page.scss' })
export class DashboardPage {
  private readonly http = inject(HttpClient); private readonly fb = inject(FormBuilder); private readonly router = inject(Router); readonly auth = inject(AuthService);
  readonly sections: { id: Section; label: string }[] = [{ id: 'students', label: 'Sinh viên' }, { id: 'classes', label: 'Lớp học' }, { id: 'courses', label: 'Môn học' }, { id: 'enrollments', label: 'Đăng ký học' }];
  readonly active = signal<Section>('students'); readonly loading = signal(false); readonly error = signal(''); readonly editingId = signal<number | null>(null); readonly editingEnrollment = signal<Enrollment | null>(null);
  readonly students = signal<Student[]>([]); readonly classes = signal<SchoolClass[]>([]); readonly courses = signal<Course[]>([]); readonly enrollments = signal<Enrollment[]>([]);
  readonly count = computed(() => ({ students: this.students().length, classes: this.classes().length, courses: this.courses().length, enrollments: this.enrollments().length })[this.active()]);
  readonly form = this.fb.nonNullable.group({ studentCode: [''], fullName: [''], dateOfBirth: [''], email: [''], classId: [0], name: [''], code: [''], credits: [3], studentId: [0], courseId: [0], grade: [''] });
  constructor() { this.load(); }
  select(section: Section): void { this.active.set(section); this.reset(); }
  load(): void { this.loading.set(true); this.error.set(''); forkJoin({ students: this.http.get<Student[]>(`${apiUrl}/students`), classes: this.http.get<SchoolClass[]>(`${apiUrl}/classes`), courses: this.http.get<Course[]>(`${apiUrl}/courses`), enrollments: this.http.get<Enrollment[]>(`${apiUrl}/enrollments`) }).subscribe({ next: data => { this.students.set(data.students); this.classes.set(data.classes); this.courses.set(data.courses); this.enrollments.set(data.enrollments); this.reset(); this.loading.set(false); }, error: () => { this.error.set('Không thể tải dữ liệu. Hãy kiểm tra StudentApi đang chạy ở cổng 5073.'); this.loading.set(false); } }); }
  save(): void {
    const type = this.active(); const raw = this.form.getRawValue(); let request;
    if (type === 'students') { const payload = { studentCode: raw.studentCode, fullName: raw.fullName, dateOfBirth: raw.dateOfBirth, email: raw.email, classId: Number(raw.classId) }; request = this.editingId() ? this.http.put(`${apiUrl}/students/${this.editingId()}`, payload) : this.http.post(`${apiUrl}/students`, payload); }
    else if (type === 'classes') { request = this.editingId() ? this.http.put(`${apiUrl}/classes/${this.editingId()}`, { name: raw.name }) : this.http.post(`${apiUrl}/classes`, { name: raw.name }); }
    else if (type === 'courses') { const payload = { code: raw.code, name: raw.name, credits: Number(raw.credits) }; request = this.editingId() ? this.http.put(`${apiUrl}/courses/${this.editingId()}`, payload) : this.http.post(`${apiUrl}/courses`, payload); }
    else { const grade = raw.grade === '' ? null : Number(raw.grade); const editing = this.editingEnrollment(); request = editing ? this.http.put(`${apiUrl}/enrollments/${editing.studentId}/${editing.courseId}`, { grade }) : this.http.post(`${apiUrl}/enrollments`, { studentId: Number(raw.studentId), courseId: Number(raw.courseId), grade }); }
    this.loading.set(true); request.subscribe({ next: () => this.load(), error: (error: HttpErrorResponse) => { this.error.set(error.error?.message ?? 'Không thể lưu dữ liệu.'); this.loading.set(false); } });
  }
  edit(item: Student | SchoolClass | Course | Enrollment): void { const type = this.active(); this.editingId.set(null); this.editingEnrollment.set(null); if (type === 'students') { const x = item as Student; this.editingId.set(x.id); this.form.patchValue(x); } else if (type === 'classes') { const x = item as SchoolClass; this.editingId.set(x.id); this.form.patchValue(x); } else if (type === 'courses') { const x = item as Course; this.editingId.set(x.id); this.form.patchValue(x); } else { const x = item as Enrollment; this.editingEnrollment.set(x); this.form.patchValue({ ...x, grade: x.grade?.toString() ?? '' }); } }
  remove(item: Student | SchoolClass | Course | Enrollment): void { if (!confirm('Bạn có chắc muốn xóa bản ghi này?')) return; const type = this.active(); const endpoint = type === 'enrollments' ? `${apiUrl}/enrollments/${(item as Enrollment).studentId}/${(item as Enrollment).courseId}` : `${apiUrl}/${type}/${(item as Student | SchoolClass | Course).id}`; this.http.delete(endpoint).subscribe({ next: () => this.load(), error: () => this.error.set('Không thể xóa bản ghi. Có thể bản ghi đang được sử dụng.') }); }
  reset(): void { this.editingId.set(null); this.editingEnrollment.set(null); this.form.reset({ studentCode: '', fullName: '', dateOfBirth: '', email: '', classId: this.classes()[0]?.id ?? 0, name: '', code: '', credits: 3, studentId: this.students()[0]?.id ?? 0, courseId: this.courses()[0]?.id ?? 0, grade: '' }); }
  label(): string { return this.sections.find(x => x.id === this.active())?.label ?? ''; }
  className(id: number): string { return this.classes().find(x => x.id === id)?.name ?? `Lớp #${id}`; }
  studentName(id: number): string { return this.students().find(x => x.id === id)?.fullName ?? `Sinh viên #${id}`; }
  courseName(id: number): string { return this.courses().find(x => x.id === id)?.name ?? `Môn #${id}`; }
  logout(): void { this.auth.logout(); void this.router.navigateByUrl('/login'); }
}
