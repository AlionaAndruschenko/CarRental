using System;
using System.Collections.Generic;

namespace CarRentalDomain.Model;

public partial class Rental: Entity
{
  

    public int CustomerId { get; set; }

    public int CarId { get; set; }

    public DateTime RentalDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string? Notes { get; set; }

    public virtual Car? Car { get; set; }
    public virtual Customer? Customer { get; set; }
}
