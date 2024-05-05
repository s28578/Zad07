using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IWarehouseRepository
{
    public Task<int> FulfillOrder(ProductWarehouse pw);
}