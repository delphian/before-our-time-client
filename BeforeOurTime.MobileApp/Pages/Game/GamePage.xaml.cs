﻿using Autofac;
using BeforeOurTime.MobileApp.Controls;
using BeforeOurTime.MobileApp.Services.Messages;
using BeforeOurTime.Models.Modules.Core.Messages.UseItem;
using BeforeOurTime.Models.Modules.Core.Models.Items;
using BeforeOurTime.Models.Modules.World.Messages.Location.CreateLocation;
using BeforeOurTime.Models.Modules.World.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeforeOurTime.MobileApp.Pages.Game
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
        /// <summary>
        /// Dependency injection container
        /// </summary>
        private IContainer Container { set; get; }
        /// <summary>
        /// View model
        /// </summary>
        public GamePageViewModel ViewModel { set; get; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Dependency injection container</param>
        public GamePage (IContainer container)
		{
			InitializeComponent();
            Container = container;
            BindingContext = ViewModel = new GamePageViewModel(Container);
        }
        public async void ButtonWest_OnClicked(object sender, EventArgs e)
        {
            try
            {
                await ViewModel.UseExit("west");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        public async void ButtonEast_OnClicked(object sender, EventArgs e)
        {
            try
            {
                await ViewModel.UseExit("east");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        public async void ButtonNorth_OnClicked(object sender, EventArgs e)
        {
            try
            {
                await ViewModel.UseExit("north");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        public async void ButtonSouth_OnClicked(object sender, EventArgs e)
        {
            try
            {
                await ViewModel.UseExit("south");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        /// <summary>
        /// Select an exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void CommandListView_ItemSelected(object sender, EventArgs e)
        {
            var command = ((BeforeOurTime.Models.Modules.Core.Models.Properties.Command)((ListView)sender).SelectedItem);
            var useRequest = new CoreUseItemRequest()
            {
                ItemId = ViewModel.SelectedItem.Id,
                Use = command
            };
            await Container.Resolve<IMessageService>().SendAsync(useRequest);
        }
    }
}