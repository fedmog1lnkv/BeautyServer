using Api.Hubs.RecordChat;
using Domain.IntegrationEvents.Record;

namespace Api.Utils;

public static class HubClientMapping
{
    public static readonly IDictionary<Type, Type> EventToHubClientMap = new Dictionary<Type, Type>
    {
        { typeof(RecordAddMessageEvent), typeof(RecordChatHubClient<RecordAddMessageEvent>) },
        { typeof(RecordDeleteMessageEvent), typeof(RecordChatHubClient<RecordDeleteMessageEvent>) },
        { typeof(RecordReadMessageEvent), typeof(RecordChatHubClient<RecordReadMessageEvent>) },
        { typeof(RecordStatusLogEvent), typeof(RecordChatHubClient<RecordStatusLogEvent>) }
    };
}