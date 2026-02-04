using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("SqlServer")));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        var mongoConn = config.GetConnectionString("MongoDb");
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
        services.AddScoped<IReadModelService, MongoReadModelService>();
        services.AddScoped<IOrderRepository, Infrastructure.Persistence.Repositories.OrderRepository>();

        return services;
    }
}
