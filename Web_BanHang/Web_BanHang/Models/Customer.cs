using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_BanHang.Models
{
    public class Customer
    {
        [Key]
        public int customer_ID { get; set; }

    }
}