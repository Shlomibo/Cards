using Moq;

namespace Deck.UnitTests.CardDeckTests;

public abstract class CardDeckTestsBase
{
    protected static IEnumerable<RandomCard> CreateCards(int count)
    {
        HashSet<RandomCard> cards = [];

        while (count > 0)
        {
            RandomCard card = new();

            if (cards.Add(card))
            {
                count--;
                yield return card;
            }
        }
    }

    private protected static TestData CreateTestData(
        IEnumerable<RandomCard>? cards = null)
    {
        RandomCard.Random = new Random(TestContext.CurrentContext.Random.Next());

        Mock<ReaderWriterLockSlim> @lock = new(MockBehavior.Loose)
        {
            DefaultValueProvider = InstanceDefaultValueProvider.Create(new ReaderWriterLockSlim()),
        };
        Mock<Random> random = new(MockBehavior.Loose)
        {
            DefaultValueProvider = InstanceDefaultValueProvider.Create(RandomCard.Random),
        };

        CardsDeck<RandomCard> testSubject = new(cards ?? [], random.Object, @lock.Object);

        return new TestData(testSubject, @lock);
    }

    private protected record TestData(
        CardsDeck<RandomCard> TestSubject,
        Mock<ReaderWriterLockSlim> Lock
    ) : IDisposable
    {
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Lock.Object.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TestData()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
