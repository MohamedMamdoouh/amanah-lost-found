import { Component, computed, input } from '@angular/core';

import { IconComponent, IconName } from '../icon/icon.component';

export type BadgeVariant =
  | 'pending'
  | 'approved'
  | 'rejected'
  | 'neutral'
  | 'published'
  | 'lost'
  | 'found'
  | 'claim'
  | 'resolved';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [IconComponent],
  template: `
    <span
      class="badge"
      [class.badge--pending]="variant() === 'pending'"
      [class.badge--approved]="variant() === 'approved'"
      [class.badge--rejected]="variant() === 'rejected'"
      [class.badge--neutral]="variant() === 'neutral'"
      [class.badge--published]="variant() === 'published'"
      [class.badge--lost]="variant() === 'lost'"
      [class.badge--found]="variant() === 'found'"
      [class.badge--claim]="variant() === 'claim'"
      [class.badge--resolved]="variant() === 'resolved'"
      [class.badge--dot]="dot()"
    >
      @if (statusIcon(); as icon) {
        <app-icon class="badge__icon" [name]="icon" size="sm" />
      }
      <span class="badge__label"><ng-content /></span>
    </span>
  `,
  styleUrl: './badge.component.scss',
})
export class BadgeComponent {
  readonly variant = input<BadgeVariant>('neutral');
  readonly dot = input(false);
  readonly showIcon = input(true);

  readonly statusIcon = computed((): IconName | null => {
    if (this.dot() || !this.showIcon()) {
      return null;
    }

    switch (this.variant()) {
      case 'pending':
        return 'clock';
      case 'approved':
      case 'published':
      case 'resolved':
        return 'check';
      case 'rejected':
        return 'close';
      case 'lost':
        return 'lost';
      case 'found':
        return 'found';
      case 'claim':
        return 'chat';
      case 'neutral':
        return 'shield';
      default:
        return null;
    }
  });
}
