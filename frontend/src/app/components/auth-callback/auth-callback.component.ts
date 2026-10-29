import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth-callback',
  templateUrl: './auth-callback.component.html',
  styles: [`
    .callback-container {
      min-height: 60vh;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  `]
})
export class AuthCallbackComponent implements OnInit {
  isLoading = true;
  errorMessage = '';
  isSuccess = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const error = params['error'];
      const token = params['token'];

      if (error) {
        this.isLoading = false;
        this.errorMessage = error;
        return;
      }

      if (token) {
        const username = params['username'] || '';
        const fullName = params['fullName'] || username;
        const role = params['role'] || 'User';

        this.authService.saveAuthTokenAndUser({
          token: token,
          username: username,
          fullName: fullName,
          role: role,
          expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString()
        });

        this.isLoading = false;
        this.isSuccess = true;

        setTimeout(() => {
          this.router.navigate(['/dashboard']);
        }, 1200);
      } else {
        this.isLoading = false;
        this.errorMessage = 'Không nhận được mã xác thực token từ máy chủ.';
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
