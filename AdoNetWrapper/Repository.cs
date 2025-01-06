#nullable disable

using AdoNetWrapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace AdoNetWrapper;

public class Repository
{
    public Repository(string cnn) => ConnectionString = cnn;
    public string ConnectionString { get; set; }
    public List<TEntity> ExequteQuery<TEntity>(string sql, Object paramValues = null)
    {
        List<TEntity> ret;

        using SqlServerDatabaseContext dbContext = new(this.ConnectionString);
        dbContext.CreateCommand(sql, paramValues);

        ret = BuildEntityList<TEntity>(dbContext.CreateDataReader());

        return ret;
    }
    public int ExecuteNonQuery(string sql, Object paramValues = null)
    {
        int ret;
        IDbCommand cmd;
        using SqlServerDatabaseContext dbContext = new(this.ConnectionString);
        cmd = dbContext.CreateCommand(sql, paramValues);
        cmd.Connection.Open();
        ret = cmd.ExecuteNonQuery();
        return ret;
    }

    private List<TEntity> BuildEntityList<TEntity>(IDataReader rdr)
    {
        List<TEntity> ret = new();
        string columnName;

        PropertyInfo[] props = typeof(TEntity).GetProperties();

        while (rdr.Read())
        {
            TEntity entity = Activator.CreateInstance<TEntity>();

            // Loop through columns in data reader
            for (int index = 0; index < rdr.FieldCount; index++)
            {
                columnName = rdr.GetName(index);
                PropertyInfo col = props.FirstOrDefault(col => col.Name == columnName);

                if (col == null)
                {
                    // Is column name in a [Column] attribute?
                    col = props.FirstOrDefault(
                      c => c.GetCustomAttribute
                        <ColumnAttribute>()?.Name == columnName);
                }

                if (col != null)
                {
                    // Get the value from the table
                    var value = rdr[columnName];
                    // Assign value to property if not null
                    if (!value.Equals(DBNull.Value))
                    {
                        col.SetValue(entity, value, null);
                    }
                }
            }
            // Add new entity to the list
            ret.Add(entity);
        }

        return ret;
    }
}