using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for OFACService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class OFACService : System.Web.Services.WebService
{
    public OFACService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod]
    public void StartOFACScheduler()
    {
        System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = @"select * from [dbo].[TblSchedulerStatus]";
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(commandText, con);
        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
        System.Data.DataTable dt = new System.Data.DataTable();
        da.Fill(dt);
        if (dt.Rows[0][0].Equals("start"))
            OFACMaster.ScheduleOFACJob();      
        con.Close();
    }
    [WebMethod]
    public void StartIRScheduler()
    {
        System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = @"select * from [dbo].[TblSchedulerStatus]";
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(commandText, con);
        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
        System.Data.DataTable dt = new System.Data.DataTable();
        da.Fill(dt);
        if (dt.Rows[0][1].Equals("start"))
            IRSMaster.ScheduleIRSJob();
        con.Close();
    }
    [WebMethod]
    public void StartPubcheduler()
    {
        System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(SyncConstants.ConnectionString);
        con.Open();
        string commandText = @"select * from [dbo].[TblSchedulerStatus]";
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(commandText, con);
        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
        System.Data.DataTable dt = new System.Data.DataTable();
        da.Fill(dt);
        if (dt.Rows[0][2].Equals("start"))
            Pub78Master.SchedulePUB78Job();
        con.Close();
    }
}
