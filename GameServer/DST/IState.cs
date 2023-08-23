namespace GameServer.DST
{
	public interface IState<out TShared, out TPlayer>
	{
		TShared SharedState { get; }
		TPlayer PlayerState { get; }
	}
}
