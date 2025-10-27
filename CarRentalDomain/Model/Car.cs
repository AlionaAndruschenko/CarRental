﻿using System;
using System.Collections.Generic;

namespace CarRentalDomain.Model;

public partial class Car: Entity
{
   

    public string Make { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string? PhotoPath { get; set; }

    public int CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
