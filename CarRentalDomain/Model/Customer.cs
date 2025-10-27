using System;
using System.Collections.Generic;

namespace CarRentalDomain.Model;

public partial class Customer: Entity
{
   

    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
