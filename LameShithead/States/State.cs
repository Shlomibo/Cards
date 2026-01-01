using System;

namespace LameShithead.States;

public abstract record State
{
    public async Task<State> NextState(CancellationToken cancellation)
    {
        try
        {
            return await NextStateUnsafe(cancellation);
        }
        catch (Exception ex)
        {
            return new ErrorState(ex);
        }
    }

    protected abstract Task<State> NextStateUnsafe(CancellationToken cancellation);
}
