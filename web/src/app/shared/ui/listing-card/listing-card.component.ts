import { NgTemplateOutlet } from '@angular/common';
import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { BadgeComponent, BadgeVariant } from '../badge/badge.component';
import { IconComponent, IconName } from '../icon/icon.component';
import { ReportTypeMarkComponent } from '../report-type-mark/report-type-mark.component';

@Component({
  selector: 'app-listing-card',
  standalone: true,
  imports: [
    NgTemplateOutlet,
    RouterLink,
    BadgeComponent,
    IconComponent,
    ReportTypeMarkComponent,
  ],
  templateUrl: './listing-card.component.html',
  styleUrl: './listing-card.component.scss',
})
export class ListingCardComponent {
  readonly title = input.required<string>();
  readonly titleIcon = input<IconName | null>(null);
  readonly subtitleLead = input<string | null>(null);
  readonly subtitle = input<string | null>(null);
  readonly subtitleIcon = input<IconName | null>(null);
  readonly location = input<string | null>(null);
  readonly date = input<string | null>(null);
  readonly reportType = input<string | null>(null);
  readonly badgeLabel = input<string | null>(null);
  readonly badgeVariant = input<BadgeVariant>('neutral');
  readonly imageUrl = input<string | null>(null);
  readonly routerLink = input<string | string[] | null>(null);
  readonly fragment = input<string | null>(null);
  readonly actionLabel = input<string | null>(null);
  /** When true, the card body is not a link (use footer actions to navigate). */
  readonly splitLink = input(false);
  readonly unread = input(false);
}
