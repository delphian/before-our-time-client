﻿using BeforeOurTime.Models.Modules.Account.Messages.Json;
using BeforeOurTime.Models.Modules.Account.Models;
using BeforeOurTime.Models.Modules.Account.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeforeOurTime.MobileApp.Services.Accounts
{
    public interface IAccountService : IService
    {
        /// <summary>
        /// Events that will fire when login state changes
        /// </summary>
        event RecieveStateChange OnStateChange;
        /// <summary>
        /// Attempt to authenticate, enumerate account characters, and attach to default character
        /// </summary>
        /// <returns>True if ready to play, false if one or more steps were unsuccessful</returns>
        Task<bool> ConnectAsync();
        /// <summary>
        /// Create new account
        /// </summary>
        /// <param name="email">Account name</param>
        /// <param name="password">Account password</param>
        /// <param name="temporary">Account is intended for trial purposes</param>
        /// <returns>Guid of newly created account</returns>
        Task<Account> RegisterAsync(string email, string password, bool temporary = false);
        /// <summary>
        /// Update account password
        /// </summary>
        /// <param name="accountId">Account id to update</param>
        /// <param name="OldPassword">Old password value</param>
        /// <param name="NewPassword">New password value</param>
        /// <returns></returns>
        Task<Account> UpdatePasswordAsync(
            Guid accountId,
            string OldPassword,
            string NewPassword);
        /// <summary>
        /// Update existing account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task UpdateAsync(Account account);
        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid accountId);
        /// <summary>
        /// Login to server
        /// </summary>
        /// <param name="name">Account name</param>
        /// <param name="password">Account password</param>
        /// <returns></returns>
        Task<Account> LoginAsync(string name, string password);
        /// <summary>
        /// Logout of the server
        /// </summary>
        Task<bool> Logout();
        /// <summary>
        /// Get the last successful login credentials
        /// </summary>
        /// <returns></returns>
        Account GetAccount();
        /// <summary>
        /// Read account data
        /// </summary>
        /// <param name="accountIds">List of account ids to retrieve data on, or null for all</param>
        /// <returns></returns>
        Task<List<AccountData>> ReadAccountDataAsync(
            List<Guid> accountIds = null);
        /// <summary>
        /// Read account data in json format
        /// </summary>
        /// <param name="accountIds">List of account ids to retrieve json data on, or null for all</param>
        /// <returns></returns>
        Task<List<AccountJson>> ReadAccountDataJsonAsync(
            List<Guid> accountIds = null);
        /// <summary>
        /// Restore accounts on server from json backup file
        /// </summary>
        /// <param name="accountsJson">JSON backup file of accounts</param>
        /// <returns></returns>
        Task RestoreAccountDataAsync(string accountsJson);
        /// <summary>
        /// Get the current login state
        /// </summary>
        /// <returns></returns>
        LoginState GetState();
        /// <summary>
        /// Report if the service is currently logged in to server
        /// </summary>
        /// <returns></returns>
        bool IsLoggedIn();
    }
}
