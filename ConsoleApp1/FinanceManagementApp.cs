using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    //  Record for Transaction
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    //  Interface for Transaction Processing
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    //  Concrete Processor Classes
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Bank Transfer of {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Mobile Money payment of {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Crypto Wallet transaction of {transaction.Amount:C} for {transaction.Category}");
        }
    }

    //  Base Account Class
    public class Account
    {
        public string AccountNumber { get; }
        protected decimal Balance { get; set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }

    //  Sealed SavingsAccount Class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction successful. Updated balance: {Balance:C}");
            }
        }
    }

    //  FinanceApp Class
    public class FinanceApp
    {
        private List<Transaction> _transactions = new();

        public void Run()
        {
            var account = new SavingsAccount("ACC123", 1000m);

            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 200m, "Entertainment");

            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            p1.Process(t1);
            account.ApplyTransaction(t1);

            p2.Process(t2);
            account.ApplyTransaction(t2);

            p3.Process(t3);
            account.ApplyTransaction(t3);

            _transactions.AddRange(new[] { t1, t2, t3 });
        }
    }

    //  Main Method
    public class Program
    {
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
            Console.ReadLine();
        }
    }
}

