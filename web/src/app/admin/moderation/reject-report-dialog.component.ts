import { Component, inject, input, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { ApiErrorService } from '../../i18n/api-error.service';
import { AlertComponent } from '../../shared/ui/alert/alert.component';
import { ButtonComponent } from '../../shared/ui/button/button.component';
import { IconComponent } from '../../shared/ui/icon/icon.component';
import { AdminModerationService } from '../admin-moderation.service';

const REJECTION_REASON_CODES = [
  'rejection.unclear_photos',
  'rejection.spam_or_scam',
  'rejection.duplicate_report',
  'rejection.insufficient_description',
  'rejection.contact_info',
  'rejection.prohibited_item',
  'rejection.wrong_category',
  'rejection.raw_id_number',
] as const;

@Component({
  selector: 'app-reject-report-dialog',
  standalone: true,
  imports: [
    AlertComponent,
    ButtonComponent,
    IconComponent,
    ReactiveFormsModule,
    TranslateModule,
  ],
  templateUrl: './reject-report-dialog.component.html',
  styleUrl: './reject-report-dialog.component.scss',
})
export class RejectReportDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly moderationService = inject(AdminModerationService);
  private readonly apiErrors = inject(ApiErrorService);
  private readonly translate = inject(TranslateService);

  readonly reportId = input.required<string>();

  readonly closed = output<void>();
  readonly rejected = output<void>();

  readonly rejecting = signal(false);
  readonly summaryError = signal<string | null>(null);

  readonly rejectionReasons = REJECTION_REASON_CODES;

  readonly form = this.fb.nonNullable.group({
    reasonCode: ['', Validators.required],
    note: [''],
  });

  reasonLabel(code: string): string {
    return this.translate.instant(code);
  }

  onBackdropClick(): void {
    if (!this.rejecting()) {
      this.closed.emit();
    }
  }

  onCancel(): void {
    if (!this.rejecting()) {
      this.closed.emit();
    }
  }

  async onSubmit(): Promise<void> {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.summaryError.set(
        this.translate.instant('common.form.validation_summary'),
      );
      return;
    }

    this.rejecting.set(true);
    this.summaryError.set(null);

    try {
      await firstValueFrom(
        this.moderationService.reject(this.reportId(), {
          reasonCode: this.form.controls.reasonCode.value,
          note: this.form.controls.note.value || null,
        }),
      );
      this.rejected.emit();
    } catch (error) {
      this.summaryError.set(this.apiErrors.messageFromHttpError(error));
    } finally {
      this.rejecting.set(false);
    }
  }
}
