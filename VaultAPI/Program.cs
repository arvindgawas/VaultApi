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
            //rcmattenddata();
            //ncmvaultdata();
        }
        public static void rcmvaultdata()
        {
            RCMProdDBEntities dbcon = new RCMProdDBEntities();
            try
            {
                List<rcmvault> lstrcmvault = (from a in dbcon.BusinessCallLogs
                                              join b in dbcon.CallDenominations on a.CallNo equals b.CallNo
                                              join c in dbcon.CashDepositTxns on b.CallNo equals c.CallNo
                                              join d in dbcon.BusinessCallLogDetails on b.CallNo equals d.CallNo
                                              //join vm in dbcon.VaultMasters on c.VaultID equals vm.VaultID
                                              //join v in dbcon.VaultMappings on vm.ID equals v.RCMVaultId
                                              //join mvm in dbcon.MdmVaultMasters on v.MDMVaultid equals mvm.Id
                                              where c.CallNo == 42344848 && c.DepositType == "V"
                                              //where DbFunctions.TruncateTime(c.ActionDate) == DateTime.Today && c.DepositType == "V" && c.apisent == null
                                              select new rcmvault
                                              {
                                                  templateId = 5525219007528960,
                                                  tranid = c.Id,
                                                  type = "RCM",
                                                  indentType="IN",
                                                  indentDate = c.ActionDate.ToString(),
                                                  bankCode = a.CustomerCode,
                                                  bankName = d.CustomerName,
                                                  clientCode = a.CustCustomerCode,
                                                  clientName = d.ClientCustName,
                                                  callNo = a.CallNo.ToString(),
                                                  //vaultId = mvm.VaultCode,
                                                  vaultId = c.VaultID,
                                                  activityName = "Cash Pickup from Customers",
                                                  custodianId = a.AttendBy,
                                                  noDeno5 = b.Deno5 == null ? 0 : b.Deno5,
                                                  noDeno2 = b.Deno2 == null ? 0 : b.Deno2,
                                                  noDeno1 = 0,
                                                  noDeno10 = b.Deno10 == null ? 0 : b.Deno10,
                                                  noDeno20 = b.Deno20 == null ? 0 : b.Deno20,
                                                  noDeno50 = b.Deno50 == null ? 0 : b.Deno50,
                                                  noDeno100 = b.Deno100 == null ? 0 : b.Deno100,
                                                  noDeno200 = b.Deno200 == null ? 0 : b.Deno200,
                                                  noDeno500 = b.Deno500 == null ? 0 : b.Deno500,
                                                  noDeno2000 = b.Deno2000 == null ? 0 : b.Deno2000,
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

                        /*
                        if (result.IsSuccessStatusCode)
                        {
                            foreach (rcmvault objrcmv in missedtrans)
                            {

                                var data = (from a in dbcon.CashDepositTxns
                                            where a.Id == objrcmv.tranid
                                            select a).SingleOrDefault();

                                //data.apisent = "Y";

                            }
                        }
                        */
                    }

                }

                dbcon.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void  rcmattenddata()
        {
            RCMProdDBEntities dbcon = new RCMProdDBEntities();
            try
            {
                List<rcmvault> missedtrans = (from a in dbcon.BusinessCallLogs
                                              join b in dbcon.CallDenominations on a.CallNo equals b.CallNo
                                              join d in dbcon.BusinessCallLogDetails on b.CallNo equals d.CallNo
                                              where DbFunctions.TruncateTime(a.AttendDate) == DateTime.Today && (a.CallStatus == "Attend" 
                                              || a.CallStatus == "Accept" || a.CallStatus=="Credit" ) && d.DepositionType=="NCM" && a.flagapisent == null

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
                                                  noDeno5 = b.Deno5 == null ? 0 : b.Deno5 ,
                                                  noDeno2 = b.Deno2 == null ? 0 : b.Deno2,
                                                  noDeno1 = 0,
                                                  noDeno10 = b.Deno10 == null ? 0 : b.Deno10 ,
                                                  noDeno20 = b.Deno20 == null ? 0 : b.Deno20 ,
                                                  noDeno50 = b.Deno50 == null ? 0 : b.Deno50 ,
                                                  noDeno100 = b.Deno100 == null ? 0 : b.Deno100,
                                                  noDeno200 = b.Deno200 == null ? 0 : b.Deno200,
                                                  noDeno500 = b.Deno500 == null ? 0 : b.Deno500 ,
                                                  noDeno2000 = b.Deno2000 == null ? 0 : b.Deno2000,
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
        public static void ncmvaultdata()
        {
            RCMProdDBEntities dbcon = new RCMProdDBEntities();
            try
            {
                List<rcmvault> lstrcmvault = (from a in dbcon.BusinessCallLogs
                                              join b in dbcon.CallDenominations on a.CallNo equals b.CallNo
                                              join c in dbcon.RNNCMDepositionTxns on a.CallNo equals c.CallNo
                                              join d in dbcon.RNNCMDepositionTxnDetails on c.Id equals d.RNNCMDepsotionTxnId
                                              where c.CallNo == 19522764 && d.NCMModeId == 8
                                              //where DbFunctions.TruncateTime(d.CreatedDate) == DateTime.Today  && d.NCMModeId==8 && d.flagapisent==null
                                              select new rcmvault
                                              {
                                                  templateId = 5525219007528960,
                                                  tranid = (int)d.Id,
                                                  type = "NCM",
                                                  indentType = "IN",
                                                  indentDate = d.CreatedDate.ToString(),
                                                  bankCode = a.CustomerCode,
                                                  bankName = a.CustomerCode,
                                                  clientCode = a.CustCustomerCode,
                                                  clientName = a.CustCustomerCode,
                                                  callNo = a.CallNo.ToString(),
                                                  vaultId = d.VaultId.ToString(),
                                                  custodianId = a.AttendBy,
                                                  activityName = "Cash Pickup from Customers",
                                                  noDeno5 = b.Deno5 == null ? 0 : b.Deno5,
                                                  noDeno2 = b.Deno2 == null ? 0 : b.Deno2,
                                                  noDeno1 = 0,
                                                  noDeno10 = b.Deno10 == null ? 0 : b.Deno10,
                                                  noDeno20 = b.Deno20 == null ? 0 : b.Deno20,
                                                  noDeno50 = b.Deno50 == null ? 0 : b.Deno50,
                                                  noDeno100 = b.Deno100 == null ? 0 : b.Deno100,
                                                  noDeno200 = b.Deno200 == null ? 0 : b.Deno200,
                                                  noDeno500 = b.Deno500 == null ? 0 : b.Deno500,
                                                  noDeno2000 = b.Deno2000 == null ? 0 : b.Deno2000,
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

                        /*
                        if (result.IsSuccessStatusCode)
                        {
                            foreach (rcmvault objrcmv in missedtrans)
                            {

                             var data = (from a in dbcon.RNNCMDepositionTxnDetails
                                        where a.Id == objrcmv.tranid
                                        select a).SingleOrDefault();

                                //data.apisent = "Y";

                            }
                        }
                        */
                    }

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

