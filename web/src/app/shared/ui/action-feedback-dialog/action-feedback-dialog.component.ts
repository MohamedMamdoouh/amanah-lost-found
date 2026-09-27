import {
  Component,
  DestroyRef,
  inject,
  input,
  OnInit,
  output,
  signal,
} from '@angular/core';
import { ButtonComponent } from '../button/button.component';
import { IconComponent } from '../icon/icon.component';

@Component({
  selector: 'app-action-feedback-dialog',
  standalone: true,
  imports: [ButtonComponent, IconComponent],
  templateUrl: './action-feedback-dialog.component.html',
  styleUrl: './action-feedback-dialog.component.scss',
})
export class ActionFeedbackDialogComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  readonly titleId = input.required<string>();
  readonly title = input.required<string>();
  readonly message = input.required<string>();
  readonly countdownSeconds = input(5);
  readonly closeLabel = input.required<string>();
  readonly countdownLabel = input.required<string>();

  readonly closed = output<void>();

  readonly secondsLeft = signal(0);

  private closedOnce = false;

  ngOnInit(): void {
    this.secondsLeft.set(this.countdownSeconds());
    const intervalId = setInterval(() => {
      const next = this.secondsLeft() - 1;
      if (next <= 0) {
        clearInterval(intervalId);
        this.emitClosed();
        return;
      }
      this.secondsLeft.set(next);
    }, 1000);

    this.destroyRef.onDestroy(() => clearInterval(intervalId));
  }

  countdownText(): string {
    return this.countdownLabel().replace(
      '{{seconds}}',
      String(this.secondsLeft()),
    );
  }

  onBackdropClick(): void {
    this.emitClosed();
  }

  onCloseClick(): void {
    this.emitClosed();
  }

  private emitClosed(): void {
    if (this.closedOnce) {
      return;
    }
    this.closedOnce = true;
    this.closed.emit();
  }
}

export interface ActionFeedbackState {
  title: string;
  message: string;
  onClosed?: () => void;
}
