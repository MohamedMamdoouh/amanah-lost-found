using System.Net;
using System.Net.Http.Json;
using Amanah.Api.Data.Entities;
using Amanah.Api.Services.Lifecycle;
using Amanah.Api.Tests.Browse;
using Amanah.Api.Tests.Infrastructure;
using Amanah.Api.Tests.Reports;
using Amanah.Contracts.Responses.Admin;
using Microsoft.EntityFrameworkCore;

namespace Amanah.Api.Tests.Admin;

public class AdminAnalyticsTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task Get_analytics_overview_without_auth_returns_unauthorized()
    {
        await using var context = await ReportTestContext.CreateAsync(factory);
        context.Client.DefaultRequestHeaders.Authorization = null;

        var response = await context.Client.GetAsync("/api/v1/admin/analytics/overview");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_analytics_overview_as_user_returns_forbidden()
    {
        await using var context = await ReportTestContext.CreateAsync(factory);

        var response = await context.Client.GetAsync("/api/v1/admin/analytics/overview");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_analytics_overview_as_admin_returns_snapshot_counts()
    {
        await using var context = await ReportTestContext.CreateAsync(factory);

        await BrowseTestHelpers.SeedReportAsync(
            context,
            new BrowseTestHelpers.SeedReportOptions
            {
                Type = ReportType.Lost,
                Status = ReportStatus.Published,
                Title = "Lost wallet overview test",
            });
        await BrowseTestHelpers.SeedReportAsync(
            context,
            new BrowseTestHelpers.SeedReportOptions
            {
                Type = ReportType.Found,
                Status = ReportStatus.Resolved,
                Title = "Found keys overview test",
            });
        await BrowseTestHelpers.SeedReportAsync(
            context,
            new BrowseTestHelpers.SeedReportOptions
            {
                Type = ReportType.Lost,
                Status = ReportStatus.Withdrawn,
                Title = "Expired listing overview test",
            });
        var expiredReport = await context.DbContext.Reports
            .SingleAsync(report => report.Title == "Expired listing overview test");
        expiredReport.WithdrawalReason = ReportLifecycleService.ExpiredWithdrawReason;
        await context.DbContext.SaveChangesAsync();

        await HttpTestHelpers.LoginAsAdminAsync(context);

        var response = await context.Client.GetAsync("/api/v1/admin/analytics/overview");
        var body = await response.Content.ReadFromJsonAsync<AdminAnalyticsOverviewResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(1, body.Impact.LivePublished.Lost);
        Assert.Equal(0, body.Impact.LivePublished.Found);
        Assert.Equal(1, body.Impact.ReunionsResolved.Found);
        Assert.Equal(0, body.Impact.ReunionsResolved.Lost);
        Assert.Equal(3, body.Reports.All);
        Assert.Equal(2, body.Reports.AllByType.Lost);
        Assert.Equal(1, body.Reports.AllByType.Found);
        Assert.Equal(1, body.Reports.Withdrawn);
        Assert.Equal(body.Reports.PendingReview, body.Queues.ModerationPending);
    }
}
