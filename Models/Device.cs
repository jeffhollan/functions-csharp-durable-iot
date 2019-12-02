using Newtonsoft.Json;

public class Device 
{
    public Device(LightToggleRequest req)
    {
        this.Id = req.Id.ToString();
        this.Status = req.Toggle;
        this.DeviceName = req.Id.ToString();
        DeviceType = "Lights";
    }

    public Device(ConnectRoomRequest req)
    {
        this.Id = req.Id.ToString();
        this.Status = req.Status;
        this.DeviceName = req.Id.ToString();
        DeviceType = "ConferencePhone";
    }

    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("deviceName")]
    public string DeviceName { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }

    public string DeviceType { get; set; }
}