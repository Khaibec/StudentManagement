import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LinkedAccountDto, UserProfile } from '../../models/auth.model';

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
