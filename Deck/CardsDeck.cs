using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Nito.Disposables;

namespace Deck;

/// <inheritdoc cref="IDeck{TCard}"/>
public class CardsDeck<TCard> : IDeck<TCard>
    where TCard : struct
{
    #region Fields

    private readonly List<TCard> _cards;
    private readonly Random _rand = new();
    private readonly ReaderWriterLockSlim _lock = new();
    #endregion

    #region Properties

    /// <inheritdoc/>
    public TCard this[int index]
    {
        get
        {
            using (ReadLock())
            {
                return _cards[ReversedIndex(index)];
            }
        }

        set
        {
            using (WriteLock())
            {
                _cards[ReversedIndex(index)] = value;
            }
        }
    }

    /// <inheritdoc/>
    public TCard? Top
    {
        get
        {
            using (ReadLock())
            {
                return Count == 0
                    ? null
                    : _cards[^1];
            }
        }
    }

    /// <inheritdoc/>
    public int Count
    {
        get
        {
            using (ReadLock())
            {
                return _cards.Count;
            }
        }
    }

    bool ICollection<TCard>.IsReadOnly => false;
    #endregion

    #region Ctors

    /// <summary>
    /// Creates an empty deck.
    /// </summary>
    public CardsDeck()
    {
        _cards = [];
    }

    /// <summary>
    /// Creates a deck with the given cards.
    /// </summary>
    /// <param name="cards">The cards to initialize the deck with.</param>
    /// <remarks>
    /// If <paramref name="cards"/> is another <see cref="CardsDeck{TCard}"/>, it is cloned.
    /// </remarks>
    public CardsDeck(IEnumerable<TCard> cards)
    {
        _cards = cards is CardsDeck<TCard> deck
            ? [.. deck._cards]
            : [.. cards.Reverse()];
    }
    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Add(TCard card)
    {
        using (WriteLock())
        {
            _cards.Insert(0, card);
        }
    }

    /// <inheritdoc/>
    public void Add(params IEnumerable<TCard> cards)
    {
        using (WriteLock())
        {
            _cards.InsertRange(0, cards);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        using (WriteLock())
        {
            _cards.Clear();
        }
    }

    /// <inheritdoc/>
    public bool Contains(TCard card)
    {
        using (ReadLock())
        {
            return _cards.Contains(card);
        }
    }

    /// <inheritdoc/>
    public void CopyTo(TCard[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);

        using (ReadLock())
        {
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException(
                    "Destination array was not long enough. " +
                        "Check the destination index, length, and the array's lower bounds.",
                    nameof(array)
                );
            }

            for (int i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerator<TCard> GetEnumerator()
    {
        using (ReadLock())
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }
    }

    /// <inheritdoc/>
    public int IndexOf(TCard card)
    {
        using (ReadLock())
        {
            return ReversedIndex(_cards.LastIndexOf(card));
        }
    }

    /// <inheritdoc/>
    public void Insert(int index, TCard card)
    {
        using (WriteLock())
        {
            _cards.Insert(ReversedIndex(index), card);
        }
    }

    /// <inheritdoc/>
    public bool TryPop([MaybeNullWhen(false)] out TCard card)
    {
        using (UpgradeableReadLock())
        {
            if (Count == 0)
            {
                card = default;
                return false;
            }

            card = this[Count - 1];

            using (WriteLock())
            {
                _cards.RemoveAt(_cards.Count - 1);
            }

            return true;
        }
    }

    // We are listing the cards in reverse, so pushing "adds" and adding "pushes"
    /// <inheritdoc/>
    public void Push(TCard card)
    {
        using (WriteLock())
        {
            _cards.Add(card);
        }
    }

    /// <inheritdoc/>
    public void Push(params IEnumerable<TCard> cards)
    {
        using (WriteLock())
        {
            foreach (var card in cards)
            {
                _cards.Add(card);
            }
        }
    }

    /// <inheritdoc/>
    public bool Remove(TCard card)
    {
        using (UpgradeableReadLock())
        {
            int lastIndex = _cards.LastIndexOf(card);

            if (lastIndex == -1)
            {
                return false;
            }

            using (WriteLock())
            {
                _cards.RemoveAt(lastIndex);
            }
            return true;
        }
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        using (WriteLock())
        {
            _cards.RemoveAt(ReversedIndex(index));
        }
    }

    /// <inheritdoc/>
    public void Shuffle()
    {
        if (_cards.Count <= 1)
        {
            return;
        }

        using (WriteLock())
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                int j = _rand.Next(0, _cards.Count - 1);

                if (j >= i)
                {
                    j++;
                }

                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc/>
    public IReadonlyDeck<TCard> AsReadonly() => new ReadonlyDeck<TCard>(this);

    private int ReversedIndex(int index) =>
        Math.Max(Count - index - 1, 0);
    #endregion

    private IDisposable ReadLock()
    {
        _lock.EnterReadLock();
        return new Disposable(() => _lock.ExitReadLock());
    }

    private IDisposable WriteLock()
    {
        _lock.EnterWriteLock();
        return new Disposable(() => _lock.ExitWriteLock());
    }

    private IDisposable UpgradeableReadLock()
    {
        _lock.EnterUpgradeableReadLock();
        return new Disposable(() => _lock.ExitUpgradeableReadLock());
    }
}

/// <inheritdoc cref="IReadonlyDeck{TCard}"/>
public class ReadonlyDeck<TCard> : IReadonlyDeck<TCard>
    where TCard : struct
{
    private readonly IDeck<TCard> _deck;

    /// <summary>
    /// Creates a readonly wrapper around the given deck.
    /// </summary>
    /// <param name="deck">The cards deck to wrap.</param>
    public ReadonlyDeck(IDeck<TCard> deck)
    {
        _deck = deck;
    }

    /// <inheritdoc/>
    public TCard this[int index] => _deck[index];

    /// <inheritdoc/>
    public TCard? Top => _deck.Top;

    /// <inheritdoc/>
    public int Count => _deck.Count;

    /// <inheritdoc/>
    public IEnumerator<TCard> GetEnumerator() => _deck.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
