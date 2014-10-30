using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for Pub78Master
/// </summary>
public class Pub78Master
{
    public static string ExtractZip(string zipfile, string extractfile)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipfile))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(string.Format("{0}{1}", extractfile, entry.FullName)))
                    {
                        File.Delete(string.Format("{0}{1}", extractfile, entry.FullName));
                    }
                    entry.ExtractToFile(Path.Combine(extractfile, entry.FullName));
                    return string.Format("{0}{1}", extractfile, entry.FullName);
                }
            }
        }
        return "";
    }
    public static bool CheckSyncStatus()
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = @"select * from [dbo].[tblPub78LoadHistory] where loadNotes='PROCESSING'";
        SqlCommand cmd = new SqlCommand(commandText, con);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        con.Close();
        return dt.Rows.Count > 0;
    }
    public static int InsertProcessingEntry(string masterfileversiondate)
    {
        try
        {
            SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
            con.Open();
            string insertquery = string.Format("INSERT INTO [dbo].[tblPub78LoadHistory] ([loadDate],[Pub78Version],[loadedBy],[loadNotes]) values (GETDATE(),'{1}','{2}','{3}')", DateTime.Now, masterfileversiondate, "ROUTINE", "PROCESSING");
            SqlCommand cmd = new SqlCommand(insertquery, con);
            return cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
    public static void ImportPub78(string downloadurl, string masterfileversion)
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand("[dbo].[uspImportPub78]", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = int.MaxValue;
        cmd.Parameters.Add(new SqlParameter("downloadurl", downloadurl));
        cmd.Parameters.Add(new SqlParameter("masterfileversion", masterfileversion));

        string status = cmd.ExecuteScalar().ToString();
        SyncConstants.log(string.Format("\n PUB78 Sync : DateTime: {0}, Status: {1}", DateTime.Now.ToString(), status));
    }
    public static void SchedulePUB78Job()
    {
        Pub78Sync();
    }  
    public static void Pub78Sync()
    {
        DateTime maxdate = DateTime.Now.AddYears(-50);
        string strDownloadURL = SyncConstants.PUB78_FILE_DOWNLOAD_URL;
        HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(strDownloadURL);
        HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
        if (myHttpWebResponse.StatusCode == HttpStatusCode.OK && maxdate < myHttpWebResponse.LastModified)
            maxdate = myHttpWebResponse.LastModified;
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        SqlCommand cmd = new SqlCommand("select dbo.CheckForPUB78Update(@masterfileversion)", con);
        cmd.Parameters.Add("@masterfileversion", String.Format("{0:MMMM dd, yyyy}", maxdate));
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        string flag = dt.Rows[0][0].ToString();
        if (Convert.ToBoolean(flag))
        {
            if (!CheckSyncStatus())
            {
                InsertProcessingEntry(String.Format("{0:MMMM dd, yyyy}", maxdate));
                WebClient webClient = new WebClient();
                string filename = System.IO.Path.GetFileName(strDownloadURL);
                File.Copy(string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename), string.Format("{0}\\{1}", SyncConstants.DownloadLocationBackup, filename), true);
                webClient.DownloadFile(new Uri(strDownloadURL), string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename));
                String strpub78file = ExtractZip(string.Format("{0}\\{1}", SyncConstants.DownloadLocation, filename), string.Format("{0}\\", SyncConstants.DownloadLocation));
                if (!string.IsNullOrEmpty(strpub78file))
                    ImportPub78(strpub78file, String.Format("{0:MMMM dd, yyyy}", maxdate));
            }
        }
    }
    public static void UpdateSchedulerStatus(string status)
    {
        SqlConnection con = new SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = string.Format(@"update [dbo].[TblSchedulerStatus] set [Pub78]='{0}'", status);
        SqlCommand cmd = new SqlCommand(commandText, con);
        cmd.ExecuteNonQuery();
        con.Close();
    }
}