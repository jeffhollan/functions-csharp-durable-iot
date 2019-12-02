using System;

public class ConnectRoomRequest
{
    public ConnectRoomRequest(Guid id, string status)
    {
        this.Id = id;
        this.Status = status;
    }
    public Guid Id { get; set; }
    public string Status { get; set; }
}