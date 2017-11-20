using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Reflection;

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
                switch (Convert.ToChar(mchoice.Message.Content))
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
                if (IList.InvList.Count <= 0)
                {
                    intro.AddField("Inventory:", "Empty");
                }
                else
                {
                    string invString = string.Empty;
                    foreach (var i in EList.list)
                    {
                        invString += i.ToString();
                    }
                    intro.AddField("Inventory:", invString);
                }
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
                        //await IList.Sell(,_ctx);
                        await _ctx.RespondAsync("Not fixed yet").ConfigureAwait(false);
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
        {
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
                if (EList.list.Count <= 0)
                {
                    intro.AddField("Employee:", "Empty");
                }
                else
                {
                    string empString = string.Empty;
                    foreach(var i in EList.list)
                    {
                        empString += i.ToString();
                    }
                    intro.AddField("Employee:", empString);
                }
                
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
                        int searchFNum = 0;
                        if (int.TryParse(fNum.Message.Content, out var eh)) { searchFNum = eh; }
                        await EList.Fire(searchFNum, _ctx);
                        break;
                    case 'c':
                        await _ctx.Message.RespondAsync($"Enter employee number (You will go over all employees with this number):");
                        var cNum = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.ToLower(), out var value) && value >= 0), TimeWait);
                        int searchCNum = 0;
                        if (int.TryParse(cNum.Message.Content, out var ehs)) { searchCNum = ehs; }
                        await EList.Change(searchCNum, _ctx);
                        await _ctx.RespondAsync("Not fixed yet").ConfigureAwait(false);
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
                AMenu.AddField("Enter 'r'", "To roll over month", true);
                AMenu.AddField("Enter 'c'", "To alter budget by adding or removing money", true);
                AMenu.AddField("Enter 'q'", "To quit Account Menu", true);
                await _ctx.RespondAsync(embed: AMenu);
                var mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.ToLower(), out var value) && Char.IsLetter(value)), TimeSpan.FromSeconds(60));
                switch (Convert.ToChar(mchoice.Message.Content))
                {
                    case 'r':
                        await Account.NextMonth(_ctx,budget,EList.EmployeeCost,IList.OrderCost);
                        await _ctx.RespondAsync("Not fixed yet").ConfigureAwait(false);
                        break;
                    case 'c':
                        budget = await Account.ChangeBudget(_ctx, budget);
                        await _ctx.RespondAsync("Not fixed yet").ConfigureAwait(false);
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
            Employee newEm = new Employee();

            var interactivity = e.Client.GetInteractivity();
            //set values   
            await e.RespondAsync($"Enter new employee's name:");
            var UInput3 = await interactivity.WaitForMessageAsync((x => x.Content == x.Content), TimeWait);
            newEm.Name = UInput3.Message.Content;
            await e.RespondAsync($"Enter {newEm.Name}'s employee number\n" +
                $"(note: if 2 employee's have the same number both will have their shift changed in the same command, and both will be fired at once):");
            var UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
            if (int.TryParse(UInput2.Message.Content, out var vEmnum)) { newEm.EmNum = vEmnum; };
            await e.Message.RespondAsync($"Enter {newEm.Name}'s pay rate:");
            var UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content, out var value) && value >= 0.0f));
            if (int.TryParse(UInput2.Message.Content, out var rate)) { newEm.Rate = rate; }
            await e.Message.RespondAsync($"Enter hour mark {newEm.Name}'s shift start time:");
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
            if (int.TryParse(UInput2.Message.Content, out var sh)) { newEm.StartH = sh; }
            await e.Message.RespondAsync($"Now the minute mark:");
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
            if (int.TryParse(UInput2.Message.Content, out var min)) { newEm.StartM = min; }
            await e.Message.RespondAsync($"Enter hour mark {newEm.Name}'s shift end time:");
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
            if (int.TryParse(UInput2.Message.Content, out var eh)) { newEm.EndH = eh; }
            await e.Message.RespondAsync($"Now the minute mark:");
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
            if (int.TryParse(UInput2.Message.Content, out var emin)) { newEm.EndM = emin; }
            var intro = new DiscordEmbedBuilder() {
                Description = "New Employee Card",
                Color = DiscordColor.DarkRed
            };
            //foreach(FieldInfo field in newEm.GetType().GetFields())
            //{
            //    newEm.
            //}
            intro.AddField("Name", newEm.Name);
            intro.AddField("Employee Number", newEm.EmNum.ToString());
            intro.AddField("Pay Rate", newEm.Rate.ToString());
            intro.AddField("Start Time", $"{newEm.StartH}:{newEm.StartM}");
            list.Add(newEm);
            EmployeeCost = new Calculator().CalcEmployeeRate(list);
        }
        public async Task Fire(int search, CommandContext e)
        {
            list.Remove(list.Find(x => x.EmNum == search));
            EmployeeCost = new Calculator().CalcEmployeeRate(list);
            await Task.CompletedTask;
        }
        public async Task Change(int search, CommandContext e)
        {
            TimeSpan TimeWait = TimeSpan.FromSeconds(60);
            var interactivity = e.Client.GetInteractivity();
            var foundEmployee = list.Find(x => x.EmNum == search);
            if (foundEmployee != null)
            {
                var temp = foundEmployee;
                //remove employee then add them back!? I think is the way to go because I don't know if Find is pass by referencea and if it is, probably not a good idea to change it
                await e.RespondAsync($"User found.\n");
                await e.RespondAsync($"Name: {foundEmployee.Name}\n");
                await e.RespondAsync($"Current Pay Rate: {foundEmployee.Rate}\n");
                await e.RespondAsync("Enter new pay rate\n");
                var UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content, out var value)) && value >= 0.0f, TimeWait);
                if (float.TryParse(UInput1.Message.Content, out var rate)) { foundEmployee.Rate = rate; }
                await e.RespondAsync($"Current shift start time - {foundEmployee.StartH} : {foundEmployee.StartM}");
                await e.Message.RespondAsync($"Enter hour mark {foundEmployee.Name}'s shift start time:");
                var UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
                if (int.TryParse(UInput2.Message.Content, out var sh)) { foundEmployee.StartH = sh; }
                await e.Message.RespondAsync($"Now the minute mark:");
                UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
                if (int.TryParse(UInput2.Message.Content, out var min)) { foundEmployee.StartM = min; }
                await e.Message.RespondAsync($"Current shift end time - {foundEmployee.EndH} : {foundEmployee.EndM}");
                await e.Message.RespondAsync($"Enter hour mark {foundEmployee.Name}'s shift end time:");
                UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
                if (int.TryParse(UInput2.Message.Content, out var eh)) { foundEmployee.EndH = eh; }
                await e.Message.RespondAsync($"Now the minute mark:");
                UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value) && value >= 0), TimeWait);
                if (int.TryParse(UInput2.Message.Content, out var emin)) { foundEmployee.EndM = emin; }
                //remove
                list.Remove(temp); //remove the old (just incase user fails to complete the change menu then it doesn't remove the object fromlist for ever)
                list.Add(foundEmployee); //add the modified
            }
            else
            {
                await e.RespondAsync("Employee not found");
            }
            EmployeeCost = new Calculator().CalcEmployeeRate(list);
            await Task.CompletedTask;
        }
    }

    public sealed class Employee
    {
        public string Name { get; set; }
        public int EmNum { get; set; }
        public float Rate { get; set; }
        public int StartH { get; set; }
        public int StartM { get; set; }
        public int EndH { get; set; }
        public int EndM { get; set; }

        public override string ToString()
        {
            return $"{Name} #{EmNum} paid {Rate} {StartH} : {StartH} - {EndH}:{EndM}\n";
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
            return hours * Rate;
        }
    }

    public class InventoryList
    {
        TimeSpan TimeWait = TimeSpan.FromSeconds(60);
        public float OrderCost { get; set; }
        public List<Product> InvList { get; set; } = new List<Product>();
        string genericImageUrl = "https://cdn.discordapp.com/attachments/373945959853457422/382221584498294796/PZbDAYdpMy2pbDzUgjoLpfG6AAAAAAElFTkSuQmCC.png";

        public async Task AddProduct(CommandContext e)
        {
            TimeSpan TimeWait = TimeSpan.FromSeconds(60);
            Product newP = new Product();
            var interactivity = e.Client.GetInteractivity();
            await e.Message.RespondAsync($"Enter new product's name:");
            var UInput3 = await interactivity.WaitForMessageAsync(x => x.Content.Trim().Length > 0,TimeWait);///////////////*   
            newP.Name = UInput3.Message.Content;
            await e.Message.RespondAsync($"Enter new products's order price:");
            var UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content, out var value)) && value >= 0.0f, TimeWait); //evaluate that the message is a float and >= 0
            if (float.TryParse(UInput1.Message.Content, out var buy)) { newP.BuyPrice = buy; }
            await e.Message.RespondAsync($"Enter new products's selling price:");
            UInput1 = await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content, out var value)) && value >= 0.0f, TimeWait);
            if (float.TryParse(UInput1.Message.Content, out var sell)) { newP.SellPrice = sell; }
            await e.RespondAsync($"How many units of {newP.Name} do you have in stock?:");
            var UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value)) && value >= 0, TimeWait);
            if (int.TryParse(UInput2.Message.Content, out var stock)) { newP.Stock = stock; }
            await e.Message.RespondAsync($"Set units per month order:");
            UInput2 = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content, out var value)) && value >= 0, TimeWait);
            if (int.TryParse(UInput2.Message.Content, out var order)) { newP.Order = order; }
            var intro = new DiscordEmbedBuilder()
            {
                Title = "Product",
                Description = "Adding product"
            };
            intro.ImageUrl = genericImageUrl;
            intro.AddField("Name", newP.Name);
            intro.AddField("Buy Price", newP.BuyPrice.ToString());
            intro.AddField("Sell Price", newP.SellPrice.ToString());
            intro.AddField("Units in stock", newP.Stock.ToString());
            intro.AddField("Order number", newP.Order.ToString());
            await e.RespondAsync(embed: intro);
            InvList.Add(newP);
            OrderCost = new Calculator().CalcOrderCost(InvList);
        }
        public async Task AlterOrder(CommandContext e)
        {
            //var interactivity = e.Client.GetInteractivity();

            //foreach (!InvList.)
            //{
            //    await e.Message.RespondAsync($"New Order Value for {name}:");
            //    int input;

            //    input = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            //    list.head.order = input;
            //    storage.push(list.head);
            //    list.pop();
            //}
            ////push back each element
            //OrderCost = new Calculator().CalcOrderCost(InvList);
            await e.RespondAsync("Not Implemented yet").ConfigureAwait(false);
        }
        public async Task Sell(string search, CommandContext e)
        {

            //while (!list.empty())
            //{
            //    await e.Message.RespondAsync("How Many {0} were sold?", name);
            //    int input;
            //    var interactivity = e.Client.GetInteractivity();
            //    input = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
            //    if ((list.head.Stock - input) < 0)
            //    {
            //        input = list.head.Stock;
            //    }
            //    list.head.Stock = list.head.Stock - input;
            //    budget = budget + list.head.SellPrice * input;
            //    storage.push(list.head);
            //    list.pop();
            //}
            ////push back each element
            //while (!storage.empty())
            //{
            //    list.push(storage.head);
            //    storgate.pop();
            //}
            await e.RespondAsync("Not Implemented yet").ConfigureAwait(false);
        }
    }


    public sealed class Product
    {
        public string Name { get; set; }
        public float SellPrice { get; set; }
        public float BuyPrice { get; set; }
        public int Stock { get; set; }
        public int Order { get; set; }

        public override string ToString()
        {
            return $"{Name} Sell:${SellPrice} buy:${BuyPrice} Stock:{BuyPrice} Next Month's Order {Stock}";
        }
    }

    public class Accounting
    {
        public async Task<float> NextMonth(CommandContext e, float budget, float EmployeeCost, float OrderCost)
        {
            Calculator Numbers = new Calculator();
            if (budget - Numbers.CalcMonthlyCost(EmployeeCost, OrderCost) < 0.0)
            {
                budget = budget - Numbers.CalcMonthlyCost(EmployeeCost, OrderCost);
                await e.Message.RespondAsync($"month rolled over\n");
            }
            else
            {
                await e.Message.RespondAsync($"cannot roll over month unless products are sold\n");
            }
            return budget;
        }
        public async Task<float> ChangeBudget(CommandContext e, float budget)
        {
            var interactivity = e.Client.GetInteractivity();
            await e.Message.RespondAsync($"enter value to add to budget (to take away from budget, enter a negative value):");
            var inter = await interactivity.WaitForMessageAsync(x => float.TryParse(x.Content, out var value) && value >= 0,TimeSpan.FromSeconds(60));
            float change = 0;
            if (int.TryParse(inter.Message.Content, out var ch)) { change = ch; }
            budget = budget + change;
            return budget;
        }
    }

    public sealed class Calculator //still stuff to clean up
    {
        public float CalcEmployeeRate(List<Employee> list) //
        {
            //pop each element
            float total = 0;
            int hours;
            int minutes;
            foreach (var i in list)
            {
                //total += ((i.EndH - i.StartH) + (i.End)

            }
            //push back each element

            return total;
        }
        public float CalcOrderCost(List<Product> list) //
        {
            //pop each element
            float total = 0;
            foreach(var i in list)
            {
                total += i.BuyPrice;
            }
            return total;
        }
        public float CalcMonthlyCost(float EmployeeCost, float OrderCost) //
        {
            return EmployeeCost + OrderCost;
        }
    }
}