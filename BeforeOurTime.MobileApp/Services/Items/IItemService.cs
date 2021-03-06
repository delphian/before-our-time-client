﻿using BeforeOurTime.Models.Modules.Core.Messages.ItemGraph;
using BeforeOurTime.Models.Modules.Core.Messages.ItemJson;
using BeforeOurTime.Models.Modules.Core.Models.Items;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeforeOurTime.MobileApp.Services.Items
{
    /// <summary>
    /// Item service for CRUD operations
    /// </summary>
    public interface IItemService : IService
    {
        /// <summary>
        /// Create multiple items from JSON string
        /// </summary>
        /// <param name="json">String of json to create items from</param>
        /// <param name="recursive">Recursively include all children items in json response</param>
        /// <returns></returns>
        Task<List<CoreItemJson>> CreateJsonAsync(
            string json,
            bool recursive = false);
        /// <summary>
        /// Read multiple items based on a list of unique item identifiers
        /// </summary>
        /// <param name="itemIds">List of unique item identifiers</param>
        /// <returns></returns>
        Task<List<Item>> ReadAsync(List<Guid> itemIds = null);
        /// <summary>
        /// Read JSON of multiple items based on a list of unique item identifiers
        /// </summary>
        /// <param name="itemIds">List of unique item identifiers</param>
        /// <param name="recursive">Recursively include all children items in json response</param>
        /// <returns></returns>
        Task<List<CoreItemJson>> ReadJsonAsync(
            List<Guid> itemIds = null,
            bool recursive = false);
        /// <summary>
        /// Read item graph
        /// </summary>
        /// <param name="itemIds">Item to begin graph with</param>
        /// <returns></returns>
        Task<ItemGraph> ReadGraphAsync(Guid? itemId = null);
        /// <summary>
        /// Read multiple items based on a list of item types
        /// </summary>
        /// <param name="itemTypes">List of unique item type names (class names)</param>
        /// <returns></returns>
        Task<List<Item>> ReadByDataTypeAsync(List<string> itemDataTypes);
        /// <summary>
        /// Update multiple items
        /// </summary>
        /// <param name="items">List of items to update</param>
        /// <returns></returns>
        /// <exception cref="Exception">Unable to update items</exception>
        Task UpdateAsync(List<Item> items);
        /// <summary>
        /// Update multiple items through json objects
        /// </summary>
        /// <param name="itemsJson">List of items to update</param>
        /// <returns></returns>
        /// <exception cref="Exception">Unable to update items</exception>
        Task UpdateJsonAsync(List<CoreItemJson> itemsJson);
        /// <summary>
        /// Delete multiple items
        /// </summary>
        /// <param name="itemIds">List of unique item identifiers to delete</param>
        /// <returns></returns>
        /// <exception cref="Exception">Unable to delete items</exception>
        Task DeleteAsync(List<Guid> itemIds);
    }
}
