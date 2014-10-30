using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (SyncConstants.GetScheduler_Status("OFAC").Equals("stop"))
            btnStartOFAC.Text = "Start Scheduler";
        else
            btnStartOFAC.Text = "Stop Scheduler";
        if (SyncConstants.GetScheduler_Status("IRS").Equals("stop"))
            btnStartIRS.Text = "Start Scheduler";
        else
            btnStartIRS.Text = "Stop Scheduler";
        if (SyncConstants.GetScheduler_Status("PUB").Equals("stop"))
            btnStartPub.Text = "Start Scheduler";
        else
            btnStartPub.Text = "Stop Scheduler";

        if (OFACMaster.CheckForDBUpdate())
        {
            btnRunOFAC.Enabled = false;
            btnRunOFAC.Text = "OFAC Sync in progress";
        }
        else
        {
            btnRunOFAC.Enabled = true;
            btnRunOFAC.Text = "Run OFAC Sync";
        }

        if (Pub78Master.CheckSyncStatus())
        {
            btnRunPub.Enabled = false;
            btnRunPub.Text = "Pub78 Sync in progress";
        }
        else
        {
            btnRunPub.Enabled = true;
            btnRunPub.Text = "Run Pub78 Sync";
        }

        if (IRSMaster.CheckSyncStatus())
        {
            btnRunIRS.Enabled = false;
            btnRunIRS.Text = "IRS Sync in progress";
        }
        else
        {
            btnRunIRS.Enabled = true;
            btnRunIRS.Text = "Run IRS Sync";
        }
    }
    protected void btnStartOFAC_Click(object sender, EventArgs e)
    {
        try
        {
            if (SyncConstants.GetScheduler_Status("OFAC").Equals("stop"))
            {
                OFACMaster.UpdateSchedulerStatus("start");
                btnStartOFAC.Text = "Stop Scheduler";
            }
            else
            {
                OFACMaster.UpdateSchedulerStatus("stop");
                btnStartOFAC.Text = "Start Scheduler";
            }
        }
        catch (Exception ex)
        {
            SyncConstants.log(string.Format("Error in OFAC Scheduler: {0}", ex));
        }
    }
    protected void btnStartIRS_Click(object sender, EventArgs e)
    {
        try
        {
            if (SyncConstants.GetScheduler_Status("IRS").Equals("stop"))
            {
                IRSMaster.UpdateSchedulerStatus("start");
                btnStartIRS.Text = "Stop Scheduler";
            }
            else
            {
                IRSMaster.UpdateSchedulerStatus("stop");
                btnStartIRS.Text = "Start Scheduler";
            }
        }
        catch (Exception ex)
        {
            SyncConstants.log(string.Format("Error in IRS Scheduler: {0}", ex));
        }
    }
    protected void btnStartPub_Click(object sender, EventArgs e)
    {
        try
        {
            if (SyncConstants.GetScheduler_Status("PUB").Equals("stop"))
            {
                Pub78Master.UpdateSchedulerStatus("start");
                btnStartPub.Text = "Stop Scheduler";
            }
            else
            {
                Pub78Master.UpdateSchedulerStatus("stop");
                btnStartPub.Text = "Start Scheduler";
            }
        }
        catch (Exception ex)
        {
            SyncConstants.log(string.Format("Error in Pub78 Scheduler: {0}", ex));
        }
    }
    protected void btnRunOFAC_Click(object sender, EventArgs e)
    {
        try
        {
            OFACMaster.OFACSync();
            gvOFAC.DataBind();
        }
        catch (Exception ex)
        {
            SyncConstants.log(string.Format("\n Error in OFAC sync: {0}", ex));
        }
    }
    protected void btnRunIRS_Click(object sender, EventArgs e)
    {
        try
        {
            IRSMaster.IRSSync();
            gvIRS.DataBind();
        }
        catch (Exception ex)
        {
            SyncConstants.log(string.Format("\n Error in IRS sync: {0}", ex));
        }
    }
    protected void btnRunPub_Click(object sender, EventArgs e)
    {
        try
        {
            Pub78Master.Pub78Sync();
            gvPUb78.DataBind();
        }
        catch (Exception ex)
        {
            SyncConstants.log(string.Format("\n Error in PUB78 sync: {0}", ex));
        }
    }
}