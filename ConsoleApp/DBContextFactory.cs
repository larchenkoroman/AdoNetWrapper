using AdoNetWrapper;

namespace ConsoleApp;
public static class DBContextFactory
{
    private static string _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True";

    public static DatabaseContext CreateDBContext()
    {
        return new SqlServerDatabaseContext(_connectionString);
    }
}
