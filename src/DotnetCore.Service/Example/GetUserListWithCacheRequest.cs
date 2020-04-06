using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotnetCore.Core.Interface;
using MediatR;

namespace DotnetCore.Service.Example
{
    public class GetUserListWithCacheRequest : IRequest<List<GetUserListRequestResponse>>, ICacheable
    {
        public CacheOption CacheOption => CacheOption.Distributed;
        public KeyValuePair<string, TimeSpan> CacheSettings => new KeyValuePair<string, TimeSpan> ("GetUserListWithCacheRequest", TimeSpan.FromMinutes(10));
    }

    public class GetUserListWithCacheRequestHandler : IRequestHandler<GetUserListWithCacheRequest, List<GetUserListRequestResponse>>
    {
        public Task<List<GetUserListRequestResponse>> Handle(GetUserListWithCacheRequest request, CancellationToken cancellationToken)
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
}