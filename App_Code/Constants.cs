using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for Constants
/// </summary>
public class SyncConstants
{
    public static String ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=test";
    //public static String ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=5Za{-p2N";
    public static String[] IRS_FILE_DOWNLOAD_URL = { "http://www.irs.gov/pub/irs-soi/eo1.csv", "http://www.irs.gov/pub/irs-soi/eo2.csv", "http://www.irs.gov/pub/irs-soi/eo3.csv", "http://www.irs.gov/pub/irs-soi/eo4.csv" };
    public static String OFAC_SDN_DOWNLOAD_URL = "http://www.treasury.gov/ofac/downloads/sdn.csv";
    public static String OFAC_ADD_DOWNLOAD_URL = "http://www.treasury.gov/ofac/downloads/add.csv";
    public static String OFAC_ALT_DOWNLOAD_URL = "http://www.treasury.gov/ofac/downloads/alt.csv";
    public static String PUB78_FILE_DOWNLOAD_URL = "http://apps.irs.gov/pub/epostcard/data-download-pub78.zip";
    public static String DownloadLocation = @"E:/Projects/IRS App/IRSSync/App_Data/downloads";
    public static String DownloadLocationBackup = @"E:/Projects/IRS App/IRSSync/App_Data/downloads_backup";
       
    public static void log(String logtext)
    {
        System.IO.StreamWriter file = new System.IO.StreamWriter(string.Format("{0}/Log/{1}", AppDomain.CurrentDomain.BaseDirectory, "Log.txt"), true);
        file.WriteLine(logtext);
        file.Close();
    }   
    
    public static String GetScheduler_Status(String type)
    {
        String status="stop";
        System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = @"select * from [dbo].[TblSchedulerStatus]";
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(commandText, con);
        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
        System.Data.DataTable dt = new System.Data.DataTable();
        da.Fill(dt);

        if(type.Equals("OFAC"))
            status=dt.Rows[0][0].ToString();
        if(type.Equals("IRS"))
            status=dt.Rows[0][1].ToString();
        if(type.Equals("PUB"))
            status=dt.Rows[0][2].ToString();
        con.Close();
        return status;
    }
}