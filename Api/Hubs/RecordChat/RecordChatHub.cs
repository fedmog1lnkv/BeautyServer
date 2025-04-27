using Domain.Repositories.Records;
using Domain.Repositories.Users;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs.RecordChat;

public class RecordChatHub(IUserReadOnlyRepository userRepository, IRecordReadOnlyRepository recordRepository)
    : Hub
{

    public override async Task OnConnectedAsync()
    {
        await JoinAsync();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var recordId = Context.GetHttpContext()?.Request.Query["recordId"];
        if (Guid.TryParse(recordId, out var recordGuid))
        {
            await LeaveAsync(recordGuid);
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task JoinAsync()
    {
        var recordId = Context.GetHttpContext()?.Request.Query["recordId"];
        var userId = Context.GetHttpContext()?.Request.Query["userId"];

        if (!Guid.TryParse(recordId, out var recordGuid) || !Guid.TryParse(userId, out var userGuid))
        {
            await Clients.Caller.SendAsync("Error", "Invalid recordId or userId");
            return;
        }

        var record = await recordRepository.GetRecordById(recordGuid);
        if (record is null)
        {
            await Clients.Caller.SendAsync("Error", "Record not found");
            return;
        }

        if (record.UserId != userGuid && record.StaffId != userGuid)
        {
            await Clients.Caller.SendAsync("Error", "There is no access to this record");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(recordGuid));
        await Clients.Caller.SendAsync("Connected");
    }

    private async Task LeaveAsync(Guid recordId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(recordId));
    }

    public async Task SendMessageToGroupAsync(Guid recordId, string message)
    {
        await Clients.Group(GetGroupName(recordId)).SendAsync("ReceiveMessage", message);
    }

    private string GetGroupName(Guid recordId)
    {
        return $"Record_{recordId}";
    }
}