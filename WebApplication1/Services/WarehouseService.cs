using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<int> FulfillOrder(ProductWarehouse productWarehouse)
    {
        if (productWarehouse.Amount <= 0)
        {
            throw new Exception("Wrong amount.");
        }

        return await _warehouseRepository.FulfillOrder(productWarehouse);
    }
}