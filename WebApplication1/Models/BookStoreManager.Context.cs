﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BookStoreManagerEntity : DbContext
    {
        public BookStoreManagerEntity()
            : base("name=BookStoreManagerEntity")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<BANK_ACCOUNT> BANK_ACCOUNT { get; set; }
        public virtual DbSet<BOOK_EDITION> BOOK_EDITION { get; set; }
        public virtual DbSet<BOOK_EDITION_IMAGE> BOOK_EDITION_IMAGE { get; set; }
        public virtual DbSet<BOOK_REVIEW> BOOK_REVIEW { get; set; }
        public virtual DbSet<CATEGORY> CATEGORies { get; set; }
        public virtual DbSet<CUSTOMER> CUSTOMERs { get; set; }
        public virtual DbSet<CUSTOMER_ORDER> CUSTOMER_ORDER { get; set; }
        public virtual DbSet<CUSTOMER_ORDER_DETAIL> CUSTOMER_ORDER_DETAIL { get; set; }
        public virtual DbSet<MANAGER> MANAGERs { get; set; }
        public virtual DbSet<PROMOTION> PROMOTIONs { get; set; }
        public virtual DbSet<PUBLISHER> PUBLISHERs { get; set; }
        public virtual DbSet<SHIP_CONFIRMATION> SHIP_CONFIRMATION { get; set; }
        public virtual DbSet<SHIPPER> SHIPPERs { get; set; }
        public virtual DbSet<STAFF> STAFFs { get; set; }
        public virtual DbSet<STOCK_INVENTORY> STOCK_INVENTORY { get; set; }
        public virtual DbSet<STOCK_RECEIVED_NOTE> STOCK_RECEIVED_NOTE { get; set; }
        public virtual DbSet<STOCK_RECEIVED_NOTE_DETAIL> STOCK_RECEIVED_NOTE_DETAIL { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TIER> TIERs { get; set; }
        public virtual DbSet<TRANSACTION_DETAIL> TRANSACTION_DETAIL { get; set; }
        public virtual DbSet<WALLET> WALLETs { get; set; }
    }
}
