public class InfoSchemaTables
{
	private string pv_TABLE_NAME;
	private string pv_TABLE_SCHEMA;

	public string? TABLE_CATALOG { get; set; }
	public string TABLE_SCHEMA { get { return pv_TABLE_SCHEMA; } set { pv_TABLE_SCHEMA = value; } } 
	public string TABLE_NAME { get { return pv_TABLE_NAME; } set { pv_TABLE_NAME = value; } } 
	public string? TABLE_TYPE { get; set; }
	public string? ENGINE { get; set; }
	public int? VERSION { get; set; }
	public string? ROW_FORMAT { get; set; }
	public Int64? TABLE_ROWS { get; set; }
	public Int64? AVG_ROW_LENGTH { get; set; }
	public Int64? DATA_LENGTH { get; set; }
	public Int64? MAX_DATA_LENGTH { get; set; }
	public Int64? INDEX_LENGTH { get; set; }
	public Int64? DATA_FREE { get; set; }
	public Int64? AUTO_INCREMENT { get; set; }
	public DateTime? CREATE_TIME { get; set; }
	public DateTime? UPDATE_TIME { get; set; }
	public DateTime? CHECK_TIME { get; set; }
	public string? TABLE_COLLATION { get; set; }
	public UInt16? CHECKSUM { get; set; }
	public string? CREATE_OPTIONS { get; set; }
	public string? TABLE_COMMENT { get; set; }
}


public class InfoSchemaColumns
{
	public string? TABLE_CATALOG { get; set; }
	public string? TABLE_SCHEMA { get; set; }
	public string? TABLE_NAME { get; set; }
	public string? COLUMN_NAME { get; set; }
	public int? ORDINAL_POSITION { get; set; }
	public string? COLUMN_DEFAULT { get; set; }
	public string? IS_NULLABLE { get; set; }
	public string? DATA_TYPE { get; set; }
	public Int64? CHARACTER_MAXIMUM_LENGTH { get; set; }
	public Int64? CHARACTER_OCTET_LENGTH { get; set; }
	public int? NUMERIC_PRECISION { get; set; }
	public int? NUMERIC_SCALE { get; set; }
	public int? DATETIME_PRECISION { get; set; }
	public string? CHARACTER_SET_NAME { get; set; }
	public string? COLLATION_NAME { get; set; }
	public string? COLUMN_TYPE { get; set; }
	public string? COLUMN_KEY { get; set; }
	public string? EXTRA { get; set; }
	public string? PRIVILEGES { get; set; }
	public string? COLUMN_COMMENT { get; set; }
	public string? GENERATION_EXPRESSION { get; set; }
	public int? SRS_ID { get; set; }
}


public class BlazorGOSQLCodeTemplateGenerator
{
	private List<InfoSchemaColumns> _infoSchemaColumns;

	public List<InfoSchemaColumns> InfoSchemaColumns 
	{ get 
		{ return _infoSchemaColumns; } 
		set 
		{ if (value != null && value.Count() > 0) _infoSchemaColumns = value; }
	}

	public BlazorGOSQLCodeTemplateGenerator(List<InfoSchemaColumns> infoSchemaColumns)
    {
		_infoSchemaColumns = infoSchemaColumns;
    }

	public String GoLangStructCode()
    {
		var output = "type " + InfoSchemaColumns[0].TABLE_NAME + " struct {\n";
		foreach (InfoSchemaColumns column in _infoSchemaColumns) {
			// NUMERIC_PRECISION        int    `json:"NUMERIC_PRECISION"`
			var convertedType = ConvertMySQLTypeToGoLangType(column.DATA_TYPE, column.COLUMN_TYPE.Contains("unsigned"));
			output += $"\t{column.COLUMN_NAME} {convertedType} `json:\"{column.COLUMN_NAME}\"` \n";
		}

		output += "}\n";
		return output;
    }

	public String CSharpClassCode()
	{
		var output = "public class " + InfoSchemaColumns[0].TABLE_NAME + "\n{\n";
		foreach (InfoSchemaColumns column in _infoSchemaColumns)
		{
			// NUMERIC_PRECISION        int    `json:"NUMERIC_PRECISION"`
			var convertedType = ConvertMySQLTypeToCSharpType(column.DATA_TYPE, column.COLUMN_TYPE.Contains("unsigned"));
			output += $"\tpublic  {convertedType}? {column.COLUMN_NAME} " + "{ get; set; } \n";
		}

		output += "}\n";
		return output;
	}

	private String ConvertMySQLTypeToGoLangType(string type, bool unsigned = false)
    {
		type = type.Trim().ToLower().Split("(")[0];

		if (type == "bigint" && unsigned == false) type = "*int64";
		else if (type == "bigint" && unsigned == true) type = "*uint64";
		else if (type == "binary" && unsigned == false) type = "string";
		else if (type == "bit" && unsigned == false) type = "*int";
		else if (type == "blob" && unsigned == false) type = "string";
		else if (type == "bool" && unsigned == false) type = "bool";
		else if (type == "char" && unsigned == false) type = "string";
		else if (type == "date" && unsigned == false) type = "time.Time";
		else if (type == "datetime" && unsigned == false) type = "time.Time";
		else if (type == "decimal" && unsigned == false) type = "float64";
		else if (type == "decimal" && unsigned == true) type = "float64";
		else if (type == "double" && unsigned == false) type = "float64";
		else if (type == "double" && unsigned == true) type = "float64";
		else if (type == "enum" && unsigned == false) type = "string";
		else if (type == "float" && unsigned == false) type = "float32";
		else if (type == "int" && unsigned == false) type = "int32";
		else if (type == "int" && unsigned == true) type = "uint32";
		else if (type == "longblob" && unsigned == false) type = "string";
		else if (type == "longtext" && unsigned == false) type = "string";
		else if (type == "mediumblob" && unsigned == false) type = "string";
		else if (type == "mediumint" && unsigned == false) type = "int32";
		else if (type == "mediumint" && unsigned == true) type = "uint32";
		else if (type == "mediumtext" && unsigned == false) type = "string";
		else if (type == "numeric" && unsigned == false) type = "float64";
		else if (type == "real" && unsigned == false) type = "float64";
		else if (type == "set" && unsigned == false) type = "string";
		else if (type == "smallint" && unsigned == false) type = "int16";
		else if (type == "smallint" && unsigned == true) type = "uint16";
		else if (type == "text" && unsigned == false) type = "string";
		else if (type == "time" && unsigned == false) type = "time.Time";
		else if (type == "timestamp" && unsigned == false) type = "time.Time";
		else if (type == "tinyblob" && unsigned == false) type = "string";
		else if (type == "tinyint" && unsigned == false) type = "int8";
		else if (type == "tinyint" && unsigned == true) type = "uint8";
		else if (type == "tinytext" && unsigned == false) type = "string";
		else if (type == "varbinary" && unsigned == false) type = "string";
		else if (type == "varchar" && unsigned == false) type = "string";
		else if (type == "year" && unsigned == false) type = "time.Time";
		else if (type == "json" && unsigned == false) type = "string";

		return type;
	}


	private String ConvertMySQLTypeToCSharpType(string type, bool unsigned = false)
	{
		type = type.Trim().ToLower().Split("(")[0];

		if (type == "bigint" && unsigned == false) type = "Int64";
		else if (type == "bigint" && unsigned == true) type = "UInt64";
		else if (type == "binary" && unsigned == false) type = "String";
		else if (type == "bit" && unsigned == false) type = "Boolean";
		else if (type == "blob" && unsigned == false) type = "String";
		else if (type == "bool" && unsigned == false) type = "Boolean";
		else if (type == "char" && unsigned == false) type = "String";
		else if (type == "date" && unsigned == false) type = "DateTime";
		else if (type == "datetime" && unsigned == false) type = "DateTime";
		else if (type == "decimal" && unsigned == false) type = "Double";
		else if (type == "decimal" && unsigned == true) type = "Double";
		else if (type == "double" && unsigned == false) type = "Double";
		else if (type == "double" && unsigned == true) type = "Double";
		else if (type == "enum" && unsigned == false) type = "String";
		else if (type == "float" && unsigned == false) type = "Single";
		else if (type == "int" && unsigned == false) type = "Int32";
		else if (type == "int" && unsigned == true) type = "UInt32";
		else if (type == "longblob" && unsigned == false) type = "String";
		else if (type == "longtext" && unsigned == false) type = "String";
		else if (type == "mediumblob" && unsigned == false) type = "String";
		else if (type == "mediumint" && unsigned == false) type = "Int32";
		else if (type == "mediumint" && unsigned == true) type = "UInt32";
		else if (type == "mediumtext" && unsigned == false) type = "String";
		else if (type == "numeric" && unsigned == false) type = "Double";
		else if (type == "real" && unsigned == false) type = "Double";
		else if (type == "set" && unsigned == false) type = "String";
		else if (type == "smallint" && unsigned == false) type = "Int16";
		else if (type == "smallint" && unsigned == true) type = "UInt16";
		else if (type == "text" && unsigned == false) type = "String";
		else if (type == "time" && unsigned == false) type = "DateTime";
		else if (type == "timestamp" && unsigned == false) type = "DateTime";
		else if (type == "tinyblob" && unsigned == false) type = "String";
		else if (type == "tinyint" && unsigned == false) type = "UInt8";
		else if (type == "tinyint" && unsigned == true) type = "UInt8";
		else if (type == "tinytext" && unsigned == false) type = "String";
		else if (type == "varbinary" && unsigned == false) type = "String";
		else if (type == "varchar" && unsigned == false) type = "String";
		else if (type == "year" && unsigned == false) type = "DateTime";
		else if (type == "json" && unsigned == false) type = "String";

		return type;
	}

}


