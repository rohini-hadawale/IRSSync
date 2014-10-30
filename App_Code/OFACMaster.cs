using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for OFACMaster
/// </summary>
public class OFACMaster
{
    public static bool CheckForDBUpdate()
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = @"select * from [dbo].[ofac_execution_log] where [add_update_status]='PROCESSING' and [sdn_update_status]='PROCESSING' and [alt_update_status]='PROCESSING'";
        SqlCommand cmd = new SqlCommand(commandText, con);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        con.Close();
        return dt.Rows.Count > 0;
    }
    public static void InsertProcessingEntry()
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand(@"insert into [dbo].[ofac_execution_log]([execution_date],[add_records_affected],[sdn_records_affected],[alt_records_affected],[add_update_status],[sdn_update_status],[alt_update_status]) values (GETDATE(),0,0,0,'PROCESSING','PROCESSING','PROCESSING')", con);
        cmd.ExecuteNonQuery();
        con.Close();
    }    
    public static void ImportOFAC(string downloadurl_sdn, string rowcount_sdn, string downloadurl_add, string rowcount_add, string downloadurl_alt, string rowcount_alt)
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand("dbo.uspImportOFAC", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = int.MaxValue;
        cmd.Parameters.Add(new SqlParameter("downloadurl_sdn", downloadurl_sdn));
        cmd.Parameters.Add(new SqlParameter("rowcount_sdn", rowcount_sdn));

        cmd.Parameters.Add(new SqlParameter("downloadurl_add", downloadurl_add));
        cmd.Parameters.Add(new SqlParameter("rowcount_add", rowcount_add));

        cmd.Parameters.Add(new SqlParameter("downloadurl_alt", downloadurl_alt));
        cmd.Parameters.Add(new SqlParameter("rowcount_alt", rowcount_alt));

        string status = cmd.ExecuteScalar().ToString();
        SyncConstants.log(string.Format("\n OFAC Sync : DateTime: {0}, Status: {1}", DateTime.Now.ToString(), status));
    }

    public static void ScheduleOFACJob()
    {
       OFACSync();
    }    
    
    public static void OFACSync()
    {
        try
        {
            if (!CheckForDBUpdate())
            {
                InsertProcessingEntry();
                WebClient webClient = new WebClient();
                string[] strDownloadURLs = { SyncConstants.OFAC_SDN_DOWNLOAD_URL, SyncConstants.OFAC_ADD_DOWNLOAD_URL, SyncConstants.OFAC_ALT_DOWNLOAD_URL };
                foreach (string strDownloadURL in strDownloadURLs)
                {
                    string filename = System.IO.Path.GetFileName(strDownloadURL);
                    File.Copy(string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename), string.Format("{0}\\{1}", SyncConstants.DownloadLocationBackup, filename), true);
                    webClient.DownloadFile(new Uri(strDownloadURL), string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename));
                }
                string downloadurl_sdn = SyncConstants.OFAC_SDN_DOWNLOAD_URL.Replace("http://www.treasury.gov/ofac/downloads/", SyncConstants.DownloadLocation + "/");
                string downloadurl_add = SyncConstants.OFAC_ADD_DOWNLOAD_URL.Replace("http://www.treasury.gov/ofac/downloads/", SyncConstants.DownloadLocation + "/");
                string downloadurl_alt = SyncConstants.OFAC_ALT_DOWNLOAD_URL.Replace("http://www.treasury.gov/ofac/downloads/", SyncConstants.DownloadLocation + "/");
                string rowcount_sdn = (File.ReadLines(downloadurl_sdn).Count() - 1).ToString();
                string rowcount_add = (File.ReadLines(downloadurl_add).Count() - 1).ToString();
                string rowcount_alt = (File.ReadLines(downloadurl_alt).Count() - 1).ToString();
                ImportOFAC(downloadurl_sdn, rowcount_sdn, downloadurl_add, rowcount_add, downloadurl_alt, rowcount_alt);
            }
        }
        catch (Exception ex)
        {
            SyncConstants.log("\n" + ex);
        }


    }

    public static void UpdateSchedulerStatus(string status)
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = string.Format(@"update [dbo].[TblSchedulerStatus] set [OFAC]='{0}'", status);
        SqlCommand cmd = new SqlCommand(commandText, con);
        cmd.ExecuteNonQuery();
        con.Close();
    }
}