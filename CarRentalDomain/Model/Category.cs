using System.ComponentModel.DataAnnotations;

namespace CarRentalDomain.Model;

public partial class Category : Entity
{
    [Required(ErrorMessage ="Поле не повинно бути порожнім")]
    [Display(Name = "Категорія")]

    public string Name { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
