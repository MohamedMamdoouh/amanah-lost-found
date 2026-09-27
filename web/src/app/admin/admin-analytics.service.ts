import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

export interface AdminAnalyticsLostFoundCount {
  lost: number;
  found: number;
}

export interface AdminAnalyticsImpact {
  reunionsResolved: AdminAnalyticsLostFoundCount;
  livePublished: AdminAnalyticsLostFoundCount;
  claimInProgress: AdminAnalyticsLostFoundCount;
}

export interface AdminAnalyticsReports {
  pendingReview: number;
  rejected: number;
  withdrawn: number;
  removedByAdmin: number;
  all: number;
  allByType: AdminAnalyticsLostFoundCount;
}

export interface AdminAnalyticsQueues {
  moderationPending: number;
  openFlags: number;
  pendingClaimsAwaitingReporter: number;
}

export interface AdminAnalyticsAbuse {
  totalSubmitted: number;
  open: number;
  resolvedNoAction: number;
  resolvedTakedown: number;
  resolvedBan: number;
}

export interface AdminAnalyticsUsers {
  registeredUsers: number;
  banned: number;
  deactivated: number;
}

export interface AdminAnalyticsOverview {
  impact: AdminAnalyticsImpact;
  reports: AdminAnalyticsReports;
  queues: AdminAnalyticsQueues;
  abuse: AdminAnalyticsAbuse;
  users: AdminAnalyticsUsers;
}

@Injectable({ providedIn: 'root' })
export class AdminAnalyticsService {
  private readonly http = inject(HttpClient);

  private get baseUrl(): string {
    return `${environment.apiBaseUrl}/admin/analytics`;
  }

  getOverview(): Observable<AdminAnalyticsOverview> {
    return this.http.get<AdminAnalyticsOverview>(`${this.baseUrl}/overview`);
  }
}
