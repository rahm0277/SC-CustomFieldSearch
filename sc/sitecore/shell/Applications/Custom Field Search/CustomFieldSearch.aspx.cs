using ComponentArt.Web.UI;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Extensions;
using Sitecore.Resources;
using Sitecore.Security;
using Sitecore.Security.Accounts;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Grids;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using Sitecore.Web.UI.XamlSharp.Ajax;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Linq;

namespace SC.CustomFieldSearch.sitecore.shell.Applications.Custom_Field_Search
{
    public partial class CustomFieldSearch : System.Web.UI.Page, IHasCommandContext
    {
        /*
        protected override void OnInit(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnInit(e);

        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
        }
        */

        private Item searchItemRoot { get; set; }
        private bool RebindRequired { get; set; }

       
        protected void Page_Init(object sender, EventArgs e)
        {
            Grid1.PageIndexChanged += new ComponentArt.Web.UI.Grid.PageIndexChangedEventHandler(OnPageIndexChanged);
            Grid1.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(OnNeedDataSource);
            Grid1.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(OnNeedRebind);
            
        }

        public void OnPageIndexChanged(object sender, ComponentArt.Web.UI.GridPageIndexChangedEventArgs args)
        {
            Grid1.CurrentPageIndex = args.NewIndex;
        }

        public void OnNeedDataSource(object sender, EventArgs oArgs)
        {
            Grid1.DataSource = buildGridData();
        }

        public void OnNeedRebind(object sender, System.EventArgs oArgs)
        {
            Grid1.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlNoItem.Visible = false;
            CommandContext context = GetCommandContext();

            string itemID = context.Parameters["itemid"].ToString();

            Database masterDB = Sitecore.Configuration.Factory.GetDatabase("master");
            searchItemRoot = masterDB.GetItem(new Sitecore.Data.ID(itemID));
            lblItemPath.Text = searchItemRoot.Paths.FullPath;

           
        }

        private void GetItems(Item item, string fieldName, string fieldValue, List<Item> itemList, bool containsClause)
        {
            if(containsClause)
            {
                var items = from i in item.Children.ToArray() where i.Fields[fieldName] != null && i.Fields[fieldName].ToString().Contains(fieldValue) select i;

                if (items.ElementAtOrDefault(0) != null)
                {
                    itemList.AddRange(items.ToList());
                }

            }
            else
            {
                var items = from i in item.Children.ToArray() where i.Fields[fieldName] != null && i.Fields[fieldName].ToString() == fieldValue select i;

                if (items.ElementAtOrDefault(0) != null)
                {
                    itemList.AddRange(items.ToList());
                }
            }
         

            foreach(Item i in item.Children)
            {
                GetItems(i, fieldName, fieldValue, itemList, containsClause);
            }
        }

        private List<Item> buildGridData()
        {
            RebindRequired = true;
            Database masterDB = Sitecore.Configuration.Factory.GetDatabase("master");
            List<Item> itemList = new List<Item>();
            if (txtFieldName.Text.Trim() != "")
            {

                GetItems(searchItemRoot, txtFieldName.Text.Trim(), txtFieldValue.Text.Trim(), itemList, chkContains.Checked);

                if (itemList.Count == 0)
                {
                    pnlNoItem.Visible = true;
                    lblMsg.Text = "No items found.";
                }

            }


            return itemList;
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            if (txtFieldName.Text.Trim() == "")
            {
                pnlNoItem.Visible = true;
                lblMsg.Text = "Please enter field name.";

            }
            else
            {
                Grid1.DataSource = buildGridData();
                Grid1.DataBind();
            }
        }



        public CommandContext GetCommandContext()
        {
            string itemID = WebUtil.GetQueryString("itemID");
            CommandContext commandContext = new CommandContext();
            commandContext.Parameters["itemid"] = itemID;
            return commandContext;
        }
    }
}