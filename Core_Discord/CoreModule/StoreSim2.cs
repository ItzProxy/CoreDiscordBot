using System;
using DSharpPlus;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using System.IO;
using System.Linq;
using Core_Discord.CoreMusic;

//figure you how to get input.
//output data & options in menus.

// for char: = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.toString(), out var value) && value.isLetter) ? true:false);
// for int: = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true:false);
// for float: = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content.toString(), out var value)) ? true:false);
// for string?: = await interactivity.WaitForMessageAsync(x => (string.TryParse(x.Content.toString(), out var value)) ? true:false);

namespace Core_Discord.CoreModule.StoreSim
{


    public class StoreSim
    {

        public float budget { get; set; } = 10000.00f;
        public EmployeeList EList { get; set; } //*
        public InventoryList IList { get; set; } //*
        public Accounting Account { get; set; } //*
        public Calculator Numbers { get; set; } //*

        protected CommandContext _ctx;

        [Command("StoreSim")]
        [Description("Play the store simulator")]
        public async Task StoreMenu(CommandContext e) //*
        {
            _ctx = e;
            bool done = false;
            var interactivity = e.Client.GetInteractivity();
            EList = new EmployeeList();
            IList = new InventoryList();
            Account = new Accounting();
            Numbers = new Calculator();
            while (!done)
            {
                var intro = new DiscordEmbedBuilder
                {
                    Description = "Introduction to Store Simulator",
                    Title = "Main Menu"
                };
                intro.AddField("Enter 'i'", "To manage Inventory", true);
                intro.AddField("Enter 'e'", "To manage Employees", true);
                intro.AddField("Enter 'a'", "To manage Account or rollover to the next month", true);
                intro.AddField("Enter 'q'", "To quit");
                await _ctx.RespondAsync(embed: intro).ConfigureAwait(false);
                var mchoice = await interactivity.WaitForMessageAsync(x => (Char.TryParse(x.Content.ToLower(), out var value) && Char.IsLetter(value)) ? true : false, TimeSpan.FromSeconds(60));
                switch (mchoice.Message.Content.ToCharArray()[0])
                {
                    case 'i':
                        await MenuI();
                        break;
                    case 'e':
                        await MenuE();
                        break;
                    case 'a':
                        await MenuA();
                        break;
                    case 'q':
                        await e.Message.RespondAsync($"goodbye\n"); //cout << "goodbye" << endl;
                        done = true;
                        break;
                    default:
                        await e.Message.RespondAsync($"Invalid choice. Please try again.\n");
                        break;
                }
            }
        }
        public async Task MenuI()
        {
            //while loop
            bool done = false;
            var interactivity = _ctx.Client.GetInteractivity();
            while (!done)
            {
                var intro = new DiscordEmbedBuilder
                {
                    Description = "Store Simulator",
                    Title = "Inventory Page"
                };
                intro.AddField("Budget", $"{budget}", true);
                intro.AddField("Next Month's Order cost", $"{IList.OrderCost}", false);
                intro.AddField("Product List", $"{ string.Join(" ", IList.InvList) }", true);
                await _ctx.RespondAsync(embed: intro); //display the intro to inventory menu

                var IMenu = new DiscordEmbedBuilder(intro);
                IMenu.ClearFields();
                IMenu.AddField("Enter 'a'", "to add a product");
                IMenu.AddField("Enter 'o'", "to alter the order");
                IMenu.AddField("Enter 's'", "to sell products.");
                IMenu.AddField("Enter 'q'", "to quit Inventory Menu");

                await _ctx.RespondAsync(embed: IMenu);

                var mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.ToLower(), out var value) && Char.IsLetter(value)), TimeSpan.FromSeconds(60));
                switch (Convert.ToChar(mchoice.Message.Content)) //figure out of this is right
                {
                    case 'a':
                        await IList.AddProduct(_ctx);
                        break;
                    case 'o':
                        await IList.AlterOrder(_ctx);
                        break;
                    case 's':
                        await IList.Sell(,_ctx);
                        break;
                    case 'q':
                        await _ctx.RespondAsync($"goodbye - Leaving Inventory Menu\n");
                        done = true;
                        break;
                    default:
                        await _ctx.RespondAsync($"Invalid choice. Please try again.\n");
                        break;
                }
            }
        }
        public async Task MenuE()
        {///////////////////////////////////////////////////////////////////////////////////////
            bool done = false;
            var interactivity = _ctx.Client.GetInteractivity();
            var TimeWait = TimeSpan.FromSeconds(60);
            while (!done)
            {
                var intro = new DiscordEmbedBuilder()
                {
                    Description = "Employee Menu - Part of Store Sim",
                    Title = "Employee Menu"
                };
                intro.AddField("Budget", $"{ budget}", true);
                intro.AddField("Next Month's Paycheck Cost", $"{EList.EmployeeCost}", true);
                intro.AddField($"Employee:", String.Join("\n", EList.list.ToArray().ToString()));
                //push back each element
                await _ctx.RespondAsync(embed: intro);

                var eMenu = new DiscordEmbedBuilder(intro);
                eMenu.ClearFields();
                eMenu.AddField("Enter 'a'", "To hire a new employee", true);
                eMenu.AddField("Enter 'f'", "to fire an employee", true);
                eMenu.AddField("Enter 'c'", "To change an employee's shift and/or pay rate.", true);
                eMenu.AddField("Enter 'q'", "To Quit", true);

                await _ctx.RespondAsync(embed: eMenu);

                var mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.ToLower(), out var value) && Char.IsLetter(value)), TimeWait);
                switch (Convert.ToChar(mchoice.Message.Content)) //figure out of this is right
                {
                    case 'a':
                        await EList.AddEmployee(_ctx);
                        break;
                    case 'f':
                        await _ctx.Message.RespondAsync($"Enter employee number (all employees with this number will be fired):");
                        var fNum = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.ToLower(), out var value) && value >= 0), TimeWait);
                        await EList.Fire(fNum, _ctx);
                        break;
                    case 'c':
                        await _ctx.Message.RespondAsync($"Enter employee number (You will go over all employees with this number):");
                        var cNum = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.ToLower(), out var value) && value >= 0), TimeWait);
                        await EList.Change(cNum, _ctx);
                        break;
                    case 'q':
                        await _ctx.Message.RespondAsync($"goodbye- Leaving Employee Menu\n");
                        done = true;
                        break;
                    default:
                        await _ctx.Message.RespondAsync($"Invalid choice. Please try again.\n");
                        break;
                }
            }
        }
        public async Task MenuA()
        {
            bool done = false;

            var interactivity = _ctx.Client.GetInteractivity();
            while (!done)
            {
                var intro = new DiscordEmbedBuilder()
                {
                    Title = "Accounting Menu",
                    Description = "Part of Store Simulator",
                    Color = DiscordColor.Red
                };
                intro.AddField("Budget:", $"{budget}", true);
                intro.AddField("Next Month's Paycheck cost", EList.EmployeeCost.ToString(), true);
                intro.AddField("Next Month's Order cost:", IList.OrderCost.ToString(), true);
                var due = budget - EList.EmployeeCost - IList.OrderCost;
                intro.AddField("Next month's budget", due.ToString(), true);
                await _ctx.RespondAsync(embed: intro);

                var AMenu = new DiscordEmbedBuilder(intro)
                {
                    Color = DiscordColor.Blue
                };
                AMenu.ClearFields();
                AMenu.AddField("Enter r to roll over month", "", true);
                AMenu.AddField("Enter c to alter budget by adding or removing money", "", true);
                AMenu.AddField("Enter q to quit", "", true);
                await _ctx.RespondAsync(embed: AMenu);
                var mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.ToLower(), out var value) && Char.IsLetter(value)), TimeSpan.FromSeconds(60));
                switch (Convert.ToChar(mchoice.Message.Content))
                {
                    case 'r':
                        Account.NextMonth(_ctx);
                        break;
                    case 'c':
                        Account.ChangeBudget();
                        break;
                    case 'q':
                        await _ctx.Message.RespondAsync($"goodbye - Leaving Account Menu\n");
                        done = true;
                        break;
                    default:
                        await _ctx.Message.RespondAsync($"Invalid choice. Please try again.\n");
                        break;
                }
            }
        }
    }

    public class EmployeeList
    {
        public float EmployeeCost { get; set; }
        public List<Employee> list { get; set; } = new List<Employee>();

        public async Task AddEmployee(CommandContext e)
        {
            TimeSpan TimeWait = TimeSpan.FromSeconds(60);
            Employee newEm;
            float UInput1;
            int UInput2;
            string UInput3;
            var interactivity = e.Client.GetInteractivity();
            //set values   
            await e.RespondAsync($"Enter new employee's name:");
            UInput3 = await interactivity.WaitForMessageAsync((x => x.Content.Any(), TimeWait),
            newEm.name = UInput3;
            await e.Message.RespondAsync($"enter new emloyee's emloyee number\n", UInput2, int);
            await e.Message.RespondAsync($"(note: if 2 employee's have the same number both will have their shift changed in the same command, and both will be fired at once):", UInput2, int);
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            newEm.EmNum = UInput2;
            await e.Message.RespondAsync($"enter new emloyee's pay rate:", UInput2, int);
            UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content.toString(), out var value)) ? true : false);
            newEm.rate = UInput1;
            await e.Message.RespondAsync($"enter hour mark newemployee's shift start time:", UInput2, int);
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            newEm.StartH = UInput2;
            await e.Message.RespondAsync($"Now the minute mark:", UInput2, int);
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            list.head.StartM = UInput2;
            await e.Message.RespondAsync($"enter hour mark newemployee's shift end time:", UInput2, int);
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            list.head.EndH = UInput2;
            await e.Message.RespondAsync($"Now the minute mark:", UInput2, int);
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            list.head.EndM = UInput2;
            list.push(newEm);
            EmployeeCost = Numbers.CalcEmployeeRate(list);
        }
        public int Fire(int search)
        {
        list.Remove(
        while (!list.empty())
            {
                if (list.head.EmNum != search)
                {
                    storage.push(list.head);
                }
                list.pop();
            }
            //push back each element
            while (!storage.empty())
            {
                list.push(storage.head);
                storage.pop();
            }
            EmployeeCost = Numbers.CalcEmployeeRate(list);
        }
        public async Task Change(int search, CommandContext e, Employee)
        {
            Stack<Employee> storage;
            float UInput1;
            int UInput2;
            var interactivity = e.Client.GetInteractivity();

            while (!list.empty())
            {//I could do this more efficiently, but this is just a game, and shuldn't have too many user generated elements.
                if (list.head.EmNum == search)
                {
                    await e.Message.RespondAsync($"User found.\n", UInput2, int);
                    await e.Message.RespondAsync("name: {0}\n");
                    await e.Message.RespondAsync("Current Pay Rate: {0}\n", rate);
                    cout << "enter new pay rate:";
                    await e.Message.RespondAsync("enter new pay rate\n", StartH, StartM);
                    UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content.toString(), out var value)) ? true : false);
                    list.head.rate = UInput1;
                    await e.Message.RespondAsync("Current Shift start time {0} : {1}\n", StartH, StartM);
                    await e.Message.RespondAsync($"enter start time, at the hour mark:", UInput2, int);
                    UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                    list.head.StartH = UInput2;
                    await e.Message.RespondAsync($"Now the minute mark:", UInput2, int);
                    UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                    list.head.StartM = UInput2;
                    await e.Message.RespondAsync("Current Shift end time {0} : {1}\n", EndH, EndM);
                    await e.Message.RespondAsync($"enter new end time, at the hour mark:", UInput2, int);
                    UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                    list.head.EndH = UInput2;
                    await e.Message.RespondAsync($"Now the minute mark:", UInput2, int);
                    UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                    list.head.EndM = UInput2;
                }
                storage.push(list.head);
                list.pop();
            }
            //push back each element
            while (!storage.empty())
            {
                list.push(storage.head);
                storgate.pop();
            }
            EmployeeCost = Numbers.CalcEmployeeRate(list);
        }
    }

    public sealed class Employee
    {
        public string name { get; set; }
        public int EmNum { get; set; }
        public float rate { get; set; }
        public int StartH { get; set; }
        public int StartM { get; set; }
        public int EndH { get; set; }
        public int EndM { get; set; }

        public override string ToString()
        {
            return $"{name} #{EmNum} paid {rate} {StartH} : {StartH} - {EndH}:{EndM}\n";
        }

        public float CalcPay()
        {
            int hours = EndH - StartH;
            if (EndM < StartM)
            {
                hours--;
            }
            if (hours < 0)
            {
                hours = 24 - hours;
            }
            return hours * rate;
        }
    }

    public class InventoryList
    {
        public float OrderCost { get; set; }
        public List<Product> InvList { get; set; } = new List<Product>();


        public async Task AddProduct(CommandContext e)
        {
            TimeSpan TimeWait = TimeSpan.FromSeconds(60);
            Product newP;
            float UInput1;
            int UInput2;
            string UInput3;
            var interactivity = e.Client.GetInteractivity();
            await e.Message.RespondAsync($"enter new product's name:");
            UInput3 = await interactivity.WaitForMessageAsync(x => x.Content.Any());///////////////*   
            newP.name = UInput3;
            await e.Message.RespondAsync($"enter new products's order price:");
            UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content.toString(), out var value)) ? true : false);
            newP.BuyPrice = UInput1;
            await e.Message.RespondAsync($"enter new products's selling price:", UInput2, int);
            UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content.toString(), out var value)) ? true : false);
            newP.SellPrice = UInput1;
            await e.Message.RespondAsync($"how many units of this product do you have in stock?:", UInput2, int);
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            newP.stock = UInput2;
            await e.Message.RespondAsync($"set units per month order:", UInput2, int);
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            newP.order = UInput2;
            list.push(newP);
            OrderCost = Numbers.CalcOrderCost(list);
        }
        public async Task AlterOrder(CommandContext e)
        {
            while (!InvList.)
            {
                await e.Message.RespondAsync($"New Order value for {name}:");
                int input;
                var interactivity = e.Client.GetInteractivity();
                input = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                list.head.order = input;
                storage.push(list.head);
                list.pop();
            }
            //push back each element
            OrderCost = new Calculator().CalcOrderCost(InvList);
        }
        public async Task Sell(string search)
        {
            _
            List<Product> storage;
            while (!list.empty())
            {
                await e.Message.RespondAsync("How Many {0} were sold?", name);
                int input;
                var interactivity = e.Client.GetInteractivity();
                input = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                if ((list.head.Stock - input) < 0)
                {
                    input = list.head.Stock;
                }
                list.head.Stock = list.head.Stock - input;
                budget = budget + list.head.SellPrice * input;
                storage.push(list.head);
                list.pop();
            }
            //push back each element
            while (!storage.empty())
            {
                list.push(storage.head);
                storgate.pop();
            }
        }
    }


    public sealed class Product
    {
        public string name { get; set; }
        public float SellPrice { get; set; }
        public float BuyPrice { get; set; }
        public int Stock { get; set; }
        public int order { get; set; }

        public override string ToString()
        {
            return $"{name} Sell:${SellPrice} buy:${BuyPrice} Stock:{BuyPrice} Next Month's Order {Stock}";
        }
    }

    public class Accounting
    {
        public float NextMonth(CommandContext e)
        {

            if (budget - Numbers.CalcMonthlyCost() < 0.0)
            {
                budget = budget - Numbers.CalcMonthlyCost();
                await e.Message.RespondAsync($"month rolled over\n", budget, int);
            }
            else
            {
                await e.Message.RespondAsync($"cannot roll over month unless products are sold\n", budget, int);
            }
        }
        public float ChangeBudget()
        {
            float change;
            var interactivity = e.Client.GetInteractivity();
            await e.Message.RespondAsync($"enter value to add to budget (to take away from budget, enter a negative value):", budget, int);
            change = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content.toString(), out var value)) ? true : false);
            budget = budget + change;
        }
    }

    public sealed class Calculator //still stuff to clean up
    {
        public float CalcEmployeeRate(List<Employee> list) //
        {
            //pop each element
            float total = 0;
            int hours;
            foreach (var i in list)
            {
                total += list.CalcPay();
            }
            //push back each element

            return total;
        }
        public float CalcOrderCost(List<Product> list) //
        {
            //pop each element
            float total = 0;
            Stack<Product> storage;
            while (!list.empty())
            {
                total += list.head.BuyPrice;
                storage.push(list.head);
                list.pop();
            }
            //push back each element
            while (!storage.empty())
            {
                list.push(storage.head);
                storgate.pop();
            }
            return total;
        }
        public float CalcMonthlyCost() //
        {
            return EmployeeCost + OrderCost;
        }
    }
}