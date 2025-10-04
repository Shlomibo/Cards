using System.ComponentModel.DataAnnotations;

namespace ShitheadServer.Server.DTO;

public sealed record ShitheadInitOptions
{
	[Range(2, 4, ErrorMessage = "Players count must be between 2 and 4")]
	public int PlayersCount { get; init; }

	public ShitheadInitOptions()
	{
	}

	public ShitheadInitOptions(int playersCount)
	{
		PlayersCount = playersCount;
	}
}
