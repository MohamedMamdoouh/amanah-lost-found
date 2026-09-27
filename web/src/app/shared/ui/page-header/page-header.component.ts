import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { IconComponent } from '../icon/icon.component';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [RouterLink, IconComponent],
  templateUrl: './page-header.component.html',
  styleUrl: './page-header.component.scss',
})
export class PageHeaderComponent {
  readonly eyebrow = input<string | null>(null);
  readonly title = input.required<string>();
  readonly titleId = input<string | null>(null);
  readonly intro = input<string | null>(null);
  readonly backLink = input<string | null>(null);
  readonly backLabel = input<string | null>(null);
}
