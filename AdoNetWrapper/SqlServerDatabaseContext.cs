#nullable disable

using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace AdoNetWrapper;

/// <summary>
/// Database context using ADO.NET 
/// for SQL Server Databases
/// </summary>
public class SqlServerDatabaseContext : DatabaseContext
{

    public SqlServerDatabaseContext(string connectString) : base(connectString) { }

    protected override void Init()
    {
        base.Init();

        ParameterPrefix = "@";
    }

    public override SqlConnection CreateConnection(string connectString)
    {
        return new SqlConnection(connectString);
    }
    public override IDbCommand CreateCommand(string sql, Object paramValues = null)
    {
        SqlCommand cmd = CreateCommand(CreateConnection(), sql);
        if (paramValues != null)
        {
            Type typ = paramValues.GetType();
            PropertyInfo[] props = typ.GetProperties();

            foreach (var p in props)
                cmd.Parameters.Add(CreateParameter(p.Name, p.GetValue(paramValues)));
        }
        return cmd;
    }

    public override SqlCommand CreateCommand(IDbConnection cnn, string sql)
    {
        CommandObject = new SqlCommand(sql, (SqlConnection)cnn);
        CommandObject.CommandType = CommandType.Text;

        return (SqlCommand)CommandObject;
    }

    public override SqlParameter CreateParameter(string paramName, object value)
    {
        if (!paramName.StartsWith(ParameterPrefix))
        {
            paramName = ParameterPrefix + paramName;
        }
        return new SqlParameter(paramName, value);
    }

    public override SqlParameter GetParameter(string paramName)
    {
        if (!paramName.StartsWith(ParameterPrefix))
        {
            paramName = ParameterPrefix + paramName;
        }

        return ((SqlCommand)CommandObject).Parameters[paramName];
    }

    public override SqlDataReader CreateDataReader(IDbCommand cmd, CommandBehavior cmdBehavior = CommandBehavior.CloseConnection)
    {
        // Open Connection
        cmd.Connection.Open();
        // Create DataReader
        DataReaderObject = cmd.ExecuteReader(cmdBehavior);

        return (SqlDataReader)DataReaderObject;
    }
}
