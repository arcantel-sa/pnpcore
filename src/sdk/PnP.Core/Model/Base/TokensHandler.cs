﻿using PnP.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PnP.Core.Model
{
    /// <summary>
    /// Handler class to help managing tokens replacement
    /// </summary>
    public static class TokensHandler
    {
        /// <summary>
        /// Method to resolve a set of tokens in a provided tokenized string
        /// </summary>
        /// <param name="tokenizedValue">A string with tokens</param>
        /// <param name="pnpObject">The domain model object to use as the target reference</param>
        /// <returns>The string with tokens resolved</returns>
        public static async Task<string> ResolveTokens(IMetadataExtensible pnpObject, string tokenizedValue)
        {
            // Define the result variable
            string result = tokenizedValue;

            // Get the context aware version of the target pnpObject
            var contextAwareObject = pnpObject as IDataModelWithContext;

            // Grab the tokens in this input (tokens are between curly braces)
            var regex = new Regex("{(.*?)}", RegexOptions.Compiled);
            var matches = regex.Matches(tokenizedValue);

            // Iterate over the tokens and replace them
            foreach (Match match in matches)
            {
                // Replace {Id}
                if (match.Value.Equals("{Id}"))
                {
                    var model = pnpObject;

                    if (model.Metadata.ContainsKey(PnPConstants.MetaDataRestId))
                    {
                        result = result.Replace("{Id}", model.Metadata[PnPConstants.MetaDataRestId]);
                    }
                }

                // Replace {Parent.Id}
                if (match.Value.Equals("{Parent.Id}"))
                {
                    // there's either a collection object inbetween (e.g. ListItem --> ListItemCollection --> List), so take the parent of the parent
                    // or
                    // the parent is model class itself (e.g. Web --> Site.RootWeb)
                    if (!((pnpObject as IDataModelParent).Parent is IMetadataExtensible parent))
                    {
                        // Parent is a collection, so jump one level up
                        parent = (pnpObject as IDataModelParent).Parent.Parent as IMetadataExtensible;
                    }

                    // Ensure the parent object
                    if (parent != null)
                    {
                        await ((IDataModelParent)pnpObject).EnsureParentObjectAsync().ConfigureAwait(true);
                    }

                    if (parent.Metadata.ContainsKey(PnPConstants.MetaDataRestId))
                    {
                        result = result.Replace("{Parent.Id}", parent.Metadata[PnPConstants.MetaDataRestId]);
                    }
                }

                // Replace {GraphId}
                if (match.Value.Equals("{GraphId}"))
                {
                    var model = pnpObject;

                    if (model.Metadata.ContainsKey(PnPConstants.MetaDataGraphId))
                    {
                        result = result.Replace("{GraphId}", model.Metadata[PnPConstants.MetaDataGraphId]);
                    }
                }

                // Replace {Parent.GraphId}
                if (match.Value.Equals("{Parent.GraphId}"))
                {
                    // there's either a collection object inbetween (e.g. TeamChannel --> TeamChannelCollection --> Team), so take the parent of the parent
                    // or
                    // the parent is model class itself (e.g. TeamChannel --> Team.PrimaryChannel)
                    if (!((pnpObject as IDataModelParent).Parent is IMetadataExtensible parent))
                    {
                        // Parent is a collection, so jump one level up
                        parent = (pnpObject as IDataModelParent).Parent.Parent as IMetadataExtensible;
                    }

                    // Ensure the parent object
                    if (parent != null)
                    {
                        await ((IDataModelParent)parent).EnsureParentObjectAsync().ConfigureAwait(true);
                    }

                    if (parent.Metadata.ContainsKey(PnPConstants.MetaDataGraphId))
                    {
                        result = result.Replace("{Parent.GraphId}", parent.Metadata[PnPConstants.MetaDataGraphId]);
                    }
                }

                // Replace tokens coming from the Site object connected to the current PnPContext
                if (match.Value.StartsWith("{Site."))
                {
                    var propertyToLoad = match.Value.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("}", "");

                    switch (propertyToLoad)
                    {
                        case "GroupId":
                            {
                                contextAwareObject.PnPContext.Site.EnsurePropertiesAsync(p => p.GroupId).Wait();
                                if (contextAwareObject.PnPContext.Site.HasValue(propertyToLoad))
                                {
                                    result = result.Replace(match.Value, contextAwareObject.PnPContext.Site.GroupId.ToString());
                                }
                                break;
                            }
                        case "Id":
                            {
                                contextAwareObject.PnPContext.Site.EnsurePropertiesAsync(p => p.Id).Wait();
                                if (contextAwareObject.PnPContext.Site.HasValue(propertyToLoad))
                                {
                                    result = result.Replace(match.Value, contextAwareObject.PnPContext.Site.Id.ToString());
                                }
                                break;
                            }
                    }
                }

                // Replace tokens coming from the Site object connected to the current PnPContext
                if (match.Value.StartsWith("{Web."))
                {
                    var propertyToLoad = match.Value.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("}", "");

                    switch (propertyToLoad)
                    {
                        case "Id":
                            {
                                contextAwareObject.PnPContext.Web.EnsurePropertiesAsync(p => p.Id).Wait();
                                if (contextAwareObject.PnPContext.Web.HasValue(propertyToLoad))
                                {
                                    result = result.Replace(match.Value, contextAwareObject.PnPContext.Web.Id.ToString());
                                }
                                break;
                            }
                        case "GraphId":
                            {
                                var model = contextAwareObject.PnPContext.Web as IMetadataExtensible;

                                if (model.Metadata.ContainsKey(PnPConstants.MetaDataGraphId))
                                {
                                    result = result.Replace("{Web.GraphId}", model.Metadata[PnPConstants.MetaDataGraphId]);
                                }
                                break;
                            }
                    }
                }
            }

            return result;
        }
    }
}
