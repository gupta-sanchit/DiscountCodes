using DiscountCodes.Core.Interfaces;
using DiscountCodes.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountCodes.Core.Registrations;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCoreServices(this IServiceCollection services)
	{
		services.AddScoped<IDiscountCodesService, DiscountCodesService>();
            return services;
	}
}