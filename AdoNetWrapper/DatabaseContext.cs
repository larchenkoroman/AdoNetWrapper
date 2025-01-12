namespace AdoNetWrapper;

using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;

public delegate void DataReaderAction(IDataReader dr);

public abstract class DatabaseContext : IDisposable
{
    public DatabaseContext(string connectString)
    {
        ConnectionString = connectString;
        Init();
    }

    public string ConnectionString { get; set; }
    public IDbConnection Connection { get; set; }
    public string ParameterPrefix { get; set; }
    public IDbCommand Command { get; set; }
    public IDataReader DataReader { get; set; }

    protected virtual void Init()
    {
        ParameterPrefix = string.Empty;
    }

    public abstract IDbCommand CreateCommand(string sql, CommandType commandType = CommandType.Text);
    public abstract IDbCommand CreateCommand(string sql, Object paramValues, CommandType commandType = CommandType.Text);
    public abstract IDataParameter CreateParameter(string paramName, object value);
    public abstract void AddOutParameter(string paramName, DbType dbType);
    public abstract IDataParameter GetParameter(string paramName);
    public abstract IDataReader CreateDataReader(CommandBehavior cmdBehavior = CommandBehavior.CloseConnection);

    public virtual void Dispose()
    {
        // Close/Dispose of data reader object
        if (DataReader != null)
        {
            DataReader.Close();
            DataReader.Dispose();
        }

        // Close/Dispose of command object
        if (Command != null)
        {
            if (Command.Connection != null)
            {
                if (Command.Transaction != null)
                {
                    Command.Transaction.Dispose();
                }
                Command.Connection.Close();
                Command.Connection.Dispose();
            }
            Command.Dispose();
        }
    }

    public List<TEntity> ExequteQuery<TEntity>(string sql, Object paramValues = null)
    {
        List<TEntity> ret;
        CreateCommand(sql, paramValues);
        ret = BuildEntityList<TEntity>(CreateDataReader());
        return ret;
    }
    public List<TEntity> BuildEntityList<TEntity>(IDataReader rdr)
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

    public  void ExecuteDR(string sql, DataReaderAction drAction, CommandType commandType = CommandType.Text)
    {
        CreateCommand(sql, commandType);
        IDataReader dr = CreateDataReader();
        while (dr.Read())
        {
            drAction(dr);
        }
    }
    public  void ExecuteDR(string sql, Object paramValues, DataReaderAction drAction, CommandType commandType = CommandType.Text)
    {
        CreateCommand(sql, paramValues, commandType);
        IDataReader dr = CreateDataReader();
        while (dr.Read())
        {
            drAction(dr);
        }
    }
    public int ExecuteNonQuery()
    {
        int ret;
        using SqlServerDatabaseContext dbContext = new(this.ConnectionString);
        Command.Connection.Open();
        ret = Command.ExecuteNonQuery();
        return ret;
    }
    public int ExecuteNonQuery(string sql, CommandType commandType = CommandType.Text)
    {
        int ret;
        IDbCommand cmd;
        using SqlServerDatabaseContext dbContext = new(this.ConnectionString);
        cmd = dbContext.CreateCommand(sql, commandType);
        cmd.Connection.Open();
        ret = cmd.ExecuteNonQuery();
        return ret;
    }
    public int ExecuteNonQuery(string sql, Object paramValues, CommandType commandType = CommandType.Text)
    {
        int ret;
        IDbCommand cmd;
        using SqlServerDatabaseContext dbContext = new(this.ConnectionString);
        cmd = dbContext.CreateCommand(sql, paramValues, commandType);
        cmd.Connection.Open();
        ret = cmd.ExecuteNonQuery();
        return ret;
    }
}