﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RCMProdDBEntities : DbContext
    {
        public RCMProdDBEntities()
            : base("name=RCMProdDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CallDenomination> CallDenominations { get; set; }
        public virtual DbSet<BusinessCallLogDetail> BusinessCallLogDetails { get; set; }
        public virtual DbSet<BusinessCallLog> BusinessCallLogs { get; set; }
        public virtual DbSet<RNNCMDepositionTxnDetail> RNNCMDepositionTxnDetails { get; set; }
        public virtual DbSet<RNNCMDepositionTxn> RNNCMDepositionTxns { get; set; }
        public virtual DbSet<CashDepositTxn> CashDepositTxns { get; set; }
        public virtual DbSet<BinMaster> BinMasters { get; set; }
        public virtual DbSet<MdmVaultMaster> MdmVaultMasters { get; set; }
        public virtual DbSet<VaultMapping> VaultMappings { get; set; }
        public virtual DbSet<VaultMaster> VaultMasters { get; set; }
    }
}