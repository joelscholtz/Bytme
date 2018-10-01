using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Models
{
    public class Item
    {
        public int id { get; set; }
        public string gender { get; set; }
        public string description { get; set; }
        public string long_description { get; set; }
        public float price { get; set; }
        public string size { get; set; }
        public string photo_url { get; set; }
        public int category_id { get; set; }
        public int quantity { get; set; }
        public int issales { get; set; }
        public DateTime dt_created { get; set; }
        public DateTime dt_modified { get; set; }
    }
    public class ItemCategories
    {
        public int id { get; set; }
        public string description { get; set; }
        public DateTime dt_created { get; set; }
        public DateTime dt_modified { get; set; }

    }
}
