using Refit;

namespace Incidenten.Shared.Api;

public interface ITestApi
{
    [Post("/test")]
    Task<TestResponse> SendTest([Body] TestRequest request);
}

public class TestRequest
{
    public string testString { get; set; } = string.Empty;
}

public class TestResponse
{
    public string Result { get; set; } = string.Empty;
}