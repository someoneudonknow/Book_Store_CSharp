﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public partial class STOCK_INVENTORY
    {
        [Display(Name ="Tổng số lượng nhập")]
        public int InventoryStockInTotal { get; set; }
        [Display(Name = "Tổng số lượng xuất")]
        public int InventoryStockOutTotal { get; set; }
        [Display(Name = "Số lượng sẵn có")]
        public int InventoryAvailableStock { get; set; }
        public int EditionID { get; set; }
    
        public virtual BOOK_EDITION BOOK_EDITION { get; set; }
    }
}
