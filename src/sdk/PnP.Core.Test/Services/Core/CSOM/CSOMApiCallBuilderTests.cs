﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PnP.Core.Services;
using PnP.Core.Services.Core.CSOM;
using PnP.Core.Services.Core.CSOM.Requests.ListItems;
using PnP.Core.Services.Core.CSOM.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnP.Core.Test.Services.Core.CSOM
{
    [TestClass]
    public class CSOMApiCallBuilderTests
    {
        [TestMethod]
        public void CSOMApiCallBuilder_Test_BuildUpdateItemApiCall()
        {
            UpdateListItemRequest request = new UpdateListItemRequest("test-site-id", "test-web-id", "test-list-id", 1);
            request.FieldsToUpdate = new List<CSOMItemField>()
            {
                new CSOMItemField()
                {
                    FieldName = "Test Field",
                    FieldType = "String",
                    FieldValue = "Test field value"
                }
            };
            CSOMApiCallBuilder builder = new CSOMApiCallBuilder();
            builder.AddRequest(request);
            ApiCall call = builder.BuildApiCall();
            Assert.AreEqual("<Request AddExpandoFieldTypeSuffix=\"true\" SchemaVersion=\"15.0.0.0\" LibraryVersion=\"16.0.0.0\" ApplicationName=\"pnp core sdk\" xmlns=\"http://schemas.microsoft.com/sharepoint/clientquery/2009\"><Actions><Method Name=\"SetFieldValue\" Id=\"1\" ObjectPathId=\"2\"><Parameters><Parameter Type=\"String\">Test Field</Parameter><Parameter Type=\"String\">Test field value</Parameter></Parameters></Method><Method Name=\"Update\" Id=\"1\" ObjectPathId=\"2\"></Method></Actions><ObjectPaths><Identity Id=\"2\" Name=\"121a659f-e03e-2000-4281-1212829d67dd|740c6a0b-85e2-48a0-a494-e0f1759d4aa7:site:test-site-id:web:test-web-id:list:test-list-id:item:1,1\" /></ObjectPaths></Request>", call.XmlBody);

            Assert.AreEqual(ApiType.CSOM, call.Type);

        }
    }
}
