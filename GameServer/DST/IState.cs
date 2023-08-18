namespace GameServer.DST
{
	public interface IStateUpdate<out TShared, out TPlayer>
	{
		TShared SharedState { get; }
		TPlayer PlayerState { get; }
	}
}
