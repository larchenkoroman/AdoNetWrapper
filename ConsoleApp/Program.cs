using AdoNetWrapper;
using ConsoleApp;
using System.Data;

//UPDATE с параметрами, параметры передаются как анонимный объект
using (var dbcontext = DBContext.CreateDBContext())
{
    dbcontext.ExecuteNonQuery("update g set Name = @Name from dbo.Goods as g where g.ID = @ID", new { ID = 3, Name = "Новый товар 225" });
}

//Заполнение List<класс-модели-данных>
//Имя свойства в классе отличается от имени поля, используется атрибут.
Console.WriteLine("From List<Product>");
using (var dbcontext = DBContext.CreateDBContext())
{
    List<Product> list = dbcontext.ExequteQuery<Product>("SELECT ID,Name,Amount FROM dbo.Goods");
    foreach (var item in list)
        Console.WriteLine(item.ToString());
}

//Хранимка с OUT параметром
using (var dbcontext = DBContext.CreateDBContext())
{
    dbcontext.CreateCommand("dbo.AddTwoNums",
                        new { n1 = 3, n2 = 5 },
                        CommandType.StoredProcedure
                       );
    dbcontext.AddOutParameter("@r", DbType.Int32);
    dbcontext.ExecuteNonQuery();
    Console.WriteLine("Out parameter:" + dbcontext.GetParameter("@r").Value?.ToString());
}


//Заполнение List<...>
Console.WriteLine("\nFrom List<int>");
List<int> lst = new();
using (var dbcontext = DBContext.CreateDBContext())
{
    dbcontext.ExecuteDR("dbo.AddTwoNums",
                        new { n1 = 3, n2 = 5 },
                        (dr) => lst.Add(dr.GetData<int>("Result"))
                        , CommandType.StoredProcedure
                       );
    Console.WriteLine(lst[0].ToString());
}