import { Component, inject, OnInit, signal } from '@angular/core';
import { AppDatePipe } from '../../i18n/app-date.pipe';
import { RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { ApiErrorService } from '../../i18n/api-error.service';
import { DomainLabelService } from '../../i18n/domain-label.service';
import {
  ActionFeedbackDialogComponent,
  ActionFeedbackState,
} from '../../shared/ui/action-feedback-dialog/action-feedback-dialog.component';
import { AlertComponent } from '../../shared/ui/alert/alert.component';
import { ButtonComponent } from '../../shared/ui/button/button.component';
import { ConfirmDialogComponent } from '../../shared/ui/confirm-dialog/confirm-dialog.component';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state.component';
import { ListingCardComponent } from '../../shared/ui/listing-card/listing-card.component';
import { LoadingIndicatorComponent } from '../../shared/ui/loading-indicator/loading-indicator.component';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header.component';
import { ClaimService } from '../claim.service';
import { MyClaimSummary } from '../models/claim.models';

@Component({
  selector: 'app-my-claims',
  standalone: true,
  imports: [
    AppDatePipe,
    ActionFeedbackDialogComponent,
    AlertComponent,
    ButtonComponent,
    ConfirmDialogComponent,
    EmptyStateComponent,
    ListingCardComponent,
    LoadingIndicatorComponent,
    PageHeaderComponent,
    RouterLink,
    TranslateModule,
  ],
  templateUrl: './my-claims.component.html',
  styleUrl: './my-claims.component.scss',
})
export class MyClaimsComponent implements OnInit {
  private readonly claimService = inject(ClaimService);
  private readonly apiErrors = inject(ApiErrorService);
  protected readonly domainLabels = inject(DomainLabelService);
  private readonly translate = inject(TranslateService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly claims = signal<MyClaimSummary[]>([]);
  readonly withdrawingId = signal<string | null>(null);
  readonly confirmWithdrawId = signal<string | null>(null);
  readonly actionError = signal<string | null>(null);
  readonly actionFeedback = signal<ActionFeedbackState | null>(null);

  ngOnInit(): void {
    void this.loadClaims();
  }

  reportLink(claim: MyClaimSummary): string[] {
    return [`/${claim.reportType}`, claim.reportId];
  }

  canWithdraw(claim: MyClaimSummary): boolean {
    return claim.status === 'pending';
  }

  openWithdraw(claim: MyClaimSummary): void {
    this.actionError.set(null);
    this.confirmWithdrawId.set(claim.id);
  }

  onWithdrawClick(event: MouseEvent, claim: MyClaimSummary): void {
    event.preventDefault();
    event.stopPropagation();
    this.openWithdraw(claim);
  }

  closeWithdrawConfirm(): void {
    this.confirmWithdrawId.set(null);
  }

  async confirmWithdraw(): Promise<void> {
    const claimId = this.confirmWithdrawId();
    if (!claimId || this.withdrawingId()) {
      return;
    }

    this.withdrawingId.set(claimId);
    this.actionError.set(null);

    try {
      await firstValueFrom(this.claimService.withdraw(claimId));
      this.claims.update((current) =>
        current.map((item) =>
          item.id === claimId ? { ...item, status: 'withdrawn' } : item,
        ),
      );
      this.closeWithdrawConfirm();
      this.actionFeedback.set({
        title: this.translate.instant('common.dialog.done_title'),
        message: this.translate.instant('claims.mine.withdraw_done'),
      });
    } catch (error) {
      this.actionError.set(
        this.apiErrors.messageFromHttpError(error, {
          conflictKey: 'claims.review.invalid_status',
        }),
      );
    } finally {
      this.withdrawingId.set(null);
    }
  }

  closeActionFeedback(): void {
    const feedback = this.actionFeedback();
    this.actionFeedback.set(null);
    feedback?.onClosed?.();
  }

  private async loadClaims(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const response = await firstValueFrom(this.claimService.getMine());
      this.claims.set(response.items);
    } catch {
      this.error.set(this.translate.instant('error.internal.error'));
    } finally {
      this.loading.set(false);
    }
  }
}
