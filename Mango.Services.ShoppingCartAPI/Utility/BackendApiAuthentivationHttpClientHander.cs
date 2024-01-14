using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.ShoppingCartAPI.Utility
{
    /// <summary>
    /// Xử lý ủy nhiệm để đem token sang request khác
    /// </summary>
    public class BackendApiAuthentivationHttpClientHander: DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;

        public BackendApiAuthentivationHttpClientHander(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _accessor.HttpContext.GetTokenAsync("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
