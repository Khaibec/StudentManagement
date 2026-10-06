import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// AuthGuard: "Người gác cổng" kiểm tra trạng thái đăng nhập trước khi cho phép vào các trang nội bộ.
// CanActivate là interface của Angular Router, nếu trả về true thì cho vào, false thì chặn lại.
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    // Đã đăng nhập hợp lệ => Cho phép truy cập route
    if (this.authService.isLoggedIn()) {
      return true;
    }

    // Chưa đăng nhập => Chuyển hướng người dùng về trang /login và chặn không cho truy cập
    this.router.navigate(['/login']);
    return false;
  }
}

// AdminGuard: Bảo vệ các trang nhạy cảm chỉ dành riêng cho quyền quản trị viên (Admin).
@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    // Phải thỏa mãn cả 2 điều kiện: Đã đăng nhập VÀ có Role là 'Admin'
    if (this.authService.isLoggedIn() && this.authService.isAdmin()) {
      return true;
    }

    // Nếu chỉ là User thường => Chuyển hướng về Dashboard, không cho vào trang quản trị
    this.router.navigate(['/dashboard']);
    return false;
  }
}
