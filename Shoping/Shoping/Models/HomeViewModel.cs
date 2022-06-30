using Shoping.Common;
using Shoping.Data.Entities;

namespace Shoping.Models
{
    public class HomeViewModel
    {
        //public ICollection<Product> Products { get; set; }
        public PaginatedList<Product> Products { get; set; }

        public ICollection<Category> Categories { get; set; }
        public float Quantity { get; set; }

    }
}
