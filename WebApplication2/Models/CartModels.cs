using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class CartModels
    {
        public BOOK_EDITION Book_Information { get; set; }
        [Display(Name = "Ảnh bìa")]
        public string BookImage { get; set; }
        [Display(Name = "Giảm giá")]
        public decimal Discount { get; set; }
        [Display(Name = "Số lượng")]
        public int Amount { get; set; }
        [Display(Name = "Tổng cộng")]
        public decimal Total { get; set; }

    }
}