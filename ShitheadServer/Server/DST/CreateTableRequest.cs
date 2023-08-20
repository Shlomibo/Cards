namespace ShitheadServer.Server.DST
{
	public sealed class CreateTableRequest
	{
		public string TableName { get; set; } = "";
		public string MasterName { get; set; } = "";
	}
}
