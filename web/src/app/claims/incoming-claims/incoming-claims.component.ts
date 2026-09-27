import { Component, inject, OnInit, signal } from '@angular/core';
import { AppDatePipe } from '../../i18n/app-date.pipe';
import { RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { DomainLabelService } from '../../i18n/domain-label.service';
import { AlertComponent } from '../../shared/ui/alert/alert.component';
import { ButtonComponent } from '../../shared/ui/button/button.component';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state.component';
import { ListingCardComponent } from '../../shared/ui/listing-card/listing-card.component';
import { LoadingIndicatorComponent } from '../../shared/ui/loading-indicator/loading-indicator.component';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header.component';
import { ClaimService } from '../claim.service';
import { IncomingClaimInboxItem } from '../models/claim.models';

@Component({
  selector: 'app-incoming-claims',
  standalone: true,
  imports: [
    AppDatePipe,
    AlertComponent,
    ButtonComponent,
    EmptyStateComponent,
    ListingCardComponent,
    LoadingIndicatorComponent,
    PageHeaderComponent,
    RouterLink,
    TranslateModule,
  ],
  templateUrl: './incoming-claims.component.html',
  styleUrl: './incoming-claims.component.scss',
})
export class IncomingClaimsComponent implements OnInit {
  private readonly claimService = inject(ClaimService);
  protected readonly domainLabels = inject(DomainLabelService);
  private readonly translate = inject(TranslateService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly items = signal<IncomingClaimInboxItem[]>([]);

  ngOnInit(): void {
    void this.loadInbox();
  }

  reviewLink(item: IncomingClaimInboxItem): string[] {
    return ['/my/reports', item.reportId];
  }

  subtitle(item: IncomingClaimInboxItem): string {
    return this.translate.instant('claims.incoming.attempt', {
      n: item.attemptNumber,
      name: item.claimantDisplayName,
    });
  }

  private async loadInbox(): Promise<void> {
    try {
      const response = await firstValueFrom(this.claimService.getInbox());
      this.items.set(response.items);
      await this.claimService.refreshPendingInboxCount();
    } catch {
      this.error.set(this.translate.instant('error.internal.error'));
    } finally {
      this.loading.set(false);
    }
  }
}
