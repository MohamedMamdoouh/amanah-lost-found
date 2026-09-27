import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { BadgeComponent, BadgeVariant } from '../badge/badge.component';
import { IconComponent } from '../icon/icon.component';
import { ReportTypeMarkComponent } from '../report-type-mark/report-type-mark.component';

@Component({
  selector: 'app-report-masthead',
  standalone: true,
  imports: [
    BadgeComponent,
    IconComponent,
    ReportTypeMarkComponent,
    RouterLink,
  ],
  templateUrl: './report-masthead.component.html',
  styleUrl: './report-masthead.component.scss',
})
export class ReportMastheadComponent {
  readonly backLink = input.required<string | string[]>();
  readonly backLabel = input.required<string>();
  readonly title = input.required<string>();
  readonly titleId = input.required<string>();
  readonly reportType = input.required<string>();
  readonly badgeLabel = input<string | null>(null);
  readonly badgeVariant = input<BadgeVariant>('pending');
}
