using AdoNetWrapper;


string cnn = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True";

Repository repo = new(cnn);
repo.ExecuteNonQuery("update g set Amount = 888 from dbo.Goods as g where g.ID = @ID", new { ID = 2 });
repo.ExecuteNonQuery("insert into dbo.Goods(Name) values(@Name)", new { Name = "rrrrrr" });

List<Product> list = repo.ExequteQuery<Product>("SELECT ID,Name,Amount FROM dbo.Goods");



foreach (var item in list)
    Console.WriteLine(item.ToString());