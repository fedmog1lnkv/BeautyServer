using Domain.Repositories.Records;
using Domain.Repositories.Users;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

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
        var tokenString = Context.GetHttpContext()?.Request.Query["token"];

        if (string.IsNullOrEmpty(tokenString) || !Guid.TryParse(recordId, out var recordGuid))
        {
            await Clients.Caller.SendAsync("Error", "Invalid recordId or token");
            Context.Abort();
            return;
        }

        Guid? userGuid = null;
        Guid? staffGuid = null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            var staffId = jwtToken.Claims.FirstOrDefault(c => c.Type == "staff_id")?.Value;

            if (userId != null && Guid.TryParse(userId, out var parsedUserId))
                userGuid = parsedUserId;

            if (staffId != null && Guid.TryParse(staffId, out var parsedStaffId))
                staffGuid = parsedStaffId;
        }
        catch (Exception)
        {
            await Clients.Caller.SendAsync("Error", "Invalid or expired token");
            Context.Abort();
            return;
        }

        var userOrStaffGuid = userGuid ?? staffGuid;
        if (userOrStaffGuid == null)
        {
            await Clients.Caller.SendAsync("Error", "No valid user_id or staff_id found in token");
            Context.Abort();
            return;
        }
        var record = await recordRepository.GetRecordById(recordGuid);
        if (record is null)
        {
            await Clients.Caller.SendAsync("Error", "Record not found");
            Context.Abort();
            return;
        }

        if (record.UserId != userOrStaffGuid && record.StaffId != userOrStaffGuid)
        {
            await Clients.Caller.SendAsync("Error", "There is no access to this record");
            Context.Abort();
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