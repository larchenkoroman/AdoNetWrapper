﻿using AdoNetWrapper;


string cnn = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True";

Repository repo = new(cnn);
repo.ExecuteNonQuery("update g set Amount = 555 from dbo.Goods as g where g.ID = @ID", new { ID = 2 });

List<Product> list = repo.ExequteQuery<Product>("SELECT ID,Name,Amount FROM dbo.Goods WHERE ID = @ID", new { ID = 2 });



foreach (var item in list)
    Console.WriteLine(item.ToString());