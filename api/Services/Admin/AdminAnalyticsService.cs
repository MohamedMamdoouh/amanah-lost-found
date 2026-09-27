using System.Linq.Expressions;
using Amanah.Api.Data;
using Amanah.Api.Data.Entities;
using Amanah.Api.Utilities.Abuse;
using Amanah.Contracts.Responses.Admin;
using Microsoft.EntityFrameworkCore;

namespace Amanah.Api.Services.Admin;

public sealed class AdminAnalyticsService(AppDbContext dbContext)
{
    public async Task<AdminAnalyticsOverviewResponse> GetOverviewAsync(
        CancellationToken cancellationToken = default)
    {
        var reports = dbContext.Reports.AsNoTracking();
        var abuseReports = dbContext.AbuseReports.AsNoTracking();
        var users = dbContext.Users.AsNoTracking();

        var pendingReview = await CountReportsByStatusAsync(ReportStatus.PendingReview, cancellationToken);

        return new AdminAnalyticsOverviewResponse
        {
            Impact = new AdminAnalyticsImpactResponse
            {
                ReunionsResolved = await CountReportsLostFoundAsync(
                    report => report.Status == ReportStatus.Resolved,
                    cancellationToken),
                LivePublished = await CountReportsLostFoundAsync(
                    report => report.Status == ReportStatus.Published,
                    cancellationToken),
                ClaimInProgress = await CountReportsLostFoundAsync(
                    report => report.Status == ReportStatus.ClaimInProgress,
                    cancellationToken),
            },
            Reports = new AdminAnalyticsReportsResponse
            {
                PendingReview = pendingReview,
                Rejected = await CountReportsByStatusAsync(ReportStatus.Rejected, cancellationToken),
                Withdrawn = await CountReportsByStatusAsync(ReportStatus.Withdrawn, cancellationToken),
                RemovedByAdmin = await CountReportsByStatusAsync(ReportStatus.RemovedByAdmin, cancellationToken),
                All = await reports.CountAsync(cancellationToken),
                AllByType = await CountReportsLostFoundAsync(cancellationToken: cancellationToken),
            },
            Queues = new AdminAnalyticsQueuesResponse
            {
                ModerationPending = pendingReview,
                OpenFlags = await abuseReports.CountAsync(
                    abuseReport => abuseReport.Status == AbuseReportStatus.Open,
                    cancellationToken),
                PendingClaimsAwaitingReporter = await dbContext.Claims.AsNoTracking().CountAsync(
                    claim => claim.Status == ClaimStatus.Pending
                        && claim.Report.Status == ReportStatus.Published,
                    cancellationToken),
            },
            Abuse = new AdminAnalyticsAbuseResponse
            {
                TotalSubmitted = await abuseReports.CountAsync(cancellationToken),
                Open = await abuseReports.CountAsync(
                    abuseReport => abuseReport.Status == AbuseReportStatus.Open,
                    cancellationToken),
                ResolvedNoAction = await CountAbuseByOutcomeAsync(AbuseResolutionOutcomes.NoAction, cancellationToken),
                ResolvedTakedown = await CountAbuseByOutcomeAsync(AbuseResolutionOutcomes.Takedown, cancellationToken),
                ResolvedBan = await CountAbuseByOutcomeAsync(AbuseResolutionOutcomes.Ban, cancellationToken),
            },
            Users = new AdminAnalyticsUsersResponse
            {
                RegisteredUsers = await users.CountAsync(
                    user => user.Role == UserRole.User,
                    cancellationToken),
                Banned = await users.CountAsync(user => user.IsBanned, cancellationToken),
                Deactivated = await users.CountAsync(
                    user => user.DeactivatedAt != null,
                    cancellationToken),
            },
        };
    }

    private async Task<int> CountReportsByStatusAsync(
        ReportStatus status,
        CancellationToken cancellationToken) =>
        await dbContext.Reports.AsNoTracking().CountAsync(
            report => report.Status == status,
            cancellationToken);

    private async Task<int> CountAbuseByOutcomeAsync(
        string outcome,
        CancellationToken cancellationToken) =>
        await dbContext.AbuseReports.AsNoTracking().CountAsync(
            abuseReport => abuseReport.Status == AbuseReportStatus.Resolved
                && abuseReport.ResolutionOutcome == outcome,
            cancellationToken);

    private async Task<AdminAnalyticsLostFoundCountResponse> CountReportsLostFoundAsync(
        Expression<Func<Report, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Reports.AsNoTracking();
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        var lost = await query.CountAsync(report => report.Type == ReportType.Lost, cancellationToken);
        var found = await query.CountAsync(report => report.Type == ReportType.Found, cancellationToken);

        return new AdminAnalyticsLostFoundCountResponse
        {
            Lost = lost,
            Found = found,
        };
    }
}
