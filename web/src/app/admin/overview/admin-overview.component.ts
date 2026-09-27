import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { AlertComponent } from '../../shared/ui/alert/alert.component';
import { LoadingIndicatorComponent } from '../../shared/ui/loading-indicator/loading-indicator.component';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header.component';
import {
  AdminAnalyticsLostFoundCount,
  AdminAnalyticsOverview,
  AdminAnalyticsService,
} from '../admin-analytics.service';

@Component({
  selector: 'app-admin-overview',
  standalone: true,
  imports: [
    AlertComponent,
    LoadingIndicatorComponent,
    PageHeaderComponent,
    RouterLink,
    TranslateModule,
  ],
  templateUrl: './admin-overview.component.html',
  styleUrl: './admin-overview.component.scss',
})
export class AdminOverviewComponent implements OnInit {
  private readonly analyticsService = inject(AdminAnalyticsService);
  private readonly translate = inject(TranslateService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly overview = signal<AdminAnalyticsOverview | null>(null);

  ngOnInit(): void {
    void this.loadOverview();
  }

  lostFoundLabel(counts: AdminAnalyticsLostFoundCount): string {
    return this.translate.instant('admin.analytics.lost_found', {
      lost: counts.lost,
      found: counts.found,
    });
  }

  private async loadOverview(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const overview = await firstValueFrom(this.analyticsService.getOverview());
      this.overview.set(overview);
    } catch {
      this.error.set(this.translate.instant('error.internal.error'));
    } finally {
      this.loading.set(false);
    }
  }
}
