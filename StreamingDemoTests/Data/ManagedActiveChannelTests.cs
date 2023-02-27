using Moq;
using StreamingDemo.Data;

namespace StreamingDemo.Tests.Data
{
    public class ManagedActiveChannelTests
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Mock<Func<Task<int>>> _getDataMock;
        private ManagedActiveChannel<int> _channel;
        private IEnumerable<int> _data;

        public ManagedActiveChannelTests()
        {
            _data = Enumerable.Range(1, 10);
            var enumerator = _data.GetEnumerator();

            _getDataMock = new Mock<Func<Task<int>>>();
            _getDataMock.Setup(x => x()).ReturnsAsync(() => {
                if (!enumerator.MoveNext())
                {
                    enumerator.Reset();
                }
                return enumerator.Current;
            });

            var interval = TimeSpan.FromMilliseconds(500);

            _channel = new ManagedActiveChannel<int>(_getDataMock.Object, interval);
        }

        [Fact]
        public async Task GetData_ShouldReturnData_WhenReadersExist()
        {
            // Arrange
            var items = new List<int>();

            // Act
            await foreach (var item in _channel.GetData(_cts.Token))
            {
                items.Add(item);
                if (items.Count == _data.Count()) break;
            }

            // Assert
            Assert.Equal(_data, items);
        }

        [Fact]
        public async Task GetData_ShouldNotReturnData_WhenNoReaders()
        {
            // Arrange
            var items = new List<int>();

            // Act & Assert
            Assert.False(_channel.CollectionActive);
            
            await Task.Delay(500);

            Assert.False(_channel.CollectionActive);

            await foreach (var item in _channel.GetData(_cts.Token))
            {
                Assert.True(_channel.CollectionActive);
                items.Add(item);
                if (items.Count == _data.Count()) _cts.Cancel();
            }

            await Task.Delay(50);

            Assert.False(_channel.CollectionActive);
        }

        [Fact]
        public async Task GetData_ShouldCancel_WhenCancellationRequested()
        {
            // Arrange
            var items = new List<int>();

            // Act
            _cts.CancelAfter(50);

            await foreach (var item in _channel.GetData(_cts.Token))
            {
                items.Add(item);
            }

            // Assert
            Assert.NotEqual(items.Count, _data.Count());
        }
    }
}
