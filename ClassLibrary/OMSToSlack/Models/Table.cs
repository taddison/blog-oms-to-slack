using System.Collections.Generic;

public class Table
{
    public string TableName { get; set; }
    public List<Column> Columns { get; set; }
    public List<List<object>> Rows { get; set; }
}