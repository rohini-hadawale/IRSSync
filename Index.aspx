<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="StyleSheet.css" rel="stylesheet" />
    <script type="text/javascript">
        function disablebutton(source, synctype) {
            source.value = synctype + ' Sync in progress';
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
        <div style="width: 900px; margin: 0 auto; text-align: center; border: 0px solid black">
            <img src="images/logo.jpg" />
        </div>
        <div style="width: 900px; margin: 0 auto; border: 0px solid black; margin-top: 20px;">
            <telerik:RadTabStrip ID="RadTabStrip1" runat="server" Skin="Office2007" SelectedIndex="0" Orientation="HorizontalTop"
                MultiPageID="RadMultiPage1">
                <Tabs>
                    <telerik:RadTab Text="OFAC Sync">
                    </telerik:RadTab>
                    <telerik:RadTab Text="IRS Sync">
                    </telerik:RadTab>
                    <telerik:RadTab Text="Pub78 Sync">
                    </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" Style="padding: 20px; border: 2px solid #B5B3B3; border-top: 0px none; background-color: ">
                <telerik:RadPageView runat="server" ID="RadPageView1">
                    <span class="heading">OFAC Sync Status</span>
                    <div class="heading_border"></div>
                    <div style="text-align: right; margin-bottom: 10px;">
                        <asp:Button ID="btnStartOFAC" runat="server" Text="Start Scheduler" OnClick="btnStartOFAC_Click" />
                        <asp:Button ID="btnRunOFAC" runat="server" Text="Run OFAC Sync" OnClientClick="disablebutton(this,'OFAC');" OnClick="btnRunOFAC_Click" />
                    </div>
                    <div>
                        <asp:GridView runat="server" ID="gvOFAC" DataSourceID="dsOFAC" CssClass="tbl" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField HeaderText="Sr. No." DataField="rowID" />
                                <asp:BoundField HeaderText="SDN Record Count" DataField="sdn_records_affected" />
                                <asp:BoundField HeaderText="ADD Record Count" DataField="add_records_affected" />
                                <asp:BoundField HeaderText="ALT Record Count" DataField="alt_records_affected" />
                                <asp:BoundField HeaderText="Execution Time" DataField="execution_date" />
                                <asp:BoundField HeaderText="Status" DataField="add_update_status" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="dsOFAC" runat="server"
                            ConnectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=test"
                            SelectCommand="SELECT top 20 Row_Number() OVER(order by execution_date desc) as rowID, * FROM dbo.ofac_execution_log order by execution_date desc"></asp:SqlDataSource>
                        <%--<asp:SqlDataSource ID="dsOFAC" runat="server"
                            ConnectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=5Za{-p2N"
                            SelectCommand="SELECT top 20 Row_Number() OVER(order by execution_date desc) as rowID, * FROM dbo.ofac_execution_log order by execution_date desc"></asp:SqlDataSource>--%>
                    </div>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageView3">
                    <span class="heading">IRS Sync Status</span>
                    <div class="heading_border">
                    </div>
                    <div style="text-align: right; margin-bottom: 10px;">
                        <asp:Button ID="btnStartIRS" runat="server" Text="Start Scheduler" OnClick="btnStartIRS_Click" />
                        <asp:Button ID="btnRunIRS" runat="server" Text="Run IRS Sync" OnClick="btnRunIRS_Click" OnClientClick="disablebutton(this,'IRS');" />
                    </div>
                    <asp:GridView runat="server" ID="gvIRS" DataSourceID="dsIRS" AutoGenerateColumns="false" CssClass="tbl">
                        <Columns>
                            <asp:BoundField HeaderText="Sr. No." DataField="rowID" />
                            <asp:BoundField HeaderText="IRS Masterfile Version" DataField="IRSMasterFileVersion" />
                            <asp:BoundField HeaderText="Sync Date" DataField="loadDate" />
                            <asp:BoundField HeaderText="Sync By" DataField="loadedBy" />
                            <asp:BoundField HeaderText="Sync Note" DataField="loadNotes" />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="dsIRS" runat="server"
                        ConnectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=test"
                        SelectCommand="SELECT top 20 Row_Number() OVER(order by loadDate desc) as rowID, * FROM dbo.tblIRSMasterFileLoadHistory order by loadDate desc"></asp:SqlDataSource>
                    <%--<asp:SqlDataSource ID="dsIRS" runat="server"
                        ConnectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=5Za{-p2N"
                        SelectCommand="SELECT top 20 Row_Number() OVER(order by loadDate desc) as rowID, * FROM dbo.tblIRSMasterFileLoadHistory order by loadDate desc"></asp:SqlDataSource>--%>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageView4">
                    <span class="heading">Pub78 Sync Status</span>
                    <div class="heading_border"></div>
                    <div style="text-align: right; margin-bottom: 10px;">
                        <asp:Button ID="btnStartPub" runat="server" Text="Start Scheduler" OnClick="btnStartPub_Click" />
                        <asp:Button ID="btnRunPub" runat="server" Text="Run Pub78 Sync" OnClick="btnRunPub_Click" OnClientClick="disablebutton(this,'Pub78');" />
                    </div>
                    <asp:GridView runat="server" ID="gvPUb78" DataSourceID="dsgvPUb78" AutoGenerateColumns="false" CssClass="tbl">
                        <Columns>
                            <asp:BoundField HeaderText="Sr. No." DataField="rowID" />
                            <asp:BoundField HeaderText="Pub78 Version" DataField="Pub78Version" />
                            <asp:BoundField HeaderText="Sync Date" DataField="loadDate" />
                            <asp:BoundField HeaderText="Sync By" DataField="loadedBy" />
                            <asp:BoundField HeaderText="Sync Note" DataField="loadNotes" />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="dsgvPUb78" runat="server"
                        ConnectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=test"
                        SelectCommand="SELECT top 20 Row_Number() OVER(order by loadDate desc) as rowID, * FROM dbo.tblPub78LoadHistory order by loadDate desc"></asp:SqlDataSource>
                    <%--<asp:SqlDataSource ID="dsgvPUb78" runat="server"
                        ConnectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaxStatusVerification;User ID=sa;Password=5Za{-p2N"
                        SelectCommand="SELECT top 20 Row_Number() OVER(order by loadDate desc) as rowID, * FROM dbo.tblPub78LoadHistory order by loadDate desc"></asp:SqlDataSource>--%>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </div>
    </form>
</body>
</html>
