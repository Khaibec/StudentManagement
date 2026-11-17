import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ChangePasswordRequest, LinkedAccountDto, UpdateProfileRequest, UserProfile } from '../../models/auth.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profile: UserProfile | null = null;
  linkedAccounts: LinkedAccountDto[] = [];
  isLoadingProfile = false;
  isLoadingAccounts = false;

  successMessage = '';
  errorMessage = '';

  // Modal xác nhận hủy liên kết
  showUnlinkModal = false;
  providerToUnlink: string | null = null;
  isUnlinking = false;

  // Cập nhật thông tin cá nhân
  isEditingProfile = false;
  editProfileData: UpdateProfileRequest = { fullName: '', email: '' };
  isSavingProfile = false;

  // Tải lên ảnh đại diện
  isUploadingAvatar = false;

  // Đổi mật khẩu / Thiết lập mật khẩu
  showPasswordModal = false;
  passwordData: ChangePasswordRequest = { currentPassword: '', newPassword: '', confirmPassword: '' };
  passwordModalError = '';
  isChangingPassword = false;

  constructor(
    public authService: AuthService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadProfile();
    this.loadLinkedAccounts();

    // Lắng nghe kết quả điều hướng callback sau khi liên kết OAuth
    this.route.queryParams.subscribe((params) => {
      if (params['linkSuccess'] === 'true') {
        const provider = params['provider'] || 'bên thứ ba';
        this.successMessage = `Liên kết tài khoản ${provider} thành công!`;
        this.loadLinkedAccounts();
      } else if (params['linkError']) {
        this.errorMessage = params['linkError'];
      }
    });
  }

  loadProfile(): void {
    this.isLoadingProfile = true;
    this.authService.getProfile().subscribe(
      (res) => {
        this.isLoadingProfile = false;
        if (res.success) {
          this.profile = res.data;
        }
      },
      () => {
        this.isLoadingProfile = false;
      }
    );
  }

  loadLinkedAccounts(): void {
    this.isLoadingAccounts = true;
    this.authService.getLinkedAccounts().subscribe(
      (res) => {
        this.isLoadingAccounts = false;
        if (res.success) {
          this.linkedAccounts = res.data;
        }
      },
      () => {
        this.isLoadingAccounts = false;
        this.errorMessage = 'Không thể tải danh sách tài khoản liên kết.';
      }
    );
  }

  getAvatarUrl(path?: string): string {
    if (!path) return '';
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }
    const backendBase = environment.apiUrl.replace(/\/api\/?$/, '');
    return `${backendBase}${path.startsWith('/') ? '' : '/'}${path}`;
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files?.[0];
    if (!file) return;

    if (file.size > 2 * 1024 * 1024) {
      this.errorMessage = 'Kích thước ảnh đại diện không được vượt quá 2MB.';
      return;
    }

    const allowedExtensions = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedExtensions.includes(file.type)) {
      this.errorMessage = 'Chỉ chấp nhận file ảnh định dạng JPG, JPEG, PNG hoặc WEBP.';
      return;
    }

    this.isUploadingAvatar = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.uploadAvatar(file).subscribe(
      (res) => {
        this.isUploadingAvatar = false;
        if (res.success && res.data) {
          if (this.profile) {
            this.profile.avatarUrl = res.data.avatarUrl;
          }
          this.successMessage = 'Cập nhật ảnh đại diện thành công!';
        }
      },
      (err) => {
        this.isUploadingAvatar = false;
        this.errorMessage = err.error?.message || 'Tải lên ảnh đại diện thất bại.';
      }
    );
  }

  startEditProfile(): void {
    if (!this.profile) return;
    this.editProfileData = {
      fullName: this.profile.fullName,
      email: this.profile.email || ''
    };
    this.isEditingProfile = true;
  }

  cancelEditProfile(): void {
    this.isEditingProfile = false;
  }

  saveProfile(): void {
    if (!this.editProfileData.fullName.trim()) {
      this.errorMessage = 'Họ và tên không được để trống.';
      return;
    }

    this.isSavingProfile = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.updateProfile(this.editProfileData).subscribe(
      (res) => {
        this.isSavingProfile = false;
        if (res.success) {
          this.profile = res.data;
          this.isEditingProfile = false;
          this.successMessage = 'Cập nhật thông tin hồ sơ thành công!';
        }
      },
      (err) => {
        this.isSavingProfile = false;
        this.errorMessage = err.error?.message || 'Cập nhật hồ sơ thất bại.';
      }
    );
  }

  openPasswordModal(): void {
    this.passwordData = { currentPassword: '', newPassword: '', confirmPassword: '' };
    this.passwordModalError = '';
    this.showPasswordModal = true;
  }

  closePasswordModal(): void {
    this.showPasswordModal = false;
    this.passwordModalError = '';
  }

  savePassword(): void {
    this.passwordModalError = '';

    if (this.profile?.hasPassword && !this.passwordData.currentPassword) {
      this.passwordModalError = 'Vui lòng nhập mật khẩu hiện tại.';
      return;
    }

    if (!this.passwordData.newPassword || this.passwordData.newPassword.length < 6) {
      this.passwordModalError = 'Mật khẩu mới phải có ít nhất 6 ký tự.';
      return;
    }

    if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
      this.passwordModalError = 'Mật khẩu xác nhận không khớp.';
      return;
    }

    this.isChangingPassword = true;

    this.authService.changePassword(this.passwordData).subscribe(
      (res) => {
        this.isChangingPassword = false;
        if (res.success) {
          if (this.profile) {
            this.profile.hasPassword = true;
          }
          this.closePasswordModal();
          this.successMessage = 'Cập nhật mật khẩu thành công!';
        }
      },
      (err) => {
        this.isChangingPassword = false;
        this.passwordModalError = err.error?.message || 'Đổi mật khẩu thất bại.';
      }
    );
  }

  connectAccount(provider: string): void {
    this.authService.linkOAuthAccount(provider);
  }

  openUnlinkModal(provider: string): void {
    this.providerToUnlink = provider;
    this.showUnlinkModal = true;
  }

  closeUnlinkModal(): void {
    this.showUnlinkModal = false;
    this.providerToUnlink = null;
  }

  confirmUnlink(): void {
    if (!this.providerToUnlink) return;

    this.isUnlinking = true;
    this.errorMessage = '';
    this.successMessage = '';

    const provider = this.providerToUnlink;
    this.authService.unlinkAccount(provider).subscribe(
      (res) => {
        this.isUnlinking = false;
        this.closeUnlinkModal();
        if (res.success) {
          this.successMessage = `Đã hủy liên kết tài khoản ${provider} thành công!`;
          this.loadLinkedAccounts();
          this.loadProfile();
        }
      },
      (err) => {
        this.isUnlinking = false;
        this.closeUnlinkModal();
        this.errorMessage = err.error?.message || `Không thể hủy liên kết tài khoản ${provider}.`;
      }
    );
  }

  getProviderIcon(provider: string): string {
    switch (provider.toLowerCase()) {
      case 'google': return 'bi bi-google text-danger';
      case 'facebook': return 'bi bi-facebook text-primary';
      case 'github': return 'bi bi-github text-dark';
      default: return 'bi bi-globe text-secondary';
    }
  }
}
