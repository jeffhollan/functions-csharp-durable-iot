using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hollan.Function
{
    public static class RoomOrchestration
    {
        [FunctionName("RoomOrchestration")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log = context.CreateReplaySafeLogger(log);
            log.LogInformation("Someone entered meeting. Starting orchestration...");

            var req = context.GetInput<RoomRequest>();
            log.LogInformation($"Getting devices for room {req.RoomId}");
            
            // Get all devices for the room
            DeviceIds devices = await context.CallActivityAsync<DeviceIds>("GetDevices", req.RoomId);

            var taskList = new List<Task>();

            // Toggle all of the lights
            foreach(var lightId in devices.Lights)
            {
                taskList.Add(
                    context.CallActivityAsync("ToggleLights", new LightToggleRequest(lightId, req.Toggle))
                );
            }

            // Connect room device IN PARALLEL
            taskList.Add(
                context.CallActivityAsync("ConnectRoom", new ConnectRoomRequest(devices.ConferencePhone, req.Toggle))
            );

            // Wait for all lights and room connected
            await Task.WhenAll(taskList);

            // Notify attendees the room is dialed in
            await context.CallActivityAsync("NotifyAttendees", req);

            return "Room Activated";
        }

        [FunctionName("GetDevices")]
        public static DeviceIds GetDevices(
            [ActivityTrigger] string req, 
            [CosmosDB("devices", "rooms", ConnectionStringSetting = "CosmosDbConnectionString", Id = "{req}", PartitionKey = "{req}")] DeviceIds devices,
            ILogger log)
        {
            log.LogInformation($"Getting devices for room {req}");
            return devices;
        }

        [FunctionName("Start")]
        public static async Task QueueStart(
            [QueueTrigger("queue")] RoomRequest roomRequest,
            [DurableClient] IDurableClient starter,
            ILogger log)
        {
            
            string instanceId = await starter.StartNewAsync("RoomOrchestration", null, roomRequest);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }
    }
}