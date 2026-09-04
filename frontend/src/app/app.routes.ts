import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/auth-page').then(m => m.AuthPage), data: { mode: 'login' } },
  { path: 'register', loadComponent: () => import('./features/auth/auth-page').then(m => m.AuthPage), data: { mode: 'register' } },
  { path: 'dashboard', canActivate: [authGuard], loadComponent: () => import('./features/dashboard/dashboard-page').then(m => m.DashboardPage) },
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' }, { path: '**', redirectTo: 'dashboard' },
];
