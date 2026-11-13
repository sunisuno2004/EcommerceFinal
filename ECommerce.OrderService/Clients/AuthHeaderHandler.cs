using System.Net.Http.Headers;

namespace ECommerce.OrderService.Clients
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor
            _httpContextAccessor;

        public AuthHeaderHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor =
                httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage>
            SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
        {
            var authorizationHeader =
                _httpContextAccessor
                    .HttpContext?
                    .Request
                    .Headers["Authorization"]
                    .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(
                    authorizationHeader) &&
                AuthenticationHeaderValue.TryParse(
                    authorizationHeader,
                    out var parsedHeader))
            {
                request.Headers.Authorization =
                    parsedHeader;
            }

            return await base.SendAsync(
                request,
                cancellationToken);
        }
    }
}