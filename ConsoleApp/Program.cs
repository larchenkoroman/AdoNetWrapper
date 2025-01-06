using AdoNetWrapper;


string cnn = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True";

Repository repo = new(cnn);
repo.ExecuteNonQuery("update g set Amount = 888 from dbo.Goods as g where g.ID = @ID", new { ID = 2 });
repo.ExecuteNonQuery("update g set Name = @Name from dbo.Goods as g where g.ID = @ID", new { ID = 3 , Name = "Новый товар 2"});

List<Product> list = repo.ExequteQuery<Product>("SELECT ID,Name,Amount FROM dbo.Goods");



foreach (var item in list)
    Console.WriteLine(item.ToString());