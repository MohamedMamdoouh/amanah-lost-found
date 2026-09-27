using Amanah.Api.Services.Admin;

namespace Amanah.Api.Extensions;

public static class CategoryAdminServiceExtensions
{
    public static IServiceCollection AddCategoryAdminServices(this IServiceCollection services)
    {
        services.AddScoped<CategoryAdminService>();
        services.AddScoped<AdminUserLookupService>();
        services.AddScoped<AdminAnalyticsService>();

        return services;
    }
}
