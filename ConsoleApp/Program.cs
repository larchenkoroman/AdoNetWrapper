using AdoNetWrapper;
using Microsoft.Data.SqlClient;
using System.Data;

string cnn = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True";

using DatabaseContext dbcontext = new SqlServerDatabaseContext(cnn);
//dbcontext.CreateCommand("dbo.AddTwoNums",
//                        new { n1 = 3, n2 = 5 },
//                        CommandType.StoredProcedure
//                       );
//dbcontext.AddOutParameter("@r", DbType.Int32);
//dbcontext.ExecuteNonQuery();
//Console.WriteLine(dbcontext.GetParameter("@r").Value?.ToString());


//Repository repo = new(cnn);
//repo.ExecuteNonQuery("update g set Amount = 888 from dbo.Goods as g where g.ID = @ID", new { ID = 2 });
//repo.ExecuteNonQuery("update g set Name = @Name from dbo.Goods as g where g.ID = @ID", new { ID = 3 , Name = "Новый товар 2"});

List<Product> list = dbcontext.ExequteQuery<Product>("SELECT ID,Name,Amount FROM dbo.Goods");
foreach (var item in list)
    Console.WriteLine(item.ToString());

//Console.WriteLine("-----");

//using DatabaseContext dbcontext = new SqlServerDatabaseContext(cnn);
//List<int> lst = new();
//dbcontext.ExecuteDR("dbo.AddTwoNums",
//                    new { n1 = 3, n2 = 5}, 
//                    //добавить CommandType
//                    (dr) => lst.Add(dr.GetData<int>("Result"))
//                    ,CommandType.StoredProcedure
//                   );

//Console.WriteLine(lst[0].ToString());