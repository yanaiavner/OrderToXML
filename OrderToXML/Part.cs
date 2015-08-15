//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderToXML
{
    using System;
    using System.Collections.Generic;
    
    public partial class Part
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Part()
        {
            this.PackageParts = new HashSet<PackagePart>();
        }
    
        public int PartID { get; set; }
        public string OLISID { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public int VendorID { get; set; }
        public string InventoryLocation { get; set; }
        public Nullable<decimal> LastUnitCost { get; set; }
        public string PCBWorksheets { get; set; }
        public Nullable<bool> NotPulledFromInventory { get; set; }
        public Nullable<double> LeadTime_ { get; set; }
        public Nullable<int> ModifiedFromPartID { get; set; }
        public Nullable<int> PreassemblyPartID { get; set; }
        public Nullable<int> ProceesTypeID { get; set; }
        public Nullable<bool> IsModified { get; set; }
        public Nullable<bool> IsLongLeadTime { get; set; }
        public bool AddToPackingList { get; set; }
        public string DisplayName { get; set; }
        public string MPN { get; set; }
        public string SageNote { get; set; }
        public Nullable<decimal> QuantityOnHand { get; set; }
        public bool IsPrimaryVendor { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PackagePart> PackageParts { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
