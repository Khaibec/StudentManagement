import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { StudentsComponent } from './pages/students/students.component';
import { ClassesComponent } from './pages/classes/classes.component';
import { CoursesComponent } from './pages/courses/courses.component';
import { EnrollmentsComponent } from './pages/enrollments/enrollments.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'students', component: StudentsComponent, canActivate: [authGuard] },
  { path: 'classes', component: ClassesComponent, canActivate: [authGuard] },
  { path: 'courses', component: CoursesComponent, canActivate: [authGuard] },
  { path: 'enrollments', component: EnrollmentsComponent, canActivate: [authGuard] },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: 'dashboard' }
];
