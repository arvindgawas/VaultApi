using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultAPI.model
{
    public class rcmvault
    {
        public long templateId { get; set; }

        public int tranid { get; set; }
        public string type { get; set; }
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string clientCode { get; set; }
        public string clientName { get; set; }
        public string callNo { get; set; }
        public string vaultId { get; set; }
        public string binId { get; set; }
        public string indentType { get; set; }
        public string indentDate { get; set; }
        public DateTime genDate { get; set; }
        public string custodianId { get; set; }
        public string activityName { get; set; }
        public int? noDen5 { get; set; }
        public int? noDen2 { get; set; }
        public int? noDen1 { get; set; }
        public int? noDen10 { get; set; }
        public int? noDen20 { get; set; }
        public int? noDen50 { get; set; }
        public int? noDen100 { get; set; }
        public int? noDen200 { get; set; }
        public int? noDen500 { get; set; }
        public int? noDen2000 { get; set; }
        public int? other { get; set; }

    }
}


