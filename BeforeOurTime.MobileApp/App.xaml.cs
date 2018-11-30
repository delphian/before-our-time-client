using Autofac;
using BeforeOurTime.MobileApp.Pages.Main;
using BeforeOurTime.MobileApp.Pages.Server;
using BeforeOurTime.MobileApp.Services.Accounts;
using BeforeOurTime.MobileApp.Services.Characters;
using BeforeOurTime.MobileApp.Services.Games;
using BeforeOurTime.MobileApp.Services.Items;
using BeforeOurTime.MobileApp.Services.Loggers;
using BeforeOurTime.MobileApp.Services.Messages;
using BeforeOurTime.MobileApp.Services.WebSockets;
using BeforeOurTime.Models.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace BeforeOurTime.MobileApp
{
	public partial class App : Application
	{
        /// <summary>
        /// Depedency injection container
        /// </summary>
        IContainer Container { set; get; }
        /// <summary>
        /// Constructor
        /// </summary>
		public App ()
		{
            string connectionString;
            InitializeComponent();
#if __ANDROID__
            connectionString = "ws://10.0.2.2:5000/ws";
#else
            connectionString = "ws://localhost:5000/ws";
#endif
            connectionString = "ws://beforeourtime.world:2024/ws";
            // Required because of UWP 'release' build runtime error when traversing GetAssemblies()
            Message.GetMessageTypeDictionary();
            // Configure services
            var loggerService = new LoggerService();
            var wsService = new WebSocketService(loggerService, connectionString);
            var messageService = new MessageService(loggerService, wsService);
            var itemService = new ItemService(messageService);
            var accountService = new AccountService(wsService, messageService);
            var characterService = new CharacterService(accountService, messageService);
            var gameService = new GameService(accountService, messageService);
            var imageService = new ImageService(messageService);
            // Configure Dependency Injection
            var builder = new ContainerBuilder();
            builder.RegisterInstance<ILoggerService>(loggerService).SingleInstance();
            builder.RegisterInstance<IWebSocketService>(wsService).SingleInstance();
            builder.RegisterInstance<IMessageService>(messageService).SingleInstance();
            builder.RegisterInstance<IItemService>(itemService).SingleInstance();
            builder.RegisterInstance<IAccountService>(accountService).SingleInstance();
            builder.RegisterInstance<ICharacterService>(characterService).SingleInstance();
            builder.RegisterInstance<IGameService>(gameService).SingleInstance();
            builder.RegisterInstance<IImageService>(imageService).SingleInstance();
            Container = builder.Build();
            // Required because of UWP 'release' build runtime error when 
            // traversing GetAssemblies()
            Message.GuidDictionaryErrors.ForEach(x =>
            {
                loggerService.Log(LogLevel.Error, x);
            });
            // Setup main page
            MainPage = new NavigationPage(new ServerPage(Container))
            {
                BarBackgroundColor = Color.Blue,
                BarTextColor = Color.WhiteSmoke,
                BackgroundColor = Color.FromHex("#ffffff")
            };
        }

        protected override void OnStart ()
		{
        }

        protected override void OnSleep ()
		{
		}
	}
}
