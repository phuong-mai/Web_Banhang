using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_BanHang.Models
{
    public class ItemCombo
    {
        public List<Combo> Combo
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }
        public int Stock
        {
            get;
            set;
        }
    }
}