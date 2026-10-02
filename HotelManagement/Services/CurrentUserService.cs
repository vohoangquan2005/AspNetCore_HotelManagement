using System.Security.Claims;
namespace HotelManagement.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?
                                                      .User?
                                                      .FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                    return null;
                if (int.TryParse(userIdClaim.Value, out var userId))
                    return userId;
                return null;
            }
        }
    }
}
