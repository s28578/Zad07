using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]

public class WarehouseController : ControllerBase
{
    private IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    

    [HttpPost]
    public async Task<IResult> FulfillOrder(ProductWarehouse productWarehouse)
    {
        int ret = await _warehouseService.FulfillOrder(productWarehouse);
        //return StatusCode(StatusCodes.Status201Created);
        return ret == -1 ? Results.NotFound() : Results.Ok();
    }
    
}