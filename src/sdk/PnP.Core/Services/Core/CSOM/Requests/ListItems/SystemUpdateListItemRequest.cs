﻿namespace PnP.Core.Services.Core.CSOM.Requests.ListItems
{
    internal class SystemUpdateListItemRequest : UpdateListItemRequest
    {
        internal SystemUpdateListItemRequest(string siteId, string webId, string listId, int itemId) : base(siteId, webId, listId, itemId)
        {
            UpdateMethodName = "SystemUpdate";
        }
    }
}
