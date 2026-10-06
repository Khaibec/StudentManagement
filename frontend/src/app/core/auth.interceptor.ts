import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

// HttpInterceptor đóng vai trò như một Middleware ở tầng Frontend.
// Mọi HTTP request từ Angular gửi ra và response trả về từ Backend đều sẽ đi qua Interceptor này.
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // 1. Lấy mã token đang được lưu trong localStorage
    const token = this.authService.getToken();

    // 2. Nếu đã đăng nhập (có token), đính kèm token vào HTTP Header: "Authorization: Bearer <token>"
    if (token) {
      // Trong Angular, đối tượng HttpRequest là bất biến (Immutable - không thể sửa trực tiếp).
      // Do đó ta phải dùng .clone() để tạo bản sao và thêm header vào bản sao đó.
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    // 3. Chuyển tiếp request đã gắn token đi tới Backend thông qua next.handle()
    return next.handle(request).pipe(
      // Bắt lỗi tập trung cho toàn bộ ứng dụng:
      catchError((error: HttpErrorResponse) => {
        // Mã 401 (Unauthorized): Token đã hết hạn hoặc không hợp lệ
        // Tự động xóa token và chuyển hướng người dùng về trang đăng nhập
        if (error.status === 401) {
          this.authService.logout();
        }
        return throwError(error);
      })
    );
  }
}
