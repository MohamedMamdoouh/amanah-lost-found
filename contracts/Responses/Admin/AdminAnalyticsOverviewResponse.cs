namespace Amanah.Contracts.Responses.Admin;

public sealed class AdminAnalyticsOverviewResponse
{
    public required AdminAnalyticsImpactResponse Impact { get; init; }

    public required AdminAnalyticsReportsResponse Reports { get; init; }

    public required AdminAnalyticsQueuesResponse Queues { get; init; }

    public required AdminAnalyticsAbuseResponse Abuse { get; init; }

    public required AdminAnalyticsUsersResponse Users { get; init; }
}

public sealed class AdminAnalyticsLostFoundCountResponse
{
    public int Lost { get; init; }

    public int Found { get; init; }
}

public sealed class AdminAnalyticsImpactResponse
{
    public required AdminAnalyticsLostFoundCountResponse ReunionsResolved { get; init; }

    public required AdminAnalyticsLostFoundCountResponse LivePublished { get; init; }

    public required AdminAnalyticsLostFoundCountResponse ClaimInProgress { get; init; }
}

public sealed class AdminAnalyticsReportsResponse
{
    public int PendingReview { get; init; }

    public int Rejected { get; init; }

    public int Withdrawn { get; init; }

    public int RemovedByAdmin { get; init; }

    public int All { get; init; }

    public required AdminAnalyticsLostFoundCountResponse AllByType { get; init; }
}

public sealed class AdminAnalyticsQueuesResponse
{
    public int ModerationPending { get; init; }

    public int OpenFlags { get; init; }

    public int PendingClaimsAwaitingReporter { get; init; }
}

public sealed class AdminAnalyticsAbuseResponse
{
    public int TotalSubmitted { get; init; }

    public int Open { get; init; }

    public int ResolvedNoAction { get; init; }

    public int ResolvedTakedown { get; init; }

    public int ResolvedBan { get; init; }
}

public sealed class AdminAnalyticsUsersResponse
{
    public int RegisteredUsers { get; init; }

    public int Banned { get; init; }

    public int Deactivated { get; init; }
}
