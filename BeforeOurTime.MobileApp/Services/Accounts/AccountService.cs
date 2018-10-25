﻿using BeforeOurTime.MobileApp.Services.Messages;
using BeforeOurTime.MobileApp.Services.WebSockets;
using BeforeOurTime.Models.Exceptions;
using BeforeOurTime.Models.Modules.Account.Messages.CreateAccount;
using BeforeOurTime.Models.Modules.Account.Messages.LoginAccount;
using BeforeOurTime.Models.Modules.Account.Messages.LogoutAccount;
using BeforeOurTime.Models.Modules.Account.Messages.UpdateAccount;
using BeforeOurTime.Models.Modules.Account.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BeforeOurTime.MobileApp.Services.Accounts
{
    /// <summary>
    /// Structure that an account loginState change event handler must implement
    /// </summary>
    public delegate void RecieveStateChange(LoginState loginState);
    /// <summary>
    /// Account management service
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// Current state of login
        /// </summary>
        private LoginState State { set; get; }
        /// <summary>
        /// Last successful login credentials
        /// </summary>
        public Account Account { set; get; }
        /// <summary>
        /// Provide methods and events to communicate with websocket server
        /// </summary>
        public IWebSocketService WebSocketService { set; get; }
        /// <summary>
        /// Manage IMessage messages between client and server
        /// </summary>
        public IMessageService MessageService { set; get; }
        /// <summary>
        /// Events that will fire when login state changes
        /// </summary>
        public event RecieveStateChange OnStateChange;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSocketService">Provide methods and events to communicate with websocket server</param>
        /// <param name="messageService"></param>
        public AccountService(IWebSocketService webSocketService, IMessageService messageService)
        {
            State = LoginState.Disconnected;
            WebSocketService = webSocketService;
            MessageService = messageService;
            if (WebSocketService.GetState() == WebSocketState.Open)
            {
                SetState(LoginState.Guest);
            }
            WebSocketService.OnStateChange += (wsState) =>
            {
                if (wsState == WebSocketState.Open)
                {
                    SetState(LoginState.Guest);
                } else
                {
                    SetState(LoginState.Disconnected);
                }
            };
        }
        /// <summary>
        /// Attempt to authenticate, enumerate account characters, and attach to default character
        /// </summary>
        /// <returns>True if ready to play, false if one or more steps were unsuccessful</returns>
        public async Task<bool> ConnectAsync()
        {
            var connected = false;
            if (WebSocketService.GetState() == WebSocketState.Open)
            {
                var loginState = GetState();
                if (GetState() == LoginState.Guest)
                {
                    var account = GetAccount();
                    if (account?.Id != null)
                    {
                        await LoginAsync(account.Name, account.Password);
                    }
                }
            }
            return connected;
        }
        /// <summary>
        /// Login to server
        /// </summary>
        /// <param name="name">Account name</param>
        /// <param name="password">Account password</param>
        /// <returns></returns>
        public async Task<Account> LoginAsync(string name, string password)
        {
            try
            {
                SetState(LoginState.Authenticating);
                var loginResponse = await MessageService.SendRequestAsync<AccountLoginAccountResponse>(new AccountLoginAccountRequest()
                {
                    Email = name,
                    Password = password
                });
                if (!loginResponse.IsSuccess())
                {
                    throw new AuthenticationDeniedException();
                }
                Account = loginResponse.Account;
                Account.Password = password;
                Application.Current.Properties["Account"] = JsonConvert.SerializeObject(Account);
                await Application.Current.SavePropertiesAsync();
                SetState(LoginState.Authenticated);
            }
            catch (Exception e)
            {
                Account = null;
                SetState(LoginState.Guest);
                throw e;
            }
            return Account;
        }
        /// <summary>
        /// Create new account
        /// </summary>
        /// <param name="email">Account name</param>
        /// <param name="password">Account password</param>
        /// <param name="temporary">Account is intended for trial purposes</param>
        /// <returns>Guid of newly created account</returns>
        public async Task<Account> RegisterAsync(string email, string password, bool temporary = false)
        {
            try
            {
                SetState(LoginState.Authenticating);
                var createAccountResponse = await MessageService
                    .SendRequestAsync<AccountCreateAccountResponse>(new AccountCreateAccountRequest()
                    {
                        Email = email,
                        Password = password,
                        Temporary = temporary
                    });
                if (!createAccountResponse.IsSuccess())
                {
                    SetState(LoginState.Guest);
                    throw new Exception("Server refused to create account");
                }
                Account = createAccountResponse.CreatedAccountEvent.Account;
                Account.Password = password;
                Application.Current.Properties["Account"] = JsonConvert.SerializeObject(Account);
                await Application.Current.SavePropertiesAsync();
                SetState(LoginState.Authenticated);
            }
            catch (Exception e)
            {
                SetState(LoginState.Guest);
                throw e;
            }
            return Account;
        }
        /// <summary>
        /// Update existing account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Account account)
        {
            var result = await MessageService
                .SendRequestAsync<AccountUpdateAccountResponse>(new AccountUpdateAccountRequest()
                {
                    Account = account
                });
            if (!result.IsSuccess())
            {
                throw new Exception($"{result._responseMessage}");
            }
            Account = account;
            Application.Current.Properties["Account"] = JsonConvert.SerializeObject(Account);
            await Application.Current.SavePropertiesAsync();
        }
        /// <summary>
        /// Logout of server
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Logout()
        {
            var logout = false;
            if (GetState() != LoginState.Disconnected)
            {
                var logoutResponse = await MessageService.SendRequestAsync<AccountLogoutAccountResponse>(new AccountLogoutAccountRequest());
                Account = null;
                Application.Current.Properties["Account"] = JsonConvert.SerializeObject(Account);
                await Application.Current.SavePropertiesAsync();
                SetState(LoginState.Guest);
                logout = logoutResponse.IsSuccess();
            }
            return logout;
        }
        /// <summary>
        /// Get the last successful login credentials
        /// </summary>
        /// <returns></returns>
        public Account GetAccount()
        {
            if (Account == null)
            {
                Account = new Account();
                if (Application.Current.Properties.ContainsKey("Account"))
                {
                    Account = JsonConvert.DeserializeObject<Account>(Application.Current.Properties["Account"] as string);
                }
            }
            return Account;
        }
        /// <summary>
        /// Set the new WebSocket connection state and propogate event
        /// </summary>
        /// <param name="state"></param>
        private void SetState(LoginState state)
        {
            if (state != State)
            {
                State = state;
                OnStateChange?.Invoke(state);
            }
        }
        /// <summary>
        /// Get the current login state
        /// </summary>
        /// <returns></returns>
        public LoginState GetState()
        {
            return State;
        }
        /// <summary>
        /// Report if the service is currently logged in to server
        /// </summary>
        /// <returns></returns>
        public bool IsLoggedIn()
        {
            var loggedIn = false;
            if (State == LoginState.Authenticated)
            {
                loggedIn = true;
            }
            return loggedIn;
        }
        /// <summary>
        /// Clear any caches the service may be using
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            Account = null;
            Application.Current.Properties.Remove("Account");
            await Application.Current.SavePropertiesAsync();
        }
    }
}
