using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_BanHang.Models
{
    public class Item
    {
        public List<Product> Product
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }
    }
}