using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace DotnetCore.Service.Example
{
    public class GetUserListRequest : IRequest<List<GetUserListRequestResponse>>
    {
    }

    public class GetUserListRequestHandler : IRequestHandler<GetUserListRequest, List<GetUserListRequestResponse>>
    {
        public Task<List<GetUserListRequestResponse>> Handle(GetUserListRequest request, CancellationToken cancellationToken)
        {
            var result = new List<GetUserListRequestResponse>();
            for (int i = 0; i < 10; i++)
            {
                result.Add(new GetUserListRequestResponse
                {
                    UserId = Guid.NewGuid(),
                    UserName = $"user-{i}"
                });
            }
            
            return Task.FromResult(result);
        }
    }


    public class GetUserListRequestResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}