using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Models
{
    public class ShoppingCartModel
    {
        public int id { get; set; }
        public int item_id { get; set; }
        public float price { get; set; }

        public float total { get; set; }
        public float subtotal { get; set; }

    }
}
