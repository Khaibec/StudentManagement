import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ApiResponse, AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5000/api/auth';
  private readonly TOKEN_KEY = 'sm_token';
  private readonly USER_KEY = 'sm_user';

  // BehaviorSubject trong RxJS lưu giữ giá trị hiện tại của user đã đăng nhập.
  // Khi một component mới subscribe vào, nó sẽ lập tức nhận được giá trị mới nhất này ngay.
  private currentUserSubject: BehaviorSubject<AuthResponse | null>;
  
  // Public biến này dưới dạng Observable (chỉ đọc) để các component khác có thể lắng nghe (subcribe)
  // mà không thể tự ý phát sinh dữ liệu bừa bãi bằng lệnh .next() từ bên ngoài.
  public currentUser$: Observable<AuthResponse | null>;

  constructor(private http: HttpClient, private router: Router) {
    // Khởi tạo BehaviorSubject với dữ liệu đã lưu trong localStorage (giúp giữ trạng thái đăng nhập khi ấn F5)
    this.currentUserSubject = new BehaviorSubject<AuthResponse | null>(this.getUserFromStorage());
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  // Getter cho phép đọc nhanh thông tin user hiện tại đồng bộ (synchronous) mà không cần phải .subscribe()
  public get currentUserValue(): AuthResponse | null {
    return this.currentUserSubject.value;
  }

  public isLoggedIn(): boolean {
    return !!this.currentUserValue && !!this.getToken();
  }

  public isAdmin(): boolean {
    return this.currentUserValue?.role === 'Admin';
  }

  login(request: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login`, request).pipe(
      // Toán tử tap() của RxJS dùng để thực hiện "tác dụng phụ" (Side Effect) như lưu token,
      // mà không làm thay đổi luồng dữ liệu trả về cho component gọi hàm này.
      tap((res) => {
        if (res.success && res.data) {
          this.saveAuth(res.data);
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/register`, request).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this.saveAuth(res.data);
        }
      })
    );
  }

  logout(): void {
    // Xóa sạch thông tin xác thực khỏi trình duyệt
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    // Bắn thông báo null tới toàn bộ các component đang lắng nghe (để cập nhật lại giao diện Navbar, Menu...)
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private saveAuth(authData: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authData.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(authData));
    this.currentUserSubject.next(authData);
  }

  private getUserFromStorage(): AuthResponse | null {
    const raw = localStorage.getItem(this.USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as AuthResponse;
    } catch {
      return null;
    }
  }
}
