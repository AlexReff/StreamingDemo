using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace StreamingDemo.Data
{
    /// <summary>
    /// Ensures getDataFunc is only running while there is an active reader of GetData
    /// </summary>
    /// <typeparam name="T">Type of data to return</typeparam>
    public class ManagedActiveChannel<T>
    {
        private readonly IntervalFuncCaller<T> _collectionFunc;
        private readonly Channel<T> _dataChannel;

        private readonly object _lock = new object();
        private static int _countActiveReaders = 0;

        public ManagedActiveChannel(Func<Task<T>> getDataFunc, TimeSpan interval)
        {
            _dataChannel = Channel.CreateUnbounded<T>();
            _collectionFunc = new IntervalFuncCaller<T>(
                getDataFunc,
                interval,
                async data => await _dataChannel.Writer.WriteAsync(data)
            );
        }

        public async IAsyncEnumerable<T> GetData([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                _countActiveReaders++;
                if (_countActiveReaders == 1)
                {
                    _collectionFunc.Start();
                }
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                yield return await _dataChannel.Reader.ReadAsync();
            }

            lock (_lock)
            {
                _countActiveReaders--;
                if (_countActiveReaders == 0)
                {
                    _collectionFunc.Stop();
                }
            }
        }
    }
}
