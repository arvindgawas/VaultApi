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
    
    public partial class RNNCMDepositionTxn
    {
        public long Id { get; set; }
        public Nullable<decimal> CallNo { get; set; }
        public Nullable<decimal> PickupAmt { get; set; }
        public Nullable<decimal> DepositedAmt { get; set; }
        public Nullable<decimal> PendingAmt { get; set; }
        public Nullable<bool> Obsolete { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
