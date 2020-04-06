using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;

namespace DotnetCore.Data.Example
{
    public class GetUserIdDataRequest : IRequest<Guid>
    {
    }

    public class GetUserIdDataRequestHandler : IRequestHandler<GetUserIdDataRequest, Guid>
    {
        private readonly IMediator _mediator;
        private readonly ConnectionHelper _connectionHelper;

        public GetUserIdDataRequestHandler(IMediator mediator, ConnectionHelper connectionHelper)
        {
            _mediator = mediator;
            _connectionHelper = connectionHelper;
        }

        public async Task<Guid> Handle(GetUserIdDataRequest request, CancellationToken cancellationToken)
        {
            using (var conn = _connectionHelper.GetConnection())
            {
                return await conn.ExecuteScalarAsync<Guid>("SELECT NEWID()");
            }
        }
    }
}