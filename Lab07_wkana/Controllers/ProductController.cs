using Lab07_wkana.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Lab07_wkana.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductDto product)
    {
        return Ok(new { message = "Producto creado exitosamente" });
    }
}