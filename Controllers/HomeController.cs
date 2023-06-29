// simple controller to return the home page
// return hello world

using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{

    // hello world
    [HttpGet]
    [Route("")]
    public ActionResult<string> HelloWorld()
    {
        return Ok("Hello World");
    }


}