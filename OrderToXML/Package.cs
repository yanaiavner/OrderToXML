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
    
    public partial class Package
    {
        public int PackageID { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public int PackageStateID { get; set; }
        public Nullable<int> PackageTypeID { get; set; }
        public string PDF { get; set; }
        public bool AddToPackingList { get; set; }
        public string DisplayName { get; set; }
    }
}