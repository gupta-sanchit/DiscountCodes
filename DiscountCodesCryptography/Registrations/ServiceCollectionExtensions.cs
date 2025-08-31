using DiscountCodes.Abstractions.Services;
using DiscountCodesCryptography.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountCodes.Persistence.Registrations;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCryptoServices(this IServiceCollection services)
	{
		services.AddScoped<IDiscountCodeGenerator, DiscountCodeGenerator>();
            return services;
	}
}