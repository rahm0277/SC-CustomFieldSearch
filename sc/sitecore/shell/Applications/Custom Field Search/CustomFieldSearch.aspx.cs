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
        private Item searchItemRoot { get; set; }
        private bool RebindRequired { get; set; }

       
        /// <summary>
        /// Setup the event handlers for the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            Grid1.PageIndexChanged += new ComponentArt.Web.UI.Grid.PageIndexChangedEventHandler(OnPageIndexChanged);
            Grid1.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(OnNeedDataSource);
            Grid1.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(OnNeedRebind);
            
        }

        /// <summary>
        /// Event handler method to handle when a paging button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnPageIndexChanged(object sender, ComponentArt.Web.UI.GridPageIndexChangedEventArgs args)
        {
            Grid1.CurrentPageIndex = args.NewIndex;
        }

        /// <summary>
        /// Handler method for when the Grid needs a datasource to bind to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oArgs"></param>
        public void OnNeedDataSource(object sender, EventArgs oArgs)
        {
            Grid1.DataSource = buildGridData();
        }

        /// <summary>
        /// Handler method for when the grid needs to rebind the datasource (for reloads or paging events)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oArgs"></param>
        public void OnNeedRebind(object sender, System.EventArgs oArgs)
        {
            Grid1.DataBind();
        }


        /// <summary>
        /// Page_load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            pnlNoItem.Visible = false;
            CommandContext context = GetCommandContext();

            string itemID = context.Parameters["itemid"].ToString();

            Database db = Sitecore.Configuration.Factory.GetDatabase("master");
            searchItemRoot = db.GetItem(new Sitecore.Data.ID(itemID));
            lblItemPath.Text = searchItemRoot.Paths.FullPath;

           
        }

        /// <summary>
        /// Method to recursively get the item that matches the field value and field name being passed
        /// TODO: see if we can use system lucene indexes for this
        /// TODO: see if xpath will be faster here
        /// </summary>
        /// <param name="item">The root item where to start the search</param>
        /// <param name="fieldName">The field name being searched on</param>
        /// <param name="fieldValue">The field value being searched on</param>
        /// <param name="itemList">The itemList is the final list of items that will get populated as a result of the search</param>
        /// <param name="containsClause">Use a contains clause instead of a == clause</param>
        /// <param name="caseInsensitive">Use ToLower(), i.e. make the lookup case insensitive</param>
        private void GetItems(Item item, string fieldName, string fieldValue, List<Item> itemList, bool containsClause, bool caseInsensitive)
        {
            var items = (IEnumerable<Item>)null;
            if(containsClause)
            {
                if (caseInsensitive)
                    items = from i in item.Children.ToArray() where i.Fields[fieldName] != null && i.Fields[fieldName].ToString().ToLower().Contains(fieldValue.ToLower()) select i;
                else
                    items = from i in item.Children.ToArray() where i.Fields[fieldName] != null && i.Fields[fieldName].ToString().Contains(fieldValue) select i;
            }
            else
            {
                if(caseInsensitive)
                    items = from i in item.Children.ToArray() where i.Fields[fieldName] != null && i.Fields[fieldName].ToString().ToLower() == fieldValue.ToLower() select i;
                else
                    items = from i in item.Children.ToArray() where i.Fields[fieldName] != null && i.Fields[fieldName].ToString() == fieldValue select i;
            }

            if (items.ElementAtOrDefault(0) != null)
            {
                itemList.AddRange(items.ToList());
            }

            foreach(Item i in item.Children)
            {
                GetItems(i, fieldName, fieldValue, itemList, containsClause, caseInsensitive);
            }
        }

        /// <summary>
        /// Method to build the list of items for the Grid - also the method used in NeedDataSource grid event handler
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Item> buildGridData()
        {
            RebindRequired = true;
            Database masterDB = Sitecore.Configuration.Factory.GetDatabase("master");
            List<Item> itemList = new List<Item>();
            if (txtFieldName.Text.Trim() != "")
            {

                GetItems(searchItemRoot, txtFieldName.Text.Trim(), txtFieldValue.Text.Trim(), itemList, chkContains.Checked, chkCaseSensitive.Checked);

                if (itemList.ToList().Count == 0)
                {
                    pnlNoItem.Visible = true;
                    lblMsg.Text = "No items found.";
                }

            }


            return itemList;
        }


        /// <summary>
        /// Handler method for the search button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Search_Click(object sender, EventArgs e)
        {
            if (txtFieldName.Text.Trim() == "")
            {
                pnlNoItem.Visible = true;
                lblMsg.Text = "Please enter field name.";

            }
            else
            {
                Grid1.CurrentPageIndex = 0;
                Grid1.DataSource = buildGridData();
                Grid1.DataBind();
            }
        }


        /// <summary>
        /// Get the command context that this application was run from
        /// </summary>
        /// <returns></returns>
        public CommandContext GetCommandContext()
        {
            string itemID = WebUtil.GetQueryString("itemID");
            CommandContext commandContext = new CommandContext();
            commandContext.Parameters["itemid"] = itemID;
            return commandContext;
        }
    }
}