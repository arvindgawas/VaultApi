//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VaultAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class VaultMaster
    {
        public int ID { get; set; }
        public string VaultID { get; set; }
        public string VaultName { get; set; }
        public string VaultType { get; set; }
        public decimal VaultLimit { get; set; }
        public string RegionCode { get; set; }
        public string LocationCode { get; set; }
        public string HubLocationCode { get; set; }
        public string SubLocation { get; set; }
        public string CompanyCode { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    }
}
