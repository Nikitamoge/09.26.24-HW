using System;
using System.Collections.Generic;
using System.Linq;


//Task 1
public static class StringExtensions
{
    public static bool IsValidBrackets(this string input)
    {
        Stack<char> stack = new Stack<char>();
        Dictionary<char, char> brackets = new Dictionary<char, char>
        {
            { '(', ')' }, { '{', '}' }, { '[', ']' }
        };

        foreach (char ch in input)
        {
            if (brackets.ContainsKey(ch))
            {
                stack.Push(ch);
            }
            else if (brackets.ContainsValue(ch))
            {
                if (stack.Count == 0 || brackets[stack.Pop()] != ch)
                {
                    return false;
                }
            }
        }

        return stack.Count == 0;
    }
}


//Task 2
public static class ArrayExtensions
{
    public static int[] Filter(this int[] array, Predicate<int> predicate)
    {
        return Array.FindAll(array, predicate);
    }
}


//Task 3
public class CreditCard
{
    public string CardNumber { get; set; }
    public string Owner { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Pin { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal Balance { get; set; }

    public event Action<decimal> OnBalanceReplenished;
    public event Action<decimal> OnFundsSpent;
    public event Action OnCreditStarted;
    public event Action<decimal> OnLimitReached;
    public event Action<string> OnPinChanged;

    public CreditCard(string cardNumber, string owner, DateTime expiryDate, string pin, decimal creditLimit)
    {
        CardNumber = cardNumber;
        Owner = owner;
        ExpiryDate = expiryDate;
        Pin = pin;
        CreditLimit = creditLimit;
        Balance = 0;
    }

    public void Replenish(decimal amount)
    {
        Balance += amount;
        OnBalanceReplenished?.Invoke(amount);
    }

    public void Spend(decimal amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            OnFundsSpent?.Invoke(amount);
        }
        else if (Balance + CreditLimit >= amount)
        {
            OnCreditStarted?.Invoke();
            Balance -= amount;
        }
        else
        {
            OnLimitReached?.Invoke(Balance + CreditLimit);
        }
    }

    public void ChangePin(string newPin)
    {
        Pin = newPin;
        OnPinChanged?.Invoke(newPin);
    }
}


//Task 4
public record DailyTemperature(double High, double Low);

class Program
{
    delegate void DisplayDelegate(string message);

    static void Main(string[] args)
    {
        //Task 1
        DisplayDelegate displayValidationResult = message => Console.WriteLine(message);
        string[] testStrings = { "{}[]", "(())", "[{}]", "[}", "[[{]}]" };
        foreach (var str in testStrings)
        {
            displayValidationResult($"{str} is valid: {str.IsValidBrackets()}");
        }


        //Task 2
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int[] evenNumbers = numbers.Filter(x => x % 2 == 0);
        int[] oddNumbers = numbers.Filter(x => x % 2 != 0);
        Console.WriteLine("Even numbers: " + string.Join(", ", evenNumbers));
        Console.WriteLine("Odd numbers: " + string.Join(", ", oddNumbers));


        //Task 3
        CreditCard card = new CreditCard("1234-5678-9012-3456", "Ryan Gosling", new DateTime(2028, 12, 31), "1234", 100m);
        card.OnBalanceReplenished += amount => Console.WriteLine($"Balance top uped: {amount}");
        card.OnFundsSpent += amount => Console.WriteLine($"Spent: {amount}");
        card.OnCreditStarted += () => Console.WriteLine("Credit funds started.");
        card.OnLimitReached += limit => Console.WriteLine($"Limit reached. Available: {limit}");
        card.OnPinChanged += newPin => Console.WriteLine($"PIN changed to: {newPin}");

        card.Replenish(10m);
        card.Spend(150m);
        card.Spend(120m);
        card.ChangePin("4321");


        //Task 4
        DailyTemperature[] temperatures = new DailyTemperature[]
        {
            new DailyTemperature(25.5, 5.0),
            new DailyTemperature(-10.0, 10.0),
            new DailyTemperature(25.3, 9.8),
            new DailyTemperature(30.6, 9.1),
            new DailyTemperature(28.8, -1.0)
        };

        var maxDiffDay = temperatures
            .Select((temp, index) => new { temp, index, diff = temp.High - temp.Low })
            .OrderByDescending(x => x.diff)
            .First();

        Console.WriteLine($"Day {maxDiffDay.index + 1} has the maximum temperature difference of {maxDiffDay.diff} degrees.");
    }
}
