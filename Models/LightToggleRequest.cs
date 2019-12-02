using System;

public class LightToggleRequest
{
    public LightToggleRequest(Guid id, string toggle)
    {
        this.Id = id;
        this.Toggle = toggle;
    }
    public Guid Id { get; set; }
    public string Toggle { get; set; }
}