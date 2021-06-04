using System.Linq;
using Otus.Teaching.Linq.ATM.Core.Entities;
using Otus.Teaching.Linq.ATM.Core.Services;
using Otus.Teaching.Linq.ATM.DataAccess;

namespace Otus.Teaching.Linq.ATM.Console
{
    class RunLogin
    {
        public string Login    = "Unknown";
        public string Password = "Unknown";
        public void GetUser()
        {
            System.Console.WriteLine("\n**************** Авторизация ****************");
            System.Console.WriteLine("Введите логин:");
            Login = System.Console.ReadLine();
            System.Console.WriteLine("Введите пароль:");
            Password = System.Console.ReadLine();
            System.Console.WriteLine("*********************************************\n");
        }
    }
    class Program
    {        
        static void Main(string[] args)
        {
            System.Console.WriteLine("Старт приложения-банкомата...");
            var atmManager = CreateATMManager();
            Program.Run(atmManager);            
            System.Console.WriteLine("Завершение работы приложения-банкомата...");
        }
        static void Run(ATMManager atmManager)
        {
            RunLogin RunByLogin = new RunLogin();

            if (Program.SignIn(atmManager, RunByLogin))
            {
                var user = atmManager.GetUser(RunByLogin.Login, RunByLogin.Password);                
                while (true)
                {
                    System.Console.WriteLine("Выберите действие: ");
                    System.Console.WriteLine("  '1' - Вывод данных о всех счетах заданного пользователя");
                    System.Console.WriteLine("  '2' - Вывод данных о всех счетах заданного пользователя, включая историю по каждому счёту");
                    System.Console.WriteLine("  '3' - Вывод данных о всех операциях пополнения счёта с указанием владельца каждого счёта");
                    System.Console.WriteLine("  '4' - Вывод данных о всех пользователях у которых на счёте сумма больше N (N задаётся из вне и может быть любой)");
                    System.Console.WriteLine("  'Exit' - Выйти");
                    string Action = System.Console.ReadLine();
                    switch (Action)
                    {
                        case "1":
                            var accounts = atmManager.GetUserAcc(user);
                            System.Console.WriteLine(accounts.Any()
                                ? $"Количество счетов у пользователя {user.FirstName} {user.SurName}: {accounts.Count}"
                                : $"У пользователя {user.FirstName} {user.SurName} нет счетов");
                            foreach (var account in accounts)
                            {
                                System.Console.WriteLine($" Остаток на счете ${account.CashAll}, дата открытия {account.OpeningDate:d}");
                            }
                            System.Console.WriteLine();
                            break;
                        case "2":
                            var AccHistory = atmManager.GetUserAccHistory(user);
                            System.Console.WriteLine(AccHistory.Any()
                                ? $"Количество счетов у пользователя {user.FirstName} {user.SurName}: {AccHistory.Count}"
                                : $"У пользователя {user.FirstName} {user.SurName} нет счетов");
                            foreach (var account in AccHistory.Keys)
                            {
                                var commonMessage = $"    По счёту с балансом ${account.CashAll} открытому {account.OpeningDate:d}";
                                var operations = AccHistory[account];
                                System.Console.WriteLine(operations.Any()
                                    ? $"{commonMessage} было совершено {operations.Count} транзакций:"
                                    : $"{commonMessage} не было совершено ни одной транзакции");
                                foreach (var operation in operations)
                                {
                                    System.Console.WriteLine($"    [{operation.OperationDate:d}] {operation.OperationType} ${operation.CashSum}");
                                }
                            }
                            System.Console.WriteLine();
                            break;
                        case "3":
                            var Transactions = atmManager.GetTransactions();
                            System.Console.WriteLine(Transactions.Any()
                                ? $"Количество транзакций с пополнениями: {Transactions.Count}"
                                : "Нет транзакций с пополнениями");

                            foreach (var Operation in Transactions)
                            {
                                System.Console.WriteLine($"    Пополнение {Operation.Item1.OperationDate:d} на сумму {Operation.Item1.CashSum} " +
                                                         $"по счёту, открытому {Operation.Item2.OpeningDate:d} у пользователя {/*User*/Operation.Item3.FirstName} {/*User*/Operation.Item3.SurName}");
                            }
                            System.Console.WriteLine();

                            break;
                        case "4":
                            System.Console.WriteLine("Введите сумму: ");
                            string Amount = System.Console.ReadLine();
                            var Users = atmManager.GetAccByBalance(System.Convert.ToDecimal(Amount));
                            System.Console.WriteLine(Users.Any()
                                ? $"Количество пользователей с балансом больше {System.Convert.ToDecimal(Amount)}: {Users.Count}. Это: "
                                : $"Нет пользователей с таким большим балансом ({System.Convert.ToDecimal(Amount)})");

                            foreach (var AccBalance in Users)
                            {
                                System.Console.WriteLine($"    {AccBalance.Item1.FirstName} {AccBalance.Item1.SurName} имеет на счете открытом {AccBalance.Item2.OpeningDate:d} " +
                                                         $"баланс {AccBalance.Item2.CashAll}, что больше {System.Convert.ToDecimal(Amount)}");
                            }
                            System.Console.WriteLine();
                            break;
                        case "Exit":
                            return;
                        default:
                            System.Console.WriteLine("Неверный формат команды...");
                            break;
                    }
                }
            }
        }  
        static bool SignIn(ATMManager atmManager, RunLogin RunByLogin)
        {           
            RunByLogin.GetUser();
            var user = atmManager.GetUser(RunByLogin.Login, RunByLogin.Password);
            System.Console.WriteLine(user == null
                ? $" Пользователя с логином '{RunByLogin.Login}' и паролем '{RunByLogin.Password}' не существует"
                : $" Пользователя с логином '{RunByLogin.Login}' и паролем '{RunByLogin.Password}' зовут {user.FirstName} {user.SurName}");
            if (user == null)
            {
                System.Console.WriteLine("Выберите действие: \n '1' - Повторить попытку; \n '2' - Завершить работу;");
                string Action = System.Console.ReadLine();
                switch (Action)
                {
                    case "1":
                         Program.SignIn(atmManager, RunByLogin);
                         return true;
                         break;
                    case "2":
                         return false;                         
                    default:
                         System.Console.WriteLine("Неверный формат команды...");
                         return false;
                         break;
                }
            }
            else
            {
                return true;
            }
        }
        static ATMManager CreateATMManager()
        {
            using var dataContext = new ATMDataContext();
            var users = dataContext.Users.ToList();
            var accounts = dataContext.Accounts.ToList();
            var history = dataContext.History.ToList();     
            
            return new ATMManager(accounts, users, history);
        }        
    }
}