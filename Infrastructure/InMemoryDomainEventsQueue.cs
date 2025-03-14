using Domain.Primitives;
using System.Threading.Channels;

namespace Infrastructure;

public sealed class InMemoryDomainEventsQueue
{
    private readonly Channel<IDomainEvent> _channel =
        Channel.CreateUnbounded<IDomainEvent>();

    public ChannelReader<IDomainEvent> Reader => _channel.Reader;

    public ChannelWriter<IDomainEvent> Writer => _channel.Writer;
}