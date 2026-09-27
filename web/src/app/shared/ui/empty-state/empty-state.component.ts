import { Component, input } from '@angular/core';

import { IconComponent, IconName } from '../icon/icon.component';

export type EmptyStateVariant = 'default' | 'success';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [IconComponent],
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.scss',
})
export class EmptyStateComponent {
  readonly title = input.required<string>();
  readonly description = input<string | null>(null);
  readonly variant = input<EmptyStateVariant>('default');
  /** Softer block for nested panels (no blob decoration). */
  readonly inset = input(false);
  readonly icon = input<IconName | null>(null);
}
