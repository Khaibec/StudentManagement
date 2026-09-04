import { Component, computed, inject, signal } from '@angular/core';
import { ReactiveFormsModule, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './auth-page.html',
  styleUrl: './auth-page.scss',
})
export class AuthPage {
  private readonly fb = inject(FormBuilder); private readonly auth = inject(AuthService); private readonly router = inject(Router); private readonly route = inject(ActivatedRoute);
  readonly isRegister = computed(() => this.route.snapshot.data['mode'] === 'register');
  readonly loading = signal(false); readonly error = signal('');
  readonly form = this.fb.nonNullable.group({ fullName: ['', [Validators.required, Validators.minLength(2)]], email: ['', [Validators.required, Validators.email]], password: ['', [Validators.required, Validators.minLength(8)]] });
  submit(): void {
    this.form.markAllAsTouched(); if (this.form.invalid || this.loading()) return;
    this.loading.set(true); this.error.set(''); const value = this.form.getRawValue();
    const request = this.isRegister() ? this.auth.register(value) : this.auth.login({ email: value.email, password: value.password });
    request.subscribe({ next: () => void this.router.navigateByUrl('/dashboard'), error: response => { this.error.set(response.error?.message ?? 'Không thể xác thực. Vui lòng thử lại.'); this.loading.set(false); } });
  }
}
