import { HttpClient } from '@angular/common/http';
import { Service, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthResponse, User } from './models';

const apiUrl = 'http://localhost:5073/api';
const storageKey = 'student-hub-session';

@Service()
export class AuthService {
  private readonly http = inject(HttpClient);
  readonly user = signal<User | null>(this.readSession()?.user ?? null);
  register(payload: { fullName: string; email: string; password: string }): Observable<AuthResponse> { return this.http.post<AuthResponse>(`${apiUrl}/auth/register`, payload).pipe(tap(session => this.save(session))); }
  login(payload: { email: string; password: string }): Observable<AuthResponse> { return this.http.post<AuthResponse>(`${apiUrl}/auth/login`, payload).pipe(tap(session => this.save(session))); }
  logout(): void { localStorage.removeItem(storageKey); this.user.set(null); }
  token(): string | null { const session = this.readSession(); if (!session || new Date(session.expiresAtUtc).getTime() <= Date.now()) { this.logout(); return null; } return session.accessToken; }
  private save(session: AuthResponse): void { localStorage.setItem(storageKey, JSON.stringify(session)); this.user.set(session.user); }
  private readSession(): AuthResponse | null { try { return JSON.parse(localStorage.getItem(storageKey) ?? 'null') as AuthResponse | null; } catch { localStorage.removeItem(storageKey); return null; } }
}
