namespace BlogApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void InitCors(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("BlazorApp", policyBuilder =>
            {
                policyBuilder
                    .WithOrigins("http://localhost:5234")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}
