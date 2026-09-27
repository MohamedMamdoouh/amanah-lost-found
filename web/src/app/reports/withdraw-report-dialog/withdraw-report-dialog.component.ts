import { Component, inject, input, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { ApiErrorService } from '../../i18n/api-error.service';
import { AlertComponent } from '../../shared/ui/alert/alert.component';
import { ButtonComponent } from '../../shared/ui/button/button.component';
import { IconComponent } from '../../shared/ui/icon/icon.component';
import { WithdrawalReason } from '../models/report.models';
import { ReportService } from '../report.service';

@Component({
  selector: 'app-withdraw-report-dialog',
  standalone: true,
  imports: [
    AlertComponent,
    ButtonComponent,
    IconComponent,
    ReactiveFormsModule,
    TranslateModule,
  ],
  templateUrl: './withdraw-report-dialog.component.html',
  styleUrl: './withdraw-report-dialog.component.scss',
})
export class WithdrawReportDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly reportService = inject(ReportService);
  private readonly apiErrors = inject(ApiErrorService);
  private readonly translate = inject(TranslateService);

  readonly reportId = input.required<string>();

  readonly closed = output<void>();
  readonly withdrawn = output<void>();

  readonly withdrawing = signal(false);
  readonly summaryError = signal<string | null>(null);

  readonly withdrawalReasons: WithdrawalReason[] = [
    'recovered_outside',
    'no_longer_needed',
    'posted_by_mistake',
    'other',
  ];

  readonly form = this.fb.nonNullable.group({
    reason: ['' as WithdrawalReason | '', Validators.required],
  });

  reasonLabel(reason: WithdrawalReason): string {
    return this.translate.instant(`reports.withdraw.reasons.${reason}`);
  }

  onBackdropClick(): void {
    if (!this.withdrawing()) {
      this.closed.emit();
    }
  }

  onCancel(): void {
    if (!this.withdrawing()) {
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

    this.withdrawing.set(true);
    this.summaryError.set(null);

    try {
      await firstValueFrom(
        this.reportService.withdraw(this.reportId(), {
          reason: this.form.controls.reason.value as WithdrawalReason,
        }),
      );
      this.withdrawn.emit();
    } catch (error) {
      this.summaryError.set(this.apiErrors.messageFromHttpError(error));
    } finally {
      this.withdrawing.set(false);
    }
  }
}
