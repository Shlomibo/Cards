namespace ShitheadServer.Server.DST
{
	public sealed record ShitheadInitOptions(int PlayersCount)
	{
		public ShitheadInitOptions()
			: this(0)
		{
		}
	}
}
