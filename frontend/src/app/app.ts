import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

type Section = 'students' | 'classes' | 'courses' | 'enrollments';
interface Student { id: number; studentCode: string; fullName: string; dateOfBirth: string; email: string; classId: number; }
interface SchoolClass { id: number; name: string; }
interface Course { id: number; code: string; name: string; credits: number; }
interface Enrollment { studentId: number; courseId: number; grade: number | null; }

@Component({
  imports: [CommonModule, FormsModule],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App implements OnInit {
  readonly apiUrl = 'http://localhost:5073/api';
  readonly sections: Section[] = ['students', 'classes', 'courses', 'enrollments'];
  readonly icons: Record<Section, string> = { students: '◉', classes: '▦', courses: '◇', enrollments: '↗' };
  activeSection: Section = 'students'; loading = false; error = '';
  editingId: number | null = null; editingEnrollment: { studentId: number; courseId: number } | null = null;
  students: Student[] = []; classes: SchoolClass[] = []; courses: Course[] = []; enrollments: Enrollment[] = [];
  studentForm: Omit<Student, 'id'> = { studentCode: '', fullName: '', dateOfBirth: '', email: '', classId: 0 };
  classForm = { name: '' }; courseForm = { code: '', name: '', credits: 3 };
  enrollmentForm: Enrollment = { studentId: 0, courseId: 0, grade: null };

  constructor(private readonly http: HttpClient) {}
  ngOnInit(): void { this.loadData(); }
  selectSection(section: Section): void { this.activeSection = section; this.cancelEdit(); }

  loadData(): void {
    this.loading = true; this.error = '';
    Promise.all([
      this.http.get<Student[]>(`${this.apiUrl}/students`).toPromise(), this.http.get<SchoolClass[]>(`${this.apiUrl}/classes`).toPromise(),
      this.http.get<Course[]>(`${this.apiUrl}/courses`).toPromise(), this.http.get<Enrollment[]>(`${this.apiUrl}/enrollments`).toPromise()
    ]).then(([students, classes, courses, enrollments]) => {
      this.students = students ?? []; this.classes = classes ?? []; this.courses = courses ?? []; this.enrollments = enrollments ?? [];
      this.cancelEdit();
    }).catch(() => this.error = 'Không thể kết nối StudentApi. Hãy kiểm tra API đang chạy ở cổng 5073.')
      .finally(() => this.loading = false);
  }

  save(): void {
    this.loading = true; const section = this.activeSection; const id = this.editingId; const enrollment = this.editingEnrollment;
    let request;
    if (section === 'students') request = id ? this.http.put(`${this.apiUrl}/students/${id}`, this.studentForm) : this.http.post(`${this.apiUrl}/students`, this.studentForm);
    else if (section === 'classes') request = id ? this.http.put(`${this.apiUrl}/classes/${id}`, this.classForm) : this.http.post(`${this.apiUrl}/classes`, this.classForm);
    else if (section === 'courses') request = id ? this.http.put(`${this.apiUrl}/courses/${id}`, this.courseForm) : this.http.post(`${this.apiUrl}/courses`, this.courseForm);
    else if (enrollment) request = this.http.put(`${this.apiUrl}/enrollments/${enrollment.studentId}/${enrollment.courseId}`, { grade: this.enrollmentForm.grade });
    else request = this.http.post(`${this.apiUrl}/enrollments`, this.enrollmentForm);
    request.subscribe({ next: () => { this.cancelEdit(); this.loadData(); }, error: (response) => { this.error = response.error || 'Không thể lưu dữ liệu.'; this.loading = false; } });
  }

  edit(item: Student | SchoolClass | Course | Enrollment): void {
    if (this.activeSection === 'students') this.studentForm = { ...(item as Student) };
    if (this.activeSection === 'classes') this.classForm = { name: (item as SchoolClass).name };
    if (this.activeSection === 'courses') this.courseForm = { code: (item as Course).code, name: (item as Course).name, credits: (item as Course).credits };
    if (this.activeSection === 'enrollments') { const value = item as Enrollment; this.enrollmentForm = { ...value }; this.editingEnrollment = { studentId: value.studentId, courseId: value.courseId }; }
    else this.editingId = (item as Student | SchoolClass | Course).id;
  }

  remove(item: Student | SchoolClass | Course | Enrollment): void {
    if (!confirm('Bạn có chắc muốn xóa bản ghi này?')) return;
    const endpoint = this.activeSection === 'enrollments' ? `${this.apiUrl}/enrollments/${(item as Enrollment).studentId}/${(item as Enrollment).courseId}` : `${this.apiUrl}/${this.activeSection}/${(item as Student | SchoolClass | Course).id}`;
    this.http.delete(endpoint).subscribe({ next: () => this.loadData(), error: () => this.error = 'Không thể xóa bản ghi.' });
  }

  cancelEdit(): void {
    this.editingId = null; this.editingEnrollment = null;
    this.studentForm = { studentCode: '', fullName: '', dateOfBirth: '', email: '', classId: this.classes[0]?.id ?? 0 };
    this.classForm = { name: '' }; this.courseForm = { code: '', name: '', credits: 3 };
    this.enrollmentForm = { studentId: this.students[0]?.id ?? 0, courseId: this.courses[0]?.id ?? 0, grade: null };
  }

  isEditing(): boolean { return this.editingId !== null || this.editingEnrollment !== null; }
  currentCount(): number { return this.activeSection === 'students' ? this.students.length : this.activeSection === 'classes' ? this.classes.length : this.activeSection === 'courses' ? this.courses.length : this.enrollments.length; }
  label(section: Section): string { return ({ students: 'Sinh viên', classes: 'Lớp học', courses: 'Môn học', enrollments: 'Đăng ký học' })[section]; }
  className(id: number): string { return this.classes.find(item => item.id === id)?.name ?? `Lớp #${id}`; }
  studentName(id: number): string { return this.students.find(item => item.id === id)?.fullName ?? `Sinh viên #${id}`; }
  courseName(id: number): string { return this.courses.find(item => item.id === id)?.name ?? `Môn #${id}`; }
  studentCount(id: number): number { return this.students.filter(student => student.classId === id).length; }
}
