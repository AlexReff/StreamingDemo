namespace StreamingDemo.Data
{
    public class IntervalFuncCaller<T> : IDisposable
    {
        private readonly Timer _timer;
        private readonly object _lock = new object();
        private bool _isFetching = false;
        private bool _isRunning = false;

        private readonly TimeSpan _interval;

        private readonly Func<T, Task>? _callback;
        private readonly Func<Task<T>> _action;

        public IntervalFuncCaller(Func<Task<T>> action, TimeSpan interval, Func<T, Task>? callback = null)
        {
            _action = action;
            _callback = callback;
            _interval = interval;

            _timer = new Timer(async (state) => await RunLoop(), null, Timeout.InfiniteTimeSpan, _interval);
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    return;
                }

                _isRunning = true;
            }

            _timer.Change(TimeSpan.Zero, _interval);
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    return;
                }

                _isRunning = false;
            }

            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private async Task RunLoop()
        {
            if (!_isRunning)
            {
                return;
            }

            lock (_lock)
            {
                if (_isFetching)
                {
                    return;
                }

                _isFetching = true;
            }

            try
            {
                var result = await _action();
                if (_callback != null)
                {
                    await _callback(result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                lock (_lock)
                {
                    _isFetching = false;
                }
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
