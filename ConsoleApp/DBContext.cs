using AdoNetWrapper;

namespace ConsoleApp;
public static class DBContext
{
    private static string _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True";

    public static DatabaseContext CreateDBContext()
    {
        return new SqlServerDatabaseContext(_connectionString);
    }
}
