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
        public string description { get; set; }
        public string long_description { get; set; }
        public int quantity { get; set; }
        public string photo_url { get; set; }
        public float total { get; set; }
        public float subtotal { get; set; }
        public int stock { get; set; }

    }
}
