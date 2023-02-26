using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreditCard
{
    public delegate void MoneyAddedEventHandler(double amount);
    public delegate void MoneySpentEventHandler(double amount);
    public delegate void CreditStartedEventHandler();
    public delegate void CreditLimitReachedEventHandler();
    public delegate void PinChangedEventHandler();

    public class CreditCard
    {
        public string CardNumber { get; set; }
        public string CardholderName { get; set; }
        public DateTime ExpirationDate { get; set; }
        private string pin;
        public string Pin
        {
            get { return pin; }
            set {pin = value; OnPinChanged();}
        }
        public double CreditLimit { get; set; }
        public double Balance { get; private set; }
        public bool IsCreditUsed { get; private set; }
        public event MoneyAddedEventHandler MoneyAdded;
        public event MoneySpentEventHandler MoneySpent;
        public event CreditStartedEventHandler CreditStarted;
        public event CreditLimitReachedEventHandler CreditLimitReached;
        public event PinChangedEventHandler PinChanged;

        public CreditCard(string cardNumber, string cardholderName, DateTime expirationDate, string pin, double creditLimit, double balance)
        {
            CardNumber = cardNumber;
            CardholderName = cardholderName;
            ExpirationDate = expirationDate;
            Pin = pin;
            CreditLimit = creditLimit;
            Balance = balance;
            IsCreditUsed = false;
        }
        public void AddMoney(double amount)
        {
            Balance += amount;
            OnMoneyAdded(amount);
        }
        public void SpendMoney(double amount)
        {
            if (amount > Balance + CreditLimit)
            {
                throw new Exception("Недостаточно средств.");
            }

            if (Balance >= amount)
            {
                Balance -= amount;
                OnMoneySpent(amount);
            }
            else
            {
                double creditUsed = amount - Balance;
                Balance = 0;
                IsCreditUsed = true;
                OnCreditStarted();
                OnMoneySpent(amount);
                if (Balance < CreditLimit)
                {
                    OnCreditLimitReached();
                }
            }
        }
        private void OnMoneyAdded(double amount)
        {
            if (MoneyAdded != null)
            {
                MoneyAdded.Invoke(amount);
            }           
        }
        private void OnMoneySpent(double amount)
        {
            if (MoneySpent != null)
            {
                MoneySpent.Invoke(amount);
            }         
        }
        private void OnCreditStarted()
        {
            if (CreditStarted != null)
            {
                CreditStarted.Invoke();
            }
            
        }
        private void OnCreditLimitReached()
        {
            if(CreditLimitReached != null)
            { 
                CreditLimitReached.Invoke();
            }         
        }

        private void OnPinChanged()
        {
            if (PinChanged != null)
            {
                PinChanged.Invoke();
            }          
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CreditCard card = new CreditCard("1234 5678 9012 3456", "Вася Пупкин", new DateTime(2025, 12, 31), "1234", 5000, 800);

            card.MoneyAdded += OnMoneyAdded;
            card.MoneySpent += OnMoneySpent;
            card.CreditStarted += OnCreditStarted;
            card.CreditLimitReached += OnCreditLimitReached;
            card.PinChanged += OnPinChanged;

            Console.WriteLine("Вас приветствует Приват-Банк! \n");
            while (true)
            {
                Console.WriteLine(" Текущий баланс: " + card.Balance+ "грн");
                Console.WriteLine(" Кредитный лимит: " + card.CreditLimit + "грн");
                Console.WriteLine(" Используется ли кредит: " + card.IsCreditUsed);
              Console.WriteLine("\nВыберите действие:");
                Console.WriteLine(" 1. Добавить деньги");
                Console.WriteLine(" 2. Тратить деньги");
                Console.WriteLine(" 3. Изменять PIN");
               Console.WriteLine(" 4. Выход");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Введите сумму для добавления:");
                        double addAmount = Convert.ToDouble(Console.ReadLine());
                        card.AddMoney(addAmount);
                        break;
                    case "2":
                        Console.WriteLine("Введите сумму, которую хотите потратить:");
                        double spendAmount = Convert.ToDouble(Console.ReadLine());
                        try
                        {
                            card.SpendMoney(spendAmount);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "3":
                        Console.WriteLine("Введите новый PIN:");
                        string newPin = Console.ReadLine();
                        card.Pin = newPin;
                        break;
                    case "4":
                        Console.WriteLine("Прощайте!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте еще раз.");
                        break;
                }
            }
        }

        static void OnMoneyAdded(double amount)
        {
            Console.WriteLine("Добавлено " + amount + " грн в ваш аккаунт.");
        }

        static void OnMoneySpent(double amount)
        {
            Console.WriteLine("Снято " + amount + " грн с вашего аккаунта.");
        }

        static void OnCreditStarted()
        {
            Console.WriteLine("Начал пользоваться кредитом.");
        }

        static void OnCreditLimitReached()
        {
            Console.WriteLine("Достигнут кредитный лимит.");
        }

        static void OnPinChanged()
        {
            Console.WriteLine("PIN-код изменен.");
        }
    }
}
