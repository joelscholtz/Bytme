using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Models
{
    public class OrderMain
    {
        public int id { get; set; }
        public string user_id { get; set; }
        public int ordstatus_id { get; set; }
        public DateTime dt_created { get; set; }
    }
    public class OrderLines
    {
        public int id { get; set; }
        public int item_id { get; set; }
        public int order_id { get; set; }
        public int qty { get; set; }
    }
    public class OrderHistory
    {
        public int id { get; set; }
        public string item_description { get; set; }
        public string street { get; set; }
        public string streetnumber { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public int qty_bought { get; set; }
        public int ord_id { get; set; }
        public int oderline_id { get; set; }
        public string user_id { get; set; }
        public float price_payed { get; set; }
        public string discount { get; set; }
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
