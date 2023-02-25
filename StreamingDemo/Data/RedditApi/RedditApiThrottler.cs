namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiThrottler : DelegatingHandler
    {
        private readonly double _secondsPerRequest = 1.0;
        private DateTime _lastRequestTime;

        public RedditApiThrottler()
        {
            _lastRequestTime = DateTime.UtcNow;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await AwaitHttpRequestLimit();
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task AwaitHttpRequestLimit()
        {
            while (true)
            {
                var elapsedSeconds = (DateTime.UtcNow - _lastRequestTime).TotalSeconds;

                if (elapsedSeconds >= _secondsPerRequest)
                {
                    _lastRequestTime = DateTime.UtcNow;
                    break;
                }

                var timeToWait = TimeSpan.FromSeconds(_secondsPerRequest - elapsedSeconds);
                await Task.Delay(timeToWait);
            }
        }
    }
}
