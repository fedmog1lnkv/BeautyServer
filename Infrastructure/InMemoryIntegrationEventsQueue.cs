using Domain.Primitives;
using System.Threading.Channels;

namespace Infrastructure;

public sealed class InMemoryIntegrationEventsQueue
{
    private readonly Channel<IIntegrationEvent> _channel =
        Channel.CreateUnbounded<IIntegrationEvent>();

    public ChannelReader<IIntegrationEvent> Reader => _channel.Reader;

    public ChannelWriter<IIntegrationEvent> Writer => _channel.Writer;
}