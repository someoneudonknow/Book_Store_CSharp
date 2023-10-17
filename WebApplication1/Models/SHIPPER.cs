//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class SHIPPER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SHIPPER()
        {
            this.CUSTOMER_ORDER = new HashSet<CUSTOMER_ORDER>();
            this.SHIP_CONFIRMATION = new HashSet<SHIP_CONFIRMATION>();
        }
    
        public int ShipperID { get; set; }
        public string ShipperName { get; set; }
        public string ShipperPhoneNumber { get; set; }
        public string ShipperAddress { get; set; }
        public string ShipperStatus { get; set; }
        public string AccountID { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUSTOMER_ORDER> CUSTOMER_ORDER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SHIP_CONFIRMATION> SHIP_CONFIRMATION { get; set; }
    }
}
