﻿using Autofac;
using BeforeOurTime.MobileApp.Services.Games;
using BeforeOurTime.Models.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using BeforeOurTime.MobileApp.Services.Characters;
using BeforeOurTime.Models.Modules.Core.Models.Items;
using BeforeOurTime.Models.Modules.Core.Messages.MoveItem;
using BeforeOurTime.MobileApp.Controls;
using System.Windows.Input;
using BeforeOurTime.Models.Modules.Core.Models.Properties;
using System.Threading.Tasks;
using BeforeOurTime.MobileApp.Services.Messages;
using BeforeOurTime.Models.Modules.Account.Models.Data;
using BeforeOurTime.Models.Modules.World.ItemProperties.Locations.Messages.ReadLocationSummary;
using BeforeOurTime.Models.Modules.Core.ItemProperties.Visibles;
using BeforeOurTime.Models.Exceptions;
using BeforeOurTime.Models.Modules.Core.Messages.ItemCrud.ReadItem;
using BeforeOurTime.Models.Modules.Core.Messages.UseItem;
using BeforeOurTime.Models.Modules.World.ItemProperties.Exits;
using BeforeOurTime.MobileApp.Services.Items;
using BeforeOurTime.MobileApp.Pages.Admin.JsonEditor;

namespace BeforeOurTime.MobileApp.Pages.Explore
{
    public class VMExplorePage : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Structure that subscriber must implement to recieve property updates
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Depedency injection container
        /// </summary>
        public IContainer Container
        {
            get { return _container; }
            set { _container = value; NotifyPropertyChanged("Container"); }
        }
        private IContainer _container { set; get; }
        /// <summary>
        /// Message service
        /// </summary>
        private IMessageService MessageService { set; get; }
        /// <summary>
        /// Page is working on a process
        /// </summary>
        public bool Working
        {
            get { return _working; }
            set { _working = value; NotifyPropertyChanged("Working"); }
        }
        private bool _working { set; get; }
        /// <summary>
        /// Page that this code behind is acting as view model for
        /// </summary>
        public Page Page { set; get; }
        /// <summary>
        /// Player's character
        /// </summary>
        public Item Me { set; get; }
        /// <summary>
        /// Background Image
        /// </summary>
        public BeforeOurTime.Models.Primitives.Images.Image BackgroundImage
        {
            get { return _backgroundImage; }
            set { _backgroundImage = value; NotifyPropertyChanged("BackgroundImage"); }
        }
        private BeforeOurTime.Models.Primitives.Images.Image _backgroundImage { set; get; }
        /// <summary>
        /// Player's inventory
        /// </summary>
        public VMInventory Inventory
        {
            get { return _inventory; }
            set { _inventory = value; NotifyPropertyChanged("Inventory"); }
        }
        private VMInventory _inventory { set; get; }
        /// <summary>
        /// Show player inventory or location items
        /// </summary>
        public bool ShowInventory
        {
            get { return _showInventory; }
            set { _showInventory = value; NotifyPropertyChanged("ShowInventory"); }
        }
        private bool _showInventory { set; get; } = false;
        /// <summary>
        /// Current account has administrative permissions
        /// </summary>
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; NotifyPropertyChanged("IsAdmin"); }
        }
        private bool _isAdmin { set; get; }
        /// <summary>
        /// Current location view model
        /// </summary>
        public VMLocation VMLocation
        {
            get { return _vmLocation; }
            set { _vmLocation = value; NotifyPropertyChanged("VMLocation"); }
        }
        private VMLocation _vmLocation { set; get; }
        /// <summary>
        /// Callback when location item has been clicked
        /// </summary>
        public ICommand LocationItemTable_OnClicked
        {
            get { return _locationItemTable_OnClicked; }
            set { _locationItemTable_OnClicked = value; NotifyPropertyChanged("LocationItemTable_OnClicked"); }
        }
        private ICommand _locationItemTable_OnClicked { set; get; }
        /// <summary>
        /// Callback when an item command has been invoked by child control
        /// </summary>
        public ICommand ItemDescriptions_ItemOnCommand
        {
            get { return _itemDescriptions_ItemOnCommand; }
            set { _itemDescriptions_ItemOnCommand = value; NotifyPropertyChanged("ItemDescriptions_ItemOnCommand"); }
        }
        private ICommand _itemDescriptions_ItemOnCommand { set; get; }
        /// <summary>
        /// Callback when an item's selected status has changed by child control
        /// </summary>
        public ICommand ItemDescriptions_ItemOnSelect
        {
            get { return _itemDescriptions_ItemOnSelect; }
            set { _itemDescriptions_ItemOnSelect = value; NotifyPropertyChanged("ItemDescriptions_ItemOnSelect"); }
        }
        private ICommand _itemDescriptions_ItemOnSelect { set; get; }
        /// <summary>
        /// Currently selected item at location
        /// </summary>
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; NotifyPropertyChanged("SelectedItem"); }
        }
        private Item _selectedItem { set; get; }
        /// <summary>
        /// An item is selected
        /// </summary>
        public bool IsItemSelected
        {
            get { return _isItemSelected; }
            set { _isItemSelected = value; NotifyPropertyChanged("IsItemSelected"); }
        }
        private bool _isItemSelected { set; get; }
        /// <summary>
        /// Visible property of currently selected item at location
        /// </summary>
        public VisibleItemProperty SelectedVisibleProperty
        {
            get { return _selectedVisibleProperty; }
            set { _selectedVisibleProperty = value; NotifyPropertyChanged("SelectedVisibleProperty"); }
        }
        private VisibleItemProperty _selectedVisibleProperty { set; get; }
        /// <summary>
        /// Selected item commands
        /// </summary>
        public List<ItemCommand> Commands
        {
            get { return _commands; }
            set { _commands = value; NotifyPropertyChanged("Commands"); }
        }
        private List<ItemCommand> _commands { set; get; }
        /// <summary>
        /// Last message recieved from server in it's raw format
        /// </summary>
        public VMEventStream EventStream
        {
            get { return _eventStream; }
            set { _eventStream = value; NotifyPropertyChanged("EventStream"); }
        }
        private VMEventStream _eventStream { set; get; } = new VMEventStream();
        /// <summary>
        /// View model for all possible emotes
        /// </summary>
        public VMEmotes VMEmotes
        {
            get { return _vmEmotes; }
            set { _vmEmotes = value; NotifyPropertyChanged("VMEmotes"); }
        }
        private VMEmotes _vmEmotes { set; get; }
        /// <summary>
        /// View model to manage admin picker
        /// </summary>
        public VMAdminPicker VMAdminPicker
        {
            get { return _vmAdminPicker; }
            set { _vmAdminPicker = value; NotifyPropertyChanged("VMAdminPicker"); }
        }
        private VMAdminPicker _vmAdminPicker { set; get; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Dependency injection container</param>
        public VMExplorePage(IContainer container, Page page)
        {
            Container = container;
            Page = page;
            Me = Container.Resolve<ICharacterService>().GetCharacter();
            VMLocation = new VMLocation(Container, Me.Id);
            VMLocation.OnItemSelected += SelectItem;
            Inventory = new VMInventory(Container);
            Container.Resolve<IImageService>()
                .ReadAsync(new List<Guid>() { new Guid("a15e4ade-5fbe-4eb1-9d62-f1c1e67a207b") })
                .ContinueWith((task) =>
                {
                    var image = task.Result?.FirstOrDefault();
                    BackgroundImage = image;
                });
            if (Me.ChildrenIds.Count() > 0)
            {
                Container.Resolve<IMessageService>().SendRequestAsync<CoreReadItemCrudResponse>(new CoreReadItemCrudRequest()
                {
                    ItemIds = Me.ChildrenIds
                }).ContinueWith(messageTask =>
                {
                    Inventory.Add(messageTask.Result.CoreReadItemCrudEvent.Items);
                });
            }
            MessageService = Container.Resolve<IMessageService>();
            IsAdmin = Me.GetData<AccountData>().Admin;
            VMEmotes = new VMEmotes(Container);
            var GameService = container.Resolve<IGameService>();
            GameService.OnMessage += OnMessage;
            GameService.OnMessage += VMLocation.OnMessage;
            LocationItemTable_OnClicked = new Xamarin.Forms.Command((object itemTableControl) =>
            {
                var control = (ItemTableControl)itemTableControl;
                SelectItem(control?.SelectedItem);
//                control.SetHighlight(SelectedItem);
            });
            ItemDescriptions_ItemOnCommand = new Xamarin.Forms.Command((object itemCommand) =>
            {
                OnItemCommand((ItemCommand)itemCommand);
            });
            ItemDescriptions_ItemOnSelect = new Xamarin.Forms.Command((object itemDescriptionsControl) =>
            {
                var control = (BotItemDescriptionsControl)itemDescriptionsControl;
                SelectItem((control.ItemSelectedLastStatus) ? control.ItemSelectedLast : null);
            });
            VMAdminPicker = new VMAdminPicker(Container, page, VMLocation);
            MessageService.SendRequestAsync<WorldReadLocationSummaryResponse>(new WorldReadLocationSummaryRequest() { });
        }
        /// <summary>
        /// Use an exit item and reference the item by a direction
        /// </summary>
        /// <param name="direction"></param>
        public async Task UseExitByDirection(string direction)
        {
            ExitDirection? directionId = null;
            directionId = (direction.ToLower() == "n") ? ExitDirection.North : directionId;
            directionId = (direction.ToLower() == "s") ? ExitDirection.South : directionId;
            directionId = (direction.ToLower() == "e") ? ExitDirection.East : directionId;
            directionId = (direction.ToLower() == "w") ? ExitDirection.West : directionId;
            directionId = (direction.ToLower() == "u") ? ExitDirection.Up : directionId;
            directionId = (direction.ToLower() == "d") ? ExitDirection.Down : directionId;
            var exitItem = VMLocation.Items
                .Where(x => x.HasProperty<ExitItemProperty>() &&
                            x.GetProperty<ExitItemProperty>()?.Direction != null &&
                            x.GetProperty<ExitItemProperty>()?.Direction == directionId)
                .FirstOrDefault();
            var goCommand = exitItem?
                .GetProperty<CommandItemProperty>()?
                    .Commands
                        .Where(x => x.Id == new Guid("c558c1f9-7d01-45f3-bc35-dcab52b5a37c"))
                        .FirstOrDefault();
            if (exitItem != null && goCommand != null)
            {
                await OnItemCommand(goCommand);
            }
        }
        /// <summary>
        /// Process an item command
        /// </summary>
        /// <param name="itemCommand"></param>
        private async Task OnItemCommand(ItemCommand itemCommand)
        {
            if (itemCommand.Id == new Guid("0db8f80d-0ea8-4cbf-80ba-b37cab739391"))
            {
                var jsonEditorPage = new JsonEditorPage(Container);
                jsonEditorPage.ViewModel.ItemId = itemCommand.ItemId.ToString();
                jsonEditorPage.ViewModel.PreLoad = true;
                jsonEditorPage.Disappearing += (disSender, disE) =>
                {
                    MessageService.Send(new WorldReadLocationSummaryRequest() { });
                };
                await Page.Navigation.PushModalAsync(jsonEditorPage);
            }
            else
            {
                var useRequest = new CoreUseItemRequest()
                {
                    ItemId = itemCommand.ItemId,
                    Use = itemCommand
                };
                var result = await MessageService.SendRequestAsync<CoreUseItemResponse>(useRequest);
                if (!result.IsSuccess())
                {
                    throw new BeforeOurTimeException(result._responseMessage);
                }
            }
        }
        /// <summary>
        /// Listen to unprompted incoming messages (events)
        /// </summary>
        /// <param name="message"></param>
        private void OnMessage(IMessage message)
        {
            if (message.IsMessageType<WorldReadLocationSummaryResponse>())
            {
                ProcessListLocationResponse(message.GetMessageAsType<WorldReadLocationSummaryResponse>());
            }
            else if (message.IsMessageType<CoreMoveItemEvent>())
            {
                var moveItemEvent = message.GetMessageAsType<CoreMoveItemEvent>();
                // Refresh selected item or close item detail box
                if (IsItemSelected && SelectedItem.Id == moveItemEvent.Item.Id)
                {
                    IsItemSelected = (moveItemEvent.Item.ParentId == Me.Id || 
                                      moveItemEvent.Item.ParentId == VMLocation.Item.Id);
                    SelectedItem = (IsItemSelected) ? moveItemEvent.Item : null;
                }
                if (moveItemEvent.NewParent.Id == VMLocation.Item.Id)
                {
                    ProcessArrivalEvent(moveItemEvent);
                }
                if (moveItemEvent.OldParent.Id == VMLocation.Item.Id)
                {
                    ProcessDepartureEvent(moveItemEvent);
                }
            }
            // Output as text message into event stream
            EventStream.OnIMessage(message, this);
        }
        /// <summary>
        /// An item has been selected by one of the child controls
        /// </summary>
        /// <param name="item"></param>
        public void SelectItem(Item item)
        {
            SelectedItem = item;
            IsItemSelected = (SelectedItem != null);
            SelectedVisibleProperty = item?.GetProperty<VisibleItemProperty>();
        }
        /// <summary>
        /// Location has updated
        /// </summary>
        /// <param name="listLocationResponse"></param>
        private void ProcessListLocationResponse(WorldReadLocationSummaryResponse listLocationResponse)
        {
            EventStream.Clear();
            SelectedItem = null;
            IsItemSelected = false;
        }
        private void ProcessArrivalEvent(CoreMoveItemEvent arrivalEvent)
        {
            if (arrivalEvent.Item.Id != Me.Id)
            {
                if (arrivalEvent.OldParent.Id == Me.Id)
                {
                    Inventory.Remove(new List<Item>() { arrivalEvent.Item });
                }
                // Force notify to fire
                Inventory.Items = Inventory.Items.ToList();
            }
        }
        private void ProcessDepartureEvent(CoreMoveItemEvent departureEvent)
        {
            if (departureEvent.Item.Id != Me.Id)
            {
                if (departureEvent.NewParent.Id == Me.Id)
                {
                    Inventory.Add(new List<Item>() { departureEvent.Item });
                }
            }
        }
        /// <summary>
        /// Notify all subscribers that a property has been updated
        /// </summary>
        /// <param name="propertyName">Name of public property that has changed</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
