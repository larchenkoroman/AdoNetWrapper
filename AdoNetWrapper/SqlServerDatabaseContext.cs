#nullable disable

using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace AdoNetWrapper;

public class SqlServerDatabaseContext : DatabaseContext
{

    public SqlServerDatabaseContext(string connectString) : base(connectString) 
    {
        Connection = new SqlConnection(connectString);
    }

    protected override void Init()
    {
        base.Init();

        ParameterPrefix = "@";
    }

    public override IDbCommand CreateCommand(string sql, CommandType commandType = CommandType.Text)
    {
        Command = new SqlCommand(sql, (SqlConnection) Connection);
        Command.CommandType = commandType;
        return Command;
    }
    public override IDbCommand CreateCommand(string sql, Object paramValues, CommandType commandType = CommandType.Text)
    {
        Command = new SqlCommand(sql, (SqlConnection) Connection);
        Command.CommandType = commandType;

        AddParamsFromObject(paramValues);
        return Command;
    }

    public override SqlParameter CreateParameter(string paramName, object value)
    {
        if (!paramName.StartsWith(ParameterPrefix))
        {
            paramName = ParameterPrefix + paramName;
        }
        return new SqlParameter(paramName, value);
    }
    public override void AddOutParameter(string paramName, DbType dbType)
    {
        if (!paramName.StartsWith(ParameterPrefix))
        {
            paramName = ParameterPrefix + paramName;
        }
        Command.Parameters.Add(new SqlParameter() {ParameterName = paramName, DbType = dbType, Direction = ParameterDirection.Output });
    }
    public override SqlParameter GetParameter(string paramName)
    {
        if (!paramName.StartsWith(ParameterPrefix))
        {
            paramName = ParameterPrefix + paramName;
        }
        return ((SqlCommand)Command).Parameters[paramName];
    }

    public override SqlDataReader CreateDataReader(CommandBehavior cmdBehavior = CommandBehavior.CloseConnection)
    {
        // Open Connection
        Command.Connection.Open();
        // Create DataReader
        DataReader = Command.ExecuteReader(cmdBehavior);

        return (SqlDataReader)DataReader;
    }

    private void AddParamsFromObject(Object paramValues)
    {
        if (paramValues != null)
        {
            Type typ = paramValues.GetType();
            PropertyInfo[] props = typ.GetProperties();

            foreach (var p in props)
                Command.Parameters.Add(CreateParameter(p.Name, p.GetValue(paramValues)));
        }
    }
}
