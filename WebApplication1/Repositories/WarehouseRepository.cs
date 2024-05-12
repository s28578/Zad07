using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Components.Sections;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    public async Task<int> FulfillOrder(ProductWarehouse productWarehouse)
    {
        //GDZIE UZYWAMY AWAIT
        //PRZED: 
        //1 -> sqlConnection 
        //2 -> sqlConnection.OpenAsync()
        //3 -> SqlCommand
        //4 -> sqlCommand.ExecuteReaderAsync()
        //5 -> sqlDataReader.ReadAsync()
        //6 -> każda metoda async
        using  SqlConnection con =
            new SqlConnection("Server=db-mssql;Database=2019SBD;Integrated Security=True;TrustServerCertificate=True");
        using SqlCommand com = new SqlCommand();
        com.Connection = con;

        await con.OpenAsync();
        DbTransaction tran = await con.BeginTransactionAsync();
        com.Transaction = (SqlTransaction)tran;

        try
        {
            Console.WriteLine("Does this work?");
            com.CommandText = "SELECT COUNT(*) from Product where IdProduct = @IdProduct";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            int countOfProduct = (int) await com.ExecuteScalarAsync();
            
            if (countOfProduct < 1)
            {
                Console.WriteLine("Count: " + countOfProduct);
                // return -1;
                throw new InvalidOperationException("Order either does not exist or is already fulfilled.");
            }

            com.Parameters.Clear();
            com.CommandText = "SELECT COUNT(*) from Warehouse where IdWarehouse = @IdWarehouse";
            com.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            int countOfWarehouses = (int) await com.ExecuteScalarAsync();
            if (countOfWarehouses < 1)
            {
                Console.WriteLine("Here1");
                return -1;
            }
            

            com.Parameters.Clear();
            com.CommandText = "SELECT COUNT(*) from Order1 where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int countOfOrders = (int) await com.ExecuteScalarAsync();
            if (countOfOrders < 1)
            {
                Console.WriteLine("Here2");

                return -1;
            }

            
            
            com.Parameters.Clear();
            com.CommandText =
                "SELECT IdOrder from Order1 where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int idOrder = (int)await com.ExecuteScalarAsync();
            if (idOrder == 1)
            {
                Console.WriteLine("Here3");

            }
            
            
            com.Parameters.Clear();
            com.CommandText = "SELECT COUNT(*) from Product_Warehouse where IdOrder = @IdOrder";
            com.Parameters.AddWithValue("@IdOrder", idOrder);
            int countOfProductsWithOrder = (int) await com.ExecuteScalarAsync();
            if (countOfProductsWithOrder > 0)
            {
                Console.WriteLine("Here4");

                return -1;
            }

            
            
            DateTime dateTime = DateTime.Now;
            com.Parameters.Clear();
            com.CommandText = "UPDATE Order1 SET FulfilledAt = @DateTime where IdProduct = @IdProduct";
            com.Parameters.AddWithValue("@DateTime", dateTime);
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            // await com.ExecuteScalarAsync();
            var affectedRows1 = await com.ExecuteNonQueryAsync();
            if (affectedRows1 == 0)
            {
                Console.WriteLine("here4.5");
            }
            
            
            
            com.Parameters.Clear();
            com.CommandText =
                "SELECT Amount from Order1 where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int amount = (int) await com.ExecuteScalarAsync();
            if (amount == 125)
            {
                Console.WriteLine("Here7");

            }
            

            
            
            // com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            
            // var affectedRows = await com.ExecuteNonQueryAsync();
            // if (affectedRows == 0)
            // {
            //     Console.WriteLine("here4.5");
            // }
            // double price = Convert.ToDouble(await com.ExecuteScalarAsync());
            // Console.WriteLine("Price: " + price);
            // return 1;
            //double price = Convert.ToDouble(pricestr);
            // return 1;
            
            com.Parameters.Clear();
            com.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder,Amount, Price, CreatedAt) " +
                              "VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount," +
                              " (select price * amount from product JOIN Order1  ON product.IdProduct = Order1.IdProduct WHERE IdOrder = @IdOrder), @CreatedAt);SELECT SCOPE_IDENTITY();";
            com.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@IdOrder", idOrder);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            //com.ExecuteScalar();
            var affectedRows2 = await com.ExecuteNonQueryAsync();
            if (affectedRows2 > 0)
            {
                Console.WriteLine("here8");
            }
            
            com.Parameters.Clear();
            com.CommandText = "SELECT IdProductWarehouse FROM Product_Warehouse WHERE " +
                              "IdWarehouse = @IdWarehouse AND IdProduct = @IdProduct AND IdOrder = @IdOrder AND " +
                              "Amount = @Amount AND Price = (select price * amount from product JOIN Order1  ON product.IdProduct = Order1.IdProduct WHERE IdOrder = @IdOrder) AND CreatedAt = @CreatedAt";
            com.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            com.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            com.Parameters.AddWithValue("@IdOrder", idOrder);
            com.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            com.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            int idProductWarehouse = (int) await com.ExecuteScalarAsync();
            Console.WriteLine("idProductWarehouse: " + idProductWarehouse);
        
            await tran.CommitAsync();
            return idProductWarehouse;

    
        }
        catch (SqlException exc)
        {
            Console.WriteLine("Here9");
            await tran.RollbackAsync();
        }
        // catch (Exception exc)
        // {
        //     
        //     Console.WriteLine("Here10");
        //
        //     await tran.RollbackAsync();
        // }

        return -1;


    }
}