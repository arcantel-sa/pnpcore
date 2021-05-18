﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PnP.Core.Transformation.Services.Core
{
    /// <summary>
    /// Interface to implement in order to provide source items to transform
    /// </summary>
    public interface ISourceProvider
    {
        /// <summary>
        /// Gets the id of each available items
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<ISourceItemId> GetItemsIdsAsync(CancellationToken token = default);

        /// <summary>
        /// Get the item and related information based on its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ISourceItem> GetItemAsync(ISourceItemId id);
    }
}
