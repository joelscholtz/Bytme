using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Models
{
    public class OrderMain
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public DateTime dt_ordered { get; set; }
        public DateTime dt_delivery { get; set; }
        public string description { get; set; }
        public DateTime dt_created { get; set; }
        public DateTime dt_modified { get; set; }
    }
    public class OderHistory
    {
        public int id { get; set; }
        public string itm_description { get; set; }
        public int qty_bought { get; set; }
        public int ord_id { get; set; }
        public float price_payed { get; set; }
        public DateTime dt_created { get; set; }
    }
    public class OrderStatus
    {
        public int id { get; set; }
        public string description { get; set; }
        public DateTime dt_created { get; set; }
        public DateTime dt_modified { get; set; }
    }
}
