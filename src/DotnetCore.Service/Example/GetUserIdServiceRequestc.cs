using System;
using System.Threading;
using System.Threading.Tasks;
using DotnetCore.Data.Example;
using MediatR;

namespace DotnetCore.Service.Example
{
    public class GetUserIdServiceRequest : IRequest<Guid>
    {
    }

    public class GetUserIdServiceRequestHandler : IRequestHandler<GetUserIdServiceRequest, Guid>
    {
        private readonly IMediator _mediator;

        public GetUserIdServiceRequestHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Guid> Handle(GetUserIdServiceRequest request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetUserIdDataRequest(), cancellationToken);
        }
    }
}