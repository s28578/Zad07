using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Components.Sections;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    public async Task<int> FulfillOrder(ProductWarehouse productWarehouse)
    {
        using SqlConnection con =
            new SqlConnection("Server=db-mssql;Database=2019SBD;Integrated Security=True;TrustServerCertificate=True");
        using SqlCommand com = new SqlCommand();
        com.Connection = con;

        await con.OpenAsync();
        DbTransaction tran = await con.BeginTransactionAsync();
        com.Transaction = (SqlTransaction)tran;

        try
        {
            
            com.CommandText = "SELECT COUNT(*) from Product where IdProduct = @IdProduct";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            int countOfProduct = (int)com.ExecuteScalar();
            
            if (countOfProduct < 1)
            {
                return -1;
            }

            com.Parameters.Clear();
            com.CommandText = "SELECT COUNT(*) from Warehouse where IdWarehouse = @IdWarehouse";
            com.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            int countOfWarehouses = (int) com.ExecuteScalar();
            if (countOfWarehouses < 1)
            {
                return -1;
            }
            

            com.Parameters.Clear();
            com.CommandText = "SELECT COUNT(*) from Order1 where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int countOfOrders = (int)com.ExecuteScalar();
            if (countOfOrders < 1)
            {
                return -1;
            }

            
            
            com.Parameters.Clear();
            com.CommandText =
                "SELECT IdOrder from Order1 where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int idOrder = (int)com.ExecuteScalar();
            
            
            com.Parameters.Clear();
            com.CommandText = "SELECT COUNT(*) from Product_Warehouse where IdOrder = @IdOrder";
            com.Parameters.AddWithValue("@IdOrder", idOrder);
            int countOfProductsWithOrder = (int)com.ExecuteScalar();
            if (countOfProductsWithOrder > 0)
            {
                return -1;
            }

            
            
            DateTime dateTime = DateTime.Now;
            com.Parameters.Clear();
            com.CommandText = "UPDATE Order1 SET FulfilledAt = @DateTime";
            com.Parameters.AddWithValue("@DateTime", dateTime);

            
            
            com.Parameters.Clear();
            com.CommandText =
                "SELECT Amount from Order1 where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int amount = (int)com.ExecuteScalar();
            

            
            com.CommandText = "SELECT Price from Product where IdProduct = @IdProduct";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            double price;
            using (var dr = await com.ExecuteReaderAsync())
            {
                await dr.ReadAsync();
                price = Convert.ToDouble(dr["Price"].ToString());
            }
            //double price = Convert.ToDouble(com.ExecuteScalar());
            return 1;
            //double price = Convert.ToDouble(pricestr);
            return 1;
            
            com.Parameters.Clear();
            com.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder,Amount, Price, CreatedAt) " +
                              "VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
            com.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@IdOrder", idOrder);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@Price", String.Format("{0:0.00}", amount*price));
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            
            com.Parameters.Clear();
            com.CommandText = "SELECT IdProductWarehouse FROM Product_Warehouse WHERE " +
                              "IdWarehouse = @IdWarehouse AND IdProduct = @IdProduct AND IdOrder = @IdOrder AND " +
                              "Amount = @Amount AND Price = @Price AND ";
            com.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@IdOrder", idOrder);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@Price", String.Format("{0:0.00}", amount*price));
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int idProductWarehouse = (int)com.ExecuteScalar();

            await tran.CommitAsync();
            return idProductWarehouse;

    
        }
        catch (SqlException exc)
        {
            await tran.RollbackAsync();
        }
        catch (Exception exc)
        {
            await tran.RollbackAsync();
        }

        return -1;


    }
}