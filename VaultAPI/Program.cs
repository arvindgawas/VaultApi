using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using VaultAPI.model;

namespace VaultAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            rcmvaultdata();
            //ncmvaultdata();
            //rcmattenddata();
        }

        public static void rcmvaultdata()
        {
            //RCMProdDBEntities dbcon = new RCMProdDBEntities();

            RCMProdDBEntitiesProd dbcon = new RCMProdDBEntitiesProd();
            try
            {
                DateTime dt = DateTime.Parse("2022-7-6");
                List<rcmvault> lstrcmvault = (from a in dbcon.BusinessCallLogs
                                              join b in dbcon.CallDenominations on a.CallNo equals b.CallNo
                                              join c in dbcon.CashDepositTxns on b.CallNo equals c.CallNo
                                              join d in dbcon.BusinessCallLogDetails on b.CallNo equals d.CallNo 
                                              join vm in dbcon.VaultMasters on c.VaultID equals vm.VaultID
                                              join v in dbcon.VaultMappings on vm.ID equals v.RCMVaultId
                                              join mvm in dbcon.MdmVaultMasters on v.MDMVaultid equals mvm.Id
                                              //where c.CallNo == 45120154 && c.DepositType == "V" && c.CallNo == 46094559
                                              //where DbFunctions.TruncateTime(c.ActionDate) == DateTime.Today && c.DepositType == "V" 
                                              where DbFunctions.TruncateTime(c.ActionDate) == dt  && c.DepositType == "V"
                                                && d.ClientCode == b.ClientCode && (a.Region == "1016" || a.Region == "1017")
                                               && mvm.CompanyCode=="CMS"  && c.apisent == null
                                              select new rcmvault
                                              {
                                                  templateId = 5525219007528960,
                                                  tranid = c.Id,
                                                  type = "RCM",
                                                  indentType = "IN",
                                                  genDate = a.GenDate,
                                                  bankCode = a.CustomerCode,
                                                  bankName = d.CustomerName,
                                                  clientCode = a.CustCustomerCode,
                                                  clientName = d.ClientCustName,
                                                  hublocationcode = a.Hublocation,
                                                  callNo = a.CallNo.ToString(),
                                                  VaultUniqueId = mvm.VaultUniqueID,
                                                  vaultId = mvm.VaultCode,
                                                  binId = "CMS-AUR-SIL-IND-RCM",
                                                  //vaultId = "CMS-WES-PUN-AUR-SIL",
                                                  activityName = "Cash Pickup from Customers",
                                                  custodianId = a.AttendBy,
                                                  noDen5 = b.Deno5 == null ? 0 : b.Deno5,
                                                  noDen2 = b.Deno2 == null ? 0 : b.Deno2,
                                                  noDen1 = 0,
                                                  noDen10 = b.Deno10 == null ? 0 : b.Deno10,
                                                  noDen20 = b.Deno20 == null ? 0 : b.Deno20,
                                                  noDen50 = b.Deno50 == null ? 0 : b.Deno50,
                                                  noDen100 = b.Deno100 == null ? 0 : b.Deno100,
                                                  noDen200 = b.Deno200 == null ? 0 : b.Deno200,
                                                  noDen500 = b.Deno500 == null ? 0 : b.Deno500,
                                                  noDen2000 = b.Deno2000 == null ? 0 : b.Deno2000,
                                                  other = b.DenoOthers == null ? 0 : (int)b.DenoOthers
                                              }).ToList();

                foreach (rcmvault objrcmvault in lstrcmvault)
                {
                    var handlernew = new WebRequestHandler();
                    handlernew.AllowAutoRedirect = true;

                    string apiurl = ConfigurationManager.AppSettings["apiurl"].ToString();
                    string apimethodurl = ConfigurationManager.AppSettings["apimethodurl"].ToString();

                    using (HttpClient clientnew = new HttpClient(handlernew))
                    {


                        //binid logic
                        
                        var vbinid = (from a in dbcon.BinMasters
                                      join h in dbcon.HublocationMasts on a.HubLocationName equals h.hublocationname
                                     where h.hublocationcode == objrcmvault.hublocationcode &&  a.ProductType == "RCM" && a.Bank == objrcmvault.bankName
                                     select a.BinCode
                                    ).SingleOrDefault();
                        objrcmvault.binId = vbinid;

                        objrcmvault.indentDate = String.Format("{0:dd/MM/yyyy}", objrcmvault.genDate);
                        clientnew.BaseAddress = new Uri(apiurl);
                        clientnew.DefaultRequestHeaders.Accept.Clear();
                        clientnew.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        clientnew.DefaultRequestHeaders.Add("CLIENT_ID", "4974797796671488");
                        var responseTask = clientnew.PostAsJsonAsync(apimethodurl, objrcmvault);
                        responseTask.Wait();
                        var result = responseTask?.Result;
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var alldata = readTask.Result;

                        var jsonbo = new JavaScriptSerializer().Serialize(objrcmvault);

                       
                        if (result.IsSuccessStatusCode)
                        {

                                var data = (from a in dbcon.CashDepositTxns
                                            where a.Id == objrcmvault.tranid
                                            select a).SingleOrDefault();

                                data.apisent = "Y";
                        }
                      
                    }

                }

                dbcon.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        
        public static void ncmvaultdata()
        {
            //RCMProdDBEntities dbcon = new RCMProdDBEntities();

            RCMProdDBEntitiesProd dbcon = new RCMProdDBEntitiesProd();
            try
            {
                DateTime dt = DateTime.Parse("2022-7-6");
                List<rcmvault> lstrcmvault = (from a in dbcon.BusinessCallLogs
                                              join b in dbcon.CallDenominations on a.CallNo equals b.CallNo
                                              join c in dbcon.RNNCMDepositionTxns on a.CallNo equals c.CallNo
                                              join d in dbcon.RNNCMDepositionTxnDetails on c.Id equals d.RNNCMDepsotionTxnId
                                              join bd in dbcon.BusinessCallLogDetails on b.CallNo equals bd.CallNo
                                              join vm in dbcon.VaultMasters on d.VaultId equals vm.ID
                                              join v in dbcon.VaultMappings on vm.ID equals v.RCMVaultId
                                              join mvm in dbcon.MdmVaultMasters on v.MDMVaultid equals mvm.Id
                                              //where c.CallNo == 19522764 && d.NCMModeId == 8   && (a.Region == "1016" || a.Region == "1017")
                                              //where DbFunctions.TruncateTime(d.CreatedDate) == DateTime.Today  && d.NCMModeId==8 && b.ClientCode == bd.ClientCode
                                              where DbFunctions.TruncateTime(d.CreatedDate) == dt && d.NCMModeId == 8 && b.ClientCode == bd.ClientCode
                                               && mvm.CompanyCode == "CMS" && d.flagapisent==null && (a.Region == "1016" || a.Region == "1017")
                                              select new rcmvault
                                              {
                                                  templateId = 5525219007528960,
                                                  tranid = (int)d.Id,
                                                  type = "NCM",
                                                  indentType = "IN",
                                                  genDate = d.CreatedDate,
                                                  bankCode = a.CustomerCode,
                                                  bankName = bd.CustomerName,
                                                  clientCode = a.CustCustomerCode,
                                                  clientName = a.CustCustomerCode,
                                                  hublocationcode = a.Hublocation,
                                                  callNo = a.CallNo.ToString(),
                                                  VaultUniqueId = mvm.VaultUniqueID,
                                                  vaultId = mvm.VaultCode,
                                                  binId = "CMS-AUR-SIL-IND-RCM",
                                                  //vaultId = "CMS-WES-PUN-AUR-SIL",
                                                  custodianId = a.AttendBy,
                                                  activityName = "Cash Pickup from Customers",
                                                  noDen5 = b.Deno5 == null ? 0 : b.Deno5,
                                                  noDen2 = b.Deno2 == null ? 0 : b.Deno2,
                                                  noDen1 = 0,
                                                  noDen10 = b.Deno10 == null ? 0 : b.Deno10,
                                                  noDen20 = b.Deno20 == null ? 0 : b.Deno20,
                                                  noDen50 = b.Deno50 == null ? 0 : b.Deno50,
                                                  noDen100 = b.Deno100 == null ? 0 : b.Deno100,
                                                  noDen200 = b.Deno200 == null ? 0 : b.Deno200,
                                                  noDen500 = b.Deno500 == null ? 0 : b.Deno500,
                                                  noDen2000 = b.Deno2000 == null ? 0 : b.Deno2000,
                                                  other = b.DenoOthers == null ? 0 : (int)b.DenoOthers
                                              }).ToList();


                foreach (rcmvault objrcmvault in lstrcmvault)
                {
                    var handlernew = new WebRequestHandler();
                    handlernew.AllowAutoRedirect = true;

                    string apiurl = ConfigurationManager.AppSettings["apiurl"].ToString();
                    string apimethodurl = ConfigurationManager.AppSettings["apimethodurl"].ToString();

                    using (HttpClient clientnew = new HttpClient(handlernew))
                    {

                        //binid logic

                        var vbinid = (from a in dbcon.BinMasters
                                      join h in dbcon.HublocationMasts on a.HubLocationName equals h.hublocationname
                                      where h.hublocationcode == objrcmvault.hublocationcode && a.ProductType == "NCM" 
                                      //&& a.Bank == objrcmvault.bankName
                                      select a.BinCode
                                    ).SingleOrDefault();
                        objrcmvault.binId = vbinid;


                        objrcmvault.indentDate = String.Format("{0:dd/MM/yyyy}", objrcmvault.genDate);
                        clientnew.BaseAddress = new Uri(apiurl);
                        clientnew.DefaultRequestHeaders.Accept.Clear();
                        clientnew.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        clientnew.DefaultRequestHeaders.Add("CLIENT_ID", "4974797796671488");
                        var responseTask = clientnew.PostAsJsonAsync(apimethodurl, objrcmvault);
                        responseTask.Wait();
                        var result = responseTask?.Result;
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var alldata = readTask.Result;

                        var jsonbo = new JavaScriptSerializer().Serialize(objrcmvault);

                       
                        if (result.IsSuccessStatusCode)
                        {

                             var data = (from a in dbcon.RNNCMDepositionTxnDetails
                                        where a.Id == objrcmvault.tranid
                                        select a).SingleOrDefault();

                               data.flagapisent = "Y";

                        }
                     
                    }

                }

                dbcon.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static void rcmattenddata()
        {
            RCMProdDBEntities dbcon = new RCMProdDBEntities();
            try
            {
                List<rcmvault> missedtrans = (from a in dbcon.BusinessCallLogs
                                              join b in dbcon.CallDenominations on a.CallNo equals b.CallNo
                                              join d in dbcon.BusinessCallLogDetails on b.CallNo equals d.CallNo
                                              where DbFunctions.TruncateTime(a.AttendDate) == DateTime.Today && (a.CallStatus == "Attend"
                                              || a.CallStatus == "Accept" || a.CallStatus == "Credit") && d.DepositionType == "NCM" 
                                              //&& a.flagapisent == null

                                              select new rcmvault
                                              {
                                                  //tranid = int.Parse(a.CallNo.ToString()),
                                                  type = "NCM",
                                                  indentType = "IN",
                                                  indentDate = a.GenDate.ToString(),
                                                  bankCode = a.CustomerCode,
                                                  bankName = d.CustomerName,
                                                  clientCode = a.CustCustomerCode,
                                                  clientName = d.ClientCustName,
                                                  callNo = a.CallNo.ToString(),
                                                  activityName = "Cash Pickup from Customers",
                                                  custodianId = a.AttendBy,
                                                  noDen5 = b.Deno5 == null ? 0 : b.Deno5,
                                                  noDen2 = b.Deno2 == null ? 0 : b.Deno2,
                                                  noDen1 = 0,
                                                  noDen10 = b.Deno10 == null ? 0 : b.Deno10,
                                                  noDen20 = b.Deno20 == null ? 0 : b.Deno20,
                                                  noDen50 = b.Deno50 == null ? 0 : b.Deno50,
                                                  noDen100 = b.Deno100 == null ? 0 : b.Deno100,
                                                  noDen200 = b.Deno200 == null ? 0 : b.Deno200,
                                                  noDen500 = b.Deno500 == null ? 0 : b.Deno500,
                                                  noDen2000 = b.Deno2000 == null ? 0 : b.Deno2000,
                                                  other = b.DenoOthers == null ? 0 : (int)b.DenoOthers
                                              }).ToList();

                var handlernew = new WebRequestHandler();
                handlernew.AllowAutoRedirect = true;

                string apiurl = "";
                string apimethodurl = "";
                apiurl = ConfigurationManager.AppSettings["apiurl"].ToString();
                apimethodurl = ConfigurationManager.AppSettings["apimethodurl"].ToString();

                var jsonBody = JsonConvert.SerializeObject(missedtrans);
                var scontent = new StringContent(jsonBody.ToString(), Encoding.UTF8, "application/json");

                using (HttpClient clientnew = new HttpClient(handlernew))
                {
                    clientnew.BaseAddress = new Uri(apiurl);
                    clientnew.DefaultRequestHeaders.Accept.Clear();
                    clientnew.DefaultRequestHeaders.Add("CLIENT_ID ", "4540555161763840");
                    clientnew.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask = clientnew.PostAsync(apimethodurl, scontent);
                    responseTask.Wait();
                    var result = responseTask?.Result;
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    var alldata = readTask.Result;
                    /*
                    if (result.IsSuccessStatusCode)
                    {
                        foreach (rcmvault objrcmv in missedtrans)
                        {

                            var data = (from a in dbcon.CashDepositTxns
                                        where a.Id == objrcmv.tranid
                                        select a).SingleOrDefault();

                            data.apisent = "Y";

                        }
                    } 
                    */
                }



                dbcon.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }
}

