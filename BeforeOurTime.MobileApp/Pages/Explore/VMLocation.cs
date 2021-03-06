﻿using Autofac;
using BeforeOurTime.MobileApp.Pages.Explore.Models;
using BeforeOurTime.MobileApp.Services.Messages;
using BeforeOurTime.Models.Exceptions;
using BeforeOurTime.Models.Messages;
using BeforeOurTime.Models.Modules.Core.ItemProperties.Visibles;
using BeforeOurTime.Models.Modules.Core.Messages.ItemCrud.CreateItem;
using BeforeOurTime.Models.Modules.Core.Messages.ItemCrud.DeleteItem;
using BeforeOurTime.Models.Modules.Core.Messages.MoveItem;
using BeforeOurTime.Models.Modules.Core.Models.Items;
using BeforeOurTime.Models.Modules.World.ItemProperties.Characters;
using BeforeOurTime.Models.Modules.World.ItemProperties.Exits;
using BeforeOurTime.Models.Modules.World.ItemProperties.Locations.Messages.ReadLocationSummary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BeforeOurTime.MobileApp.Pages.Explore
{
    public delegate void VMLocationItemSelected(Item item);
    /// <summary>
    /// View model for location
    /// </summary>
    public class VMLocation : BotViewModel, System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Depedency injection container
        /// </summary>
        private IContainer Container { set; get; }
        /// <summary>
        /// Raised when an item has been selected
        /// </summary>
        public event VMLocationItemSelected OnItemSelected;
        /// <summary>
        /// Location item
        /// </summary>
        public Item Item { set; get; }
        /// <summary>
        /// Items at location
        /// </summary>
        public List<Item> Items
        {
            get { return _items; }
            set { _items = value; NotifyPropertyChanged("Items"); }
        }
        private List<Item> _items = new List<Item>();
        /// <summary>
        /// Player's character item id
        /// </summary>
        private Guid PlayerId { set; get; }
        /// <summary>
        /// Title of location
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }
        private string _name { set; get; }
        /// <summary>
        /// Description of location
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
        }
        private string _description { set; get; }
        /// <summary>
        /// Description of location including items present and hypertext
        /// </summary>
        public FormattedString DescriptionFormatted
        {
            get { return _descriptionFormatted; }
            set { _descriptionFormatted = value; NotifyPropertyChanged("DescriptionFormatted"); }
        }
        private FormattedString _descriptionFormatted { set; get; }
        /// <summary>
        /// List of items at location and their associated formatted string spans
        /// </summary>
        private List<ItemSpan> ItemSpans { set; get; } = new List<ItemSpan>();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Dependency injection container</param>
        public VMLocation(
            IContainer container, 
            Guid playerId)
        {
            Container = container;
            PlayerId = playerId;
        }
        /// <summary>
        /// Listen to unprompted incoming messages (events)
        /// </summary>
        /// <param name="message"></param>
        public void OnMessage(IMessage message)
        {
            // Location has changed
            if (message.IsMessageType<WorldReadLocationSummaryResponse>())
            {
                var msg = message.GetMessageAsType<WorldReadLocationSummaryResponse>();
                Items = new List<Item>();
                Item = msg.Item;
                Add(msg.Items);
                Set(Item, Items);
            }
            // Item has been created
            if (message.IsMessageType<CoreCreateItemCrudEvent>())
            {
                var msg = message.GetMessageAsType<CoreCreateItemCrudEvent>();
                Add(new List<Item>() { msg.Item });
                Set(Item, Items);
            }
            // Item has been deleted
            if (message.IsMessageType<CoreDeleteItemCrudEvent>())
            {
                var msg = message.GetMessageAsType<CoreDeleteItemCrudEvent>();
                Remove(msg.Items);
                Set(Item, Items);
            }
            // Item has moved (arrived or departed)
            if (message.IsMessageType<CoreMoveItemEvent>())
            {
                var msg = message.GetMessageAsType<CoreMoveItemEvent>();
                if (msg.NewParent.Id == Item.Id && msg.Item.Id != PlayerId)
                {
                    // Item has arrived
                    Add(new List<Item>() { msg.Item });
                    Set(Item, Items);
                }
                if (msg.OldParent.Id == Item.Id && msg.Item.Id != PlayerId)
                {
                    // Item has departed
                    Remove(new List<Item>() { msg.Item });
                    Set(Item, Items);
                }
            }
        }
        /// <summary>
        /// Update view model to reflect new location
        /// </summary>
        /// <param name="item">location item</param>
        /// <param name="children">Children items at location</param>
        /// <returns></returns>
        public VMLocation Set(Item location, List<Item> children = null)
        {
            Item = location;
            Items = children.ToList();
            Name = Item.GetProperty<VisibleItemProperty>()?.Name ?? "**Unknown**";
            Description = Item.GetProperty<VisibleItemProperty>()?.Description ?? "**Unknown**";
            var buildingDescription = new FormattedString();
            buildingDescription.Spans.Add(new Span() { Text = $"{Description}. " });
            DescriptionFormatted = buildingDescription;
            return this;
        }
        /// <summary>
        /// Add items to the locally tracked items on ground
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public VMLocation Add(List<Item> items)
        {
            items.ForEach(item =>
            {
                if (item.HasProperty<VisibleItemProperty>())
                {
                    var newItems = Items.ToList();
                    newItems.Add(item);
                    Items = newItems.ToList();
                }
            });
            return this;
        }
        /// <summary>
        /// Remove items from the locally tracked items on ground
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public VMLocation Remove(List<Item> items)
        {
            Items.RemoveAll(x => items.Select(y => y.Id).ToList().Contains(x.Id));
            return this;
        }
    }
}
