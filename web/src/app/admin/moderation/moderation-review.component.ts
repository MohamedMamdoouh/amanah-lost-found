import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { ApiErrorService } from '../../i18n/api-error.service';
import {
  ActionFeedbackDialogComponent,
  ActionFeedbackState,
} from '../../shared/ui/action-feedback-dialog/action-feedback-dialog.component';
import { AlertComponent } from '../../shared/ui/alert/alert.component';
import { ButtonComponent } from '../../shared/ui/button/button.component';
import { ConfirmDialogComponent } from '../../shared/ui/confirm-dialog/confirm-dialog.component';
import { LoadingIndicatorComponent } from '../../shared/ui/loading-indicator/loading-indicator.component';
import { DomainLabelService } from '../../i18n/domain-label.service';
import { ReportMastheadComponent } from '../../shared/ui/report-masthead/report-masthead.component';
import { ReportDossierComponent } from '../../shared/ui/report-dossier/report-dossier.component';
import { ReportDetail } from '../../reports/models/report.models';
import {
  DisplayPhoto,
  initialDisplayPhotos,
  loadDisplayPhotos,
} from '../../uploads/photo-loader.util';
import { ReportPhotoUploadService } from '../../uploads/report-photo-upload.service';
import { AdminModerationService } from '../admin-moderation.service';
import { RejectReportDialogComponent } from './reject-report-dialog.component';

@Component({
  selector: 'app-moderation-review',
  standalone: true,
  imports: [
    ActionFeedbackDialogComponent,
    AlertComponent,
    ButtonComponent,
    ConfirmDialogComponent,
    LoadingIndicatorComponent,
    ReportMastheadComponent,
    RejectReportDialogComponent,
    ReportDossierComponent,
    TranslateModule,
  ],
  templateUrl: './moderation-review.component.html',
  styleUrl: './moderation-review.component.scss',
})
export class ModerationReviewComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly moderationService = inject(AdminModerationService);
  private readonly uploadService = inject(ReportPhotoUploadService);
  private readonly apiErrors = inject(ApiErrorService);
  private readonly translate = inject(TranslateService);
  protected readonly domainLabels = inject(DomainLabelService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly report = signal<ReportDetail | null>(null);
  readonly photos = signal<DisplayPhoto[]>([]);
  readonly showRejectDialog = signal(false);
  readonly showApproveConfirm = signal(false);
  readonly actionError = signal<string | null>(null);
  readonly approving = signal(false);
  readonly actionFeedback = signal<ActionFeedbackState | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set(this.translate.instant('error.internal.error'));
      this.loading.set(false);
      return;
    }

    void this.loadReport(id);
  }

  openReject(): void {
    this.actionError.set(null);
    this.showRejectDialog.set(true);
  }

  closeReject(): void {
    this.showRejectDialog.set(false);
  }

  openApproveConfirm(): void {
    this.actionError.set(null);
    this.showApproveConfirm.set(true);
  }

  closeApproveConfirm(): void {
    this.showApproveConfirm.set(false);
  }

  async confirmApprove(): Promise<void> {
    const report = this.report();
    if (!report || this.approving()) {
      return;
    }

    this.approving.set(true);
    this.actionError.set(null);

    try {
      await firstValueFrom(this.moderationService.approve(report.id));
      this.closeApproveConfirm();
      this.openActionFeedback(
        this.translate.instant('common.dialog.done_title'),
        this.translate.instant('admin.moderation.done_approved'),
        () => void this.router.navigate(['/admin/moderation']),
      );
    } catch (error) {
      this.actionError.set(this.apiErrors.messageFromHttpError(error));
    } finally {
      this.approving.set(false);
    }
  }

  onRejected(): void {
    this.closeReject();
    this.openActionFeedback(
      this.translate.instant('common.dialog.done_title'),
      this.translate.instant('admin.moderation.done_rejected'),
      () => void this.router.navigate(['/admin/moderation']),
    );
  }

  openActionFeedback(
    title: string,
    message: string,
    onClosed?: () => void,
  ): void {
    this.actionFeedback.set({ title, message, onClosed });
  }

  closeActionFeedback(): void {
    const feedback = this.actionFeedback();
    this.actionFeedback.set(null);
    feedback?.onClosed?.();
  }

  private async loadReport(id: string): Promise<void> {
    try {
      const report = await firstValueFrom(this.moderationService.getReport(id));
      this.report.set(report);
      void this.loadPhotos(report);
    } catch (error) {
      if (error instanceof HttpErrorResponse && error.status === 404) {
        this.error.set(this.translate.instant('reports.detail.not_found'));
      } else {
        this.error.set(this.translate.instant('error.internal.error'));
      }
    } finally {
      this.loading.set(false);
    }
  }

  private async loadPhotos(report: ReportDetail): Promise<void> {
    const displayPhotos = initialDisplayPhotos(report.photos);
    this.photos.set(displayPhotos);
    await loadDisplayPhotos(
      displayPhotos,
      this.uploadService,
      (photoId, patch) => this.updatePhoto(photoId, patch),
    );
  }

  private updatePhoto(
    photoId: string,
    patch: Partial<Pick<DisplayPhoto, 'url' | 'loading'>>,
  ): void {
    this.photos.update((current) =>
      current.map((item) =>
        item.id === photoId ? { ...item, ...patch } : item,
      ),
    );
  }
}
