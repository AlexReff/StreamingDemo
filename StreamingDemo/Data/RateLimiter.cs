using System.Threading.Tasks;

namespace StreamingDemo.Data
{
    public class RateLimiter
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly TimeSpan _interval;
        private DateTime _lastExecutedTime;

        public RateLimiter(int maxRequestsPerInterval, TimeSpan interval)
        {
            _semaphore = new SemaphoreSlim(maxRequestsPerInterval);
            _interval = interval;
            _lastExecutedTime = DateTime.MinValue;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            var now = DateTime.UtcNow;

            lock (_semaphore)
            {
                if (now >= _lastExecutedTime + _interval)
                {
                    _semaphore.Release(_semaphore.CurrentCount);
                    _lastExecutedTime = now;
                }
            }

            await _semaphore.WaitAsync();

            try
            {
                return await operation();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
