namespace ShitheadServer.Server.DTO
{
	public sealed record ShitheadInitOptions(int PlayersCount)
	{
		public ShitheadInitOptions()
			: this(0)
		{
		}
	}
}
