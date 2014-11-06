<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomFieldSearch.aspx.cs" Inherits="SC.CustomFieldSearch.sitecore.shell.Applications.Custom_Field_Search.CustomFieldSearch" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="Sitecore.Kernel" Namespace="Sitecore.Web.UI.HtmlControls" TagPrefix="sc" %>
<%@ Register Assembly="Sitecore.Kernel" Namespace="Sitecore.Web.UI.WebControls" TagPrefix="sc" %>
<%@ Register Assembly="Sitecore.Kernel" Namespace="Sitecore.Web.UI.WebControls.Ribbons" TagPrefix="sc" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head runat="server">
  <title>Sitecore</title>
  <sc:Stylesheet Src="Content Manager.css" DeviceDependant="true" runat="server" />
  <sc:Stylesheet Src="Ribbon.css" DeviceDependant="true" runat="server" />
  <sc:Stylesheet Src="Grid.css" DeviceDependant="true" runat="server" />
  <sc:Script Src="/sitecore/shell/Controls/InternetExplorer.js" runat="server"/>
  <sc:Script Src="/sitecore/shell/Controls/Sitecore.js" runat="server" />
  <sc:Script Src="/sitecore/shell/Controls/SitecoreObjects.js" runat="server" />
  <sc:Script Src="/sitecore/shell/Applications/Content Manager/Content Editor.js" runat="server" />  
  <style type="text/css">    
    html body
    {
      overflow: hidden;
    }

    .loadingPnl {
      width: 100%; height: 100%; background-color: #808080; 
      top: 0px; left: 0px; position: absolute; z-index: 9999;
      opacity: 0.4;
      filter: alpha(opacity=40);
     }
  </style>



  
</head>
<body style="background:transparent; height: 100%" id="PageBody" runat="server">
  <form id="customFieldSearchForm" runat="server">
    <sc:AjaxScriptManager runat="server"/>
    <sc:ContinuationManager runat="server" />
    
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

          

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false" DisplayAfter="1000">
        <ProgressTemplate>
             <table cellspacing="0" cellpadding="0" border="0" width="100%" class="loadingPnl">
                <tr>
                    <td style="font-size: 12px; text-align: center;"><img src="/sitecore/shell/themes/standard/componentart/grid/ajax-loader.gif"></td>
                </tr>
            </table>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
              <tr>
                 <td id="GridCell" height="100%" valign="top" style="background:#e9e9e9">
		                    
                                            <p style="padding-top: 15px; padding-bottom: 15px; padding-left: 8px; font-weight: bold;">
                                                Please enter the field name and the value you are searching for.
                                            </p>

	                                        <table>
                                               <tr>
                                                   <td colspan="2" style="padding-top: 5px; padding-bottom: 5px;">Searching in: <b><asp:Label ID="lblItemPath" runat="server"></asp:Label></b><br /></td>
                                               </tr>
                                               <tr>
                                                   <td>Field Name: </td>
                                                   <td><asp:TextBox ID="txtFieldName" runat="server" /></td>
                                               </tr>
                                               <tr>
                                                   <td>Field Value: </td>
                                                   <td><asp:TextBox ID="txtFieldValue" runat="server"></asp:TextBox></td>
                                               </tr>
                                               <tr>
                                                   <td>&nbsp;</td>
                                                   <td><asp:CheckBox ID="chkContains" runat="server" /> Partial Lookup</td>
                                               </tr>
                                               <tr>
                                                   <td>&nbsp;</td>
                                                   <td><asp:CheckBox ID="chkCaseSensitive" runat="server" /> Case Insensitive</td>
                                               </tr>
                                               <tr>
                                                   <td>&nbsp;</td>
                                                   <td><asp:Button ID="btnSearch" runat="server" Text=" Search " OnClick="Search_Click" /></td>
                                               </tr>
                                           </table>
	               

                                            <asp:Panel ID="pnlNoItem" runat="server" Visible="false">

                                                <p style="padding-top: 15px; padding-bottom: 15px; padding-left: 8px; font-weight: bold;">
                                                    <asp:Label ID="lblMsg" runat="server"></asp:Label>
                                                </p>

                                            </asp:Panel>

                       <sc:Border runat="server" ID="GridContainer">
                            <ComponentArt:Grid ID="Grid1"
                                CssClass="Grid"
                                RunningMode="Server"
                                HeaderCssClass="GridHeader" 
                                FooterCssClass="GridFooter"
                                PagerStyle="Slider"
                                PagerImagesFolderUrl="/sitecore/shell/themes/standard/componentart/grid/pager/"
                                PagerTextCssClass="GridFooterText"
                                PageSize="10"
                                ImagesBaseUrl="/sitecore/shell/themes/standard/componentart/grid/"
                                Width="100%" Height="100%"
                                runat="server">


                                <Levels>
                                    <ComponentArt:GridLevel 
                                            DataKeyField="scGridID"
                                            ShowTableHeading="false" 
                                            ShowSelectorCells="false" 
                                            RowCssClass="Row" 
                                            ColumnReorderIndicatorImageUrl="reorder.gif"
                                            DataCellCssClass="DataCell" 
                                            HeadingCellCssClass="HeadingCell" 
                                            HeadingCellHoverCssClass="HeadingCellHover" 
                                            HeadingCellActiveCssClass="HeadingCellActive" 
                                            HeadingRowCssClass="HeadingRow" 
                                            HeadingTextCssClass="HeadingCellText"
                                            SelectedRowCssClass="SelectedRow"
                                            GroupHeadingCssClass="GroupHeading" 
                                            SortAscendingImageUrl="asc.gif" 
                                            SortDescendingImageUrl="desc.gif" 
                                            SortImageWidth="13" 
                                            SortImageHeight="13">
                                         <Columns>
                                             <ComponentArt:GridColumn DataField="Name" HeadingText="Item Name" Width="500" />
                                             <ComponentArt:GridColumn DataField="Paths.FullPath" HeadingText="Item Path"  />
                                         </Columns>
                                    </ComponentArt:GridLevel>
                                </Levels>

                                 <ClientTemplates>
                                   
                   
              
                                  <ComponentArt:ClientTemplate Id="SliderTemplate">
                                    <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                                      <tr>
                                        <td><div style="padding:4px;font:8pt tahoma;white-space:nowrap;overflow:hidden">## DataItem.GetMember('Name').Value ##</div></td>
                                      </tr>
                                      <tr>
                                        <td style="height:14px;background-color:#666666;padding:1px 8px 1px 8px;color:white">
                                        ## DataItem.PageIndex + 1 ## / ## Users.PageCount ##
                                        </td>
                                      </tr>
                                    </table>
                                  </ComponentArt:ClientTemplate>
                                </ClientTemplates>
                            </ComponentArt:Grid>

                       </sc:Border>
                  </td>

              </tr>
            </table>
            </ContentTemplate>
       
    </asp:UpdatePanel>


               
          

      
  </form>
</body>
</html>
