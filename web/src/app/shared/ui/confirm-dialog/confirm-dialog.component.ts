import { Component, computed, input, output } from '@angular/core';

import { ButtonComponent, ButtonVariant } from '../button/button.component';
import { IconComponent, IconName } from '../icon/icon.component';

export type DialogIconTone = 'default' | 'success' | 'warning' | 'danger' | 'info';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [ButtonComponent, IconComponent],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.scss',
})
export class ConfirmDialogComponent {
  readonly titleId = input.required<string>();
  readonly title = input.required<string>();
  readonly intro = input.required<string>();
  readonly cancelLabel = input.required<string>();
  readonly confirmLabel = input.required<string>();
  readonly confirmVariant = input<ButtonVariant>('primary');
  readonly icon = input<IconName | null>(null);
  readonly iconTone = input<DialogIconTone | null>(null);
  readonly loading = input(false);
  readonly disabled = input(false);

  readonly cancelled = output<void>();
  readonly confirmed = output<void>();

  readonly resolvedIcon = computed(() => this.icon() ?? this.defaultIcon());
  readonly resolvedIconTone = computed(
    () => this.iconTone() ?? this.defaultIconTone(),
  );

  onBackdropClick(): void {
    if (!this.loading() && !this.disabled()) {
      this.cancelled.emit();
    }
  }

  private defaultIcon(): IconName {
    return this.confirmVariant() === 'danger' ? 'lost' : 'shield';
  }

  private defaultIconTone(): DialogIconTone {
    switch (this.confirmVariant()) {
      case 'danger':
        return 'danger';
      case 'outline':
        return 'warning';
      default:
        return 'default';
    }
  }
}
