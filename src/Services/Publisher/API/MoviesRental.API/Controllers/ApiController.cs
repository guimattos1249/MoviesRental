using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MoviesRental.Core;

namespace MoviesRental.API.Controllers;

[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    protected IActionResult CustomResponse(int status, bool success, object? data = null)
    {
        return (status, success) switch
        {
            (404, false) => NotFound(new BaseResponse { StatusCode = status, Success = success, Message = "no elements found" }),
            (400, false) => BadRequest(new BaseResponse { StatusCode = status, Success = success, Message = "bad request" }),
            (500, false) => StatusCode(500, new BaseResponse { StatusCode = status, Success = success, Message = "internal server error" }),
            (201, true) => Created(string.Empty, new BaseResponse { StatusCode = status, Success = success, Message = "resource created successfully", Data = data }),
            (200, true) => Ok(new BaseResponse { StatusCode = status, Success = success, Data = data }),
            _ => StatusCode(500, new BaseResponse { StatusCode = status, Success = success, Message = "internal server error" })
        };
    }
}
