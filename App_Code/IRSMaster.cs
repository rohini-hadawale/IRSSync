using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for IRSMaster
/// </summary>
public class IRSMaster
{
    public class MasterFileVersion
    {
        private bool flag;
        private string masterfileversion;
        public bool Flag
        {
            get { return flag; }
            set { flag = value; }
        }
        public string MasterFileVersionDate
        {
            get { return masterfileversion; }
            set { masterfileversion = value; }
        }
    }

    public static bool CheckSyncStatus()
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = @"select * from [dbo].[tblIRSMasterFileLoadHistory] where loadNotes='PROCESSING'";
        SqlCommand cmd = new SqlCommand(commandText, con);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        con.Close();
        return dt.Rows.Count > 0;
    }
    
    public MasterFileVersion CheckForDBUpdate()
    {
        DateTime maxdate = DateTime.Now.AddYears(-50);
        String[] arrDownloadURLs = SyncConstants.IRS_FILE_DOWNLOAD_URL;
        foreach (String strDownloadURL in arrDownloadURLs)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(strDownloadURL);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            if (myHttpWebResponse.StatusCode == HttpStatusCode.OK && maxdate < myHttpWebResponse.LastModified)
                maxdate = myHttpWebResponse.LastModified;
        }
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        SqlCommand cmd = new SqlCommand("select dbo.checkForDBUpdate(@date)", con);
        cmd.Parameters.Add("@date", String.Format("{0:MMMM dd, yyyy}", maxdate));
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        string flag = dt.Rows[0][0].ToString();
        MasterFileVersion objMasterFileVersion = new MasterFileVersion();
        objMasterFileVersion.Flag = Convert.ToBoolean(flag);
        objMasterFileVersion.MasterFileVersionDate = String.Format("{0:MMMM dd, yyyy}", maxdate);
        return objMasterFileVersion;
    }
    
    public int InsertProcessingEntry(string masterfileversiondate)
    {
        try
        {
            SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
            con.Open();
            string insertquery = string.Format("insert into dbo.tblIRSMasterFileLoadHistory(loadDate,IRSMasterFileVersion,loadedBy,loadNotes) values (GETDATE(),'{1}','{2}','{3}')", DateTime.Now, masterfileversiondate, "ROUTINE", "PROCESSING");
            SqlCommand cmd = new SqlCommand(insertquery, con);
            return cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
    
    public static void ImportIRS()
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand("dbo.uspImportIRS", con);
        cmd.CommandTimeout = int.MaxValue;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("DownloadURLs", string.Join(",", SyncConstants.IRS_FILE_DOWNLOAD_URL).Replace("http://www.irs.gov/pub/irs-soi", SyncConstants.DownloadLocation)));
        string status = cmd.ExecuteScalar().ToString();
        SyncConstants.log(string.Format("\n IRS Sync : DateTime: {0}, Status: {1}", DateTime.Now.ToString(), status));
        if (!status.Equals("1"))
        {
            string deletequery = string.Format("delete from  dbo.tblIRSMasterFileLoadHistory where loadnotes='PROCESSING'");
            cmd = new SqlCommand(deletequery, con);
            cmd.ExecuteNonQuery();
        }
    }
    public static void ScheduleIRSJob()
    {
        IRSSync();
    }
   
    public static void IRSSync()
    {
        try
        {
            IRSMaster objMaster = new IRSMaster();
            IRSMaster.MasterFileVersion objMasterFileVersion;
            objMasterFileVersion = objMaster.CheckForDBUpdate();
            if (objMasterFileVersion.Flag)
            {
                if (objMaster.InsertProcessingEntry(objMasterFileVersion.MasterFileVersionDate) == 1)
                {
                    WebClient webClient = new WebClient();
                    string[] strDownloadURLs = SyncConstants.IRS_FILE_DOWNLOAD_URL;
                    foreach (string strDownloadURL in strDownloadURLs)
                    {
                        string filename = System.IO.Path.GetFileName(strDownloadURL);
                        if (File.Exists(string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename)))
                        {
                            File.Copy(string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename), string.Format("{0}\\{1}", SyncConstants.DownloadLocationBackup, filename), true);
                            File.Delete(string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename));
                        }
                        webClient.DownloadFile(new Uri(strDownloadURL), string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename));
                    }
                    ImportIRS();
                }
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
        string commandText = string.Format(@"update [dbo].[TblSchedulerStatus] set [IRS]='{0}'", status);
        SqlCommand cmd = new SqlCommand(commandText, con);
        cmd.ExecuteNonQuery();
        con.Close();
    }
}