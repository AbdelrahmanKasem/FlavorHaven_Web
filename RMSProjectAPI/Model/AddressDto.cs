﻿public class AddressDto
{
    public Guid Id { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string? Street { get; set; }
    public string? BuildingNumber { get; set; }
    public string? Description { get; set; }
}

public class CreateAddressDto
{
    public string Country { get; set; }
    public string City { get; set; }
    public string? Street { get; set; }
    public string? BuildingNumber { get; set; }
    public string? Description { get; set; }
}
