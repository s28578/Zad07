using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IWarehouseService
{
    public Task<int> FulfillOrder(ProductWarehouse productWarehouse);
}