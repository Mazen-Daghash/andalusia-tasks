using System;

class BankAccount
{
    private decimal _balance;

    public decimal Balance => _balance;
    public string Owner { get; set; }

    public BankAccount(string owner, decimal startingBalance)
    {
        Owner = owner;
        _balance = startingBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Deposit amount must be positive.");
            return;
        }
        _balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Withdraw amount must be positive.");
            return;
        }
        if (amount > _balance)
        {
            Console.WriteLine("Not enough funds.");
            return;
        }
        _balance -= amount;
    }

    public virtual string GetAccountType()
    {
        return "Standard";
    }
}

class SavingsAccount : BankAccount
{
    public decimal InterestRate { get; set; }
    public SavingsAccount(string owner, decimal startingBalance, decimal interestRate)
        : base(owner, startingBalance)
    {
        InterestRate = interestRate;
    }

    public virtual void ApplyInterest()
    {
        Deposit(Balance * InterestRate);
    }
    public override string GetAccountType()
    {
        return "Savings";
    }
}

class PremiumSavingsAccount : SavingsAccount
{
    public PremiumSavingsAccount(string owner, decimal startingBalance, decimal interestRate)
        : base(owner, startingBalance, interestRate) { }

    public override void ApplyInterest()
    {
        Deposit(Balance * InterestRate * 2);
    }
    public override string GetAccountType()
    {
        return "Premium Savings";
    }
}

class Program
{
    static void Main()
    {
        BankAccount[] accounts = new BankAccount[]
        {
            new BankAccount("Alice", 1000),
            new SavingsAccount("Bob", 2000, 0.05m)
        };

        foreach (var acc in accounts)
        {
            Console.WriteLine($"{acc.Owner}: {acc.GetAccountType()} - Balance: {acc.Balance}");
        }

    }
}