using DTOs;

namespace GameServer;

internal sealed partial class Table<
        TGameState,
        TSharedState,
        TPlayerState,
        TGameMove>
{
    public readonly record struct Player : IEquatable<Player>
    {
        public static Player NoPlayer { get; } = default;
        public readonly int Id { get; }
        public readonly string Name { get; }
        public readonly Guid ConnectionId { get; }
        public readonly PlayerState State { get; init; } = PlayerState.Playing;

        public Player(int id, string name, Guid connectionId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Id = id;
            Name = name;
            ConnectionId = connectionId;
        }

        public Table.Player AsDescriptor() =>
                new(Id, Name, PlayerState.Playing);

        public override string ToString() => $"({Id}): {Name}";
    }
}
