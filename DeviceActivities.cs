using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

public class DeviceActivities {
    [FunctionName("ToggleLights")]
    public static void ToggleLights(
        [ActivityTrigger] LightToggleRequest lightReq, 
        [CosmosDB("devices", "devices", ConnectionStringSetting = "CosmosDbConnectionString", CreateIfNotExists = true)] out Device light,
        ILogger log)
    {
        log.LogInformation($"Toggling light {lightReq.Id} to {lightReq.Toggle}....");
        light = new Device(lightReq);
    }

    [FunctionName("ConnectRoom")]
    public static void ConnectRoom(
        [ActivityTrigger] ConnectRoomRequest req, 
        [CosmosDB("devices", "devices", ConnectionStringSetting = "CosmosDbConnectionString", CreateIfNotExists = true)] out Device light,
        ILogger log)
    {
        log.LogInformation($"Connecting phone {req.Id}...");
        light = new Device(req);
    }

    [FunctionName("NotifyAttendees")]
    public static void NotifyAttendees(
        [ActivityTrigger] RoomRequest req, 
        ILogger log)
    {
        log.LogInformation($"Notifying attendees for meeting {req.MeetingId}");
    }
}