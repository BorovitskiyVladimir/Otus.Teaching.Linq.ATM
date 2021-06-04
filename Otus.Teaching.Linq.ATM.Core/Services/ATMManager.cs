using Otus.Teaching.Linq.ATM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Otus.Teaching.Linq.ATM.Core.Services
{
    public class ATMManager
    {        
        public IEnumerable<Account> Accounts { get; }        
        public IEnumerable<User> Users { get; }        
        public IEnumerable<OperationsHistory> History { get; }        
        public ATMManager(IEnumerable<Account> accounts, IEnumerable<User> users, IEnumerable<OperationsHistory> history)
        {
            Accounts = accounts;
            Users = users;
            History = history;
        }        
        public User GetUser(string Login, string Password)
        {
            return Users.FirstOrDefault(User => User.Login.Equals(Login, StringComparison.Ordinal) && User.Password.Equals(Password, StringComparison.Ordinal));
        }

        public List<Account> GetUserAcc(User user)
        {
            return Accounts.Where(Acc => Acc.UserId == user.Id).ToList();                        
        }
        public Dictionary<Account, List<OperationsHistory>> GetUserAccHistory(User user)
        {
            return Accounts.Where(Acc => Acc.UserId == user.Id).GroupJoin(History, Acc => Acc.Id, History => History.AccountId, (Acc, History) => new
                {
                  Account = Acc, 
                  Operation = History.Select(o => o).ToList()
                }
            ).ToDictionary(Acc => Acc.Account, Acc => Acc.Operation);                        
        }
        public List<Tuple<OperationsHistory, Account, User>> GetTransactions()
        {
            return History.Where(Operation => Operation.OperationType == OperationType.InputCash
                         ).Join(Accounts, History => History.AccountId, Acc => Acc.Id, (History, Acc) => new { History = History, Account = Acc }
                         ).Join(Users, AccHistory => AccHistory.Account.UserId, user => user.Id, (AccHistory, user) => new 
                           {
                              AccHistory = AccHistory, 
                              User = user 
                           }
                         ).Select(i => new  Tuple<OperationsHistory, Account, User>(i.AccHistory.History, i.AccHistory.Account, i.User)).ToList();
        }
        public List<Tuple<User, Account>> GetAccByBalance(decimal minBalance)
        {            
            return Accounts.Where(a => a.CashAll > minBalance
                          ).Join(Users, account => account.UserId, user => user.Id, (account, user) => new { Account = account, User = user }
                          ).Select(item => new Tuple<User, Account>(item.User, item.Account)).ToList();
        }

    }
}