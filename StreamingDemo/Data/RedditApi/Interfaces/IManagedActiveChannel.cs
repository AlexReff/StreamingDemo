using System.Runtime.CompilerServices;

namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IManagedActiveChannel<T>
    {
        bool CollectionActive { get; }
        IAsyncEnumerable<T> GetData(CancellationToken cancellationToken);
    }
}
