using System.Threading.Tasks;
using Datadog.Trace;
using DotnetCore.Data.Example;
using DotnetCore.Service.Example;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCore.Api.V1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ExampleController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMediator _mediator;

        public ExampleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("users"), HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            // using(var scope = Tracer.Instance.StartActive("web.request"))
            // {
            //     var span = scope.Span;
            //     span.Type = SpanTypes.Web;
            //     span.ResourceName = "users";
            //     span.SetTag(Tags.HttpMethod, "post");
            //
            //     // do some work...
            // }
            var response = await _mediator.Send(new GetUserListRequest());
            foreach (var item in response)
            {
                item.TraceId = Datadog.Trace.CorrelationIdentifier.TraceId;
                item.SpanId = Datadog.Trace.CorrelationIdentifier.SpanId;
            }

            return Ok(response);
        }

        [Route("user-id-from-db"), HttpGet]
        public async Task<IActionResult> GetUserFromDb()
        {
            var response = await _mediator.Send(new GetUserIdServiceRequest());
            return Ok(response);
        }

        [Route("users-with-cache"), HttpGet]
        public async Task<IActionResult> GetUserWithCache()
        {
            var response = await _mediator.Send(new GetUserListWithCacheRequest());
            return Ok(response);
        }
    }
}