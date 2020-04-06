using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotnetCore.Api.HealthCheck
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly IMediator _mediator;

        public DbHealthCheck(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }
        }
    }
}