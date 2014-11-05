using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC.CustomFieldSearch.Commands
{
    public class CustomFieldSearchCommand : Command
    {
        // Methods
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            UrlString str = new UrlString();
           
            str["itemID"] = context.Items[0].ID.ToString();
            Windows.RunApplication("Custom Field Search/Search", str.ToString());
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (!base.IsAdvancedClient())
            {
                return CommandState.Hidden;
            }
            if (Context.Database.GetItem("/sitecore/content/Applications/Custom Field Search/Search") == null)
            {
                return CommandState.Hidden;
            }
            return base.QueryState(context);
        }

    }
}