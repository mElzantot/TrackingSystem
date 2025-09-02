﻿namespace NanoHealthSuite.TrackingSystem;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public Guid RoleId { get; set; }
    public string RoleName { get; set; }

}