using System;
using System.Linq;

namespace EntityFrameworkCore
{
    class wallet
    {
        public int Id { get; set; }
        public string Holder { get; set; }
        public decimal Balance { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Holder} ({Balance:C})";
        }
        public void PrintWallets()
        {
            using (var context = new ApplictionDbContext())
            {
                foreach (var item in context.wallets)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public static void PrintWalletById(int id)
        {
            using (var context = new ApplictionDbContext())
            {
                var wallet = context.wallets
                                    .FirstOrDefault(w => w.Id == id);

                if (wallet != null)
                {
                    Console.WriteLine(wallet);
                }
                else
                {
                    Console.WriteLine($"No wallet found with Id {id}");
                }
            }
        }

        public static void AddWallet(string holder, decimal balance)
        {
            var wallet = new wallet
            {
                Holder = holder,
                Balance = balance,
            };

            using (var context = new ApplictionDbContext())
            {
                context.wallets.Add(wallet);
                context.SaveChanges();
                Console.WriteLine("Successfully added a new wallet.");
            }
        }

        public static void UpdateWalletBalance(int walletId, decimal amountToAdd)
        {
            using (var context = new ApplictionDbContext())
            {
                var wallet = context.wallets.SingleOrDefault(w => w.Id == walletId);

                if (wallet != null)
                {
                    wallet.Balance += amountToAdd;
                    context.Update(wallet);
                    context.SaveChanges();
                    Console.WriteLine("Wallet updated successfully.");
                }
                else
                {
                    Console.WriteLine($"No wallet found with Id {walletId}");
                }
            }
        }

        public static void RemoveWalletById(int walletId)
        {
            using (var context = new ApplictionDbContext())
            {
                var wallet = context.wallets.SingleOrDefault(w => w.Id == walletId);

                if (wallet != null)
                {
                    context.wallets.Remove(wallet);
                    context.SaveChanges();
                    Console.WriteLine($"Wallet with Id {walletId} removed successfully.");
                }
                else
                {
                    Console.WriteLine($"No wallet found with Id {walletId}");
                }
            }
        }

        public static void QueryWalletsByBalance(decimal minBalance)
        {
            using (var context = new ApplictionDbContext())
            {
                var result = context.wallets.Where(w => w.Balance > minBalance);

                foreach (var item in result)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public static void TransferAmount(int fromWalletId, int toWalletId, decimal amount)
        {
            using (var context = new ApplictionDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var fromWallet = context.wallets.SingleOrDefault(w => w.Id == fromWalletId);
                        var toWallet = context.wallets.SingleOrDefault(w => w.Id == toWalletId);

                        if (fromWallet == null || toWallet == null)
                        {
                            Console.WriteLine("One or both wallets not found.");
                            return;
                        }

                        // Withdraw from the source wallet
                        fromWallet.Balance -= amount;
                        context.SaveChanges();

                        // Deposit to the target wallet
                        toWallet.Balance += amount;
                        context.SaveChanges();

                        // Commit the transaction
                        transaction.Commit();

                        Console.WriteLine($"Transfer of ${amount} from wallet {fromWalletId} to wallet {toWalletId} completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        // An error occurred, rollback the transaction
                        Console.WriteLine($"Error during transfer: {ex.Message}");
                        transaction.Rollback();
                    }
                }
            }
        }
    }

}
