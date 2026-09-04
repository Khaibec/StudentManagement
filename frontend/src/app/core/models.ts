export interface User { id: number; fullName: string; email: string; role: string; }
export interface AuthResponse { accessToken: string; expiresAtUtc: string; user: User; }
export interface Student { id: number; studentCode: string; fullName: string; dateOfBirth: string; email: string; classId: number; }
export interface SchoolClass { id: number; name: string; }
export interface Course { id: number; code: string; name: string; credits: number; }
export interface Enrollment { studentId: number; courseId: number; grade: number | null; }
