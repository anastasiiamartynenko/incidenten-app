using Incidenten.Domain;
using Incidenten.Shared.DTO.User;
using Refit;

namespace Incidenten.Shared.Api;

public interface IUserApi
{
    [Post("/user/sign-up")]
    Task<SignUpResponse> SignUp([Body] SignUpRequest user);
    
    [Post("/user/log-in")]
    Task<LogInResponse> LogIn([Body] LogInRequest user);

    [Get("/user/me")]
    Task<User> GetMe();
}