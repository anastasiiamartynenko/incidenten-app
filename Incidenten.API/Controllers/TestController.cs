using Microsoft.AspNetCore.Mvc;

namespace Incidenten.API.Controllers;

// Needed for test purposes only.
// TODO: remove the whole testing logic within the next story.
[ApiController]
[Route("[controller]")]
public class TestController : Controller
{
    public class TestRequest
    {
        public string TestString { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }

    [HttpGet]
    public ActionResult GetTest()
    {
        return Ok("The GET test endpoint successfully reached.");
    }

    [HttpPost]
    public ActionResult PostTest([FromBody] TestRequest request)
    {
        return Ok(new TestResponse
        {
            Result = $"{request.TestString} OK"
        });
    }
}