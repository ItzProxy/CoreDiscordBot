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

namespace Core_Discord.Store_Sim
{


    public class StoreSim
    {

    public float budget { get; set; } = 10000.00f;
    public EmployeeList EList { get; set; } //*
    public InventoryList IList { get; set; } //*
    public Accounting Acount { get; set; } //*
    public Calculator Numbers { get; set; } //*


    public async Task main(CommandContext e) //*
    {
        bool done = false;
        char mchoice;
        var interactivity = e.Client.GetInteractivityModule();
        while (!done)
        {
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);
            await e.Message.RespondAsync($"Main Menu.\n", mchoice, int);
            await e.Message.RespondAsync($"Budget: {0}.\n", budget, int);
            await e.Message.RespondAsync($"Press i to manage inventory\n", mchoice, int);
            await e.Message.RespondAsync($"press e to manage employees.\n", mchoice, int);
            await e.Message.RespondAsync($"press a to manage the account, or roll over to the next month.\n", mchoice, int);
            await e.Message.RespondAsync($"press q to quit.\n", mchoice, int);
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);
            mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.toString(), out var value) && value.isLetter) ? true : false);
            switch (mchoice)
            {
                case 'i':
                    await MenuI();
                    break;
                case 'e':
                    await MenuE();
                    break;
                case 'a':
                    MenuA();
                    break;
                case 'q':
                    await e.Message.RespondAsync($"goodbye\n", mchoice, int); //cout << "goodbye" << endl;
                    done = true;
                    break;
                default:
                    await e.Message.RespondAsync($"Invalid choice. Please try again.\n", mchoice, int);
                    break;
            }
        }
    }
    public async task MenuI()
    {
        //while loop
        bool done = false;
        char mchoice;
        var interactivity = e.Client.GetInteractivityModule();
        while (!done)
        {
            await e.Message.RespondAsync($"Inventory menu.\n", mchoice, int);
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);
            await e.Message.RespondAsync($"Budget: {0}.\n", budget, int);
            await e.Message.RespondAsync($"Next Month's Order cost: {0}.\n", IList.OrderCost, int);
            await e.Message.RespondAsync($"Product list:\n", mchoice, int);
            stack<Product> storage;
            //go through each element
            while (!LList.list.empty())
            {
                IList.list.head.printinfo();
                storage.push(IList.list.head);
                IList.list.pop();
            }
            //push back each element
            while (!storage.empty())
            {
                IList.list.push(storage.head);
                storgate.pop();
            }
            await e.Message.RespondAsync($"Press a to add a product\n", mchoice, int);
            await e.Message.RespondAsync($"press o to alter the order.\n", mchoice, int);
            await e.Message.RespondAsync($"press s to sell products.\n", mchoice, int);
            await e.Message.RespondAsync($"press q to quit.\n", mchoice, int);
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);

            mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.toString(), out var value) && value.isLetter) ? true : false);
            switch (mchoice) //figure out of this is right
            {
                case 'a':
                    IList.AddProduct();
                    break;
                case 'o':
                    IList.AlterOrder();
                    break;
                case 's':
                    IList.Sell();
                    break;
                case 'q':
                    await e.Message.RespondAsync($"goodbye\n", mchoice, int);
                    done = true;
                    break;
                default:
                    await e.Message.RespondAsync($"Invalid choice. Please try again.\n", mchoice, int);
                    break;
            }
        }
    }
    public async task MenuE()
    {///////////////////////////////////////////////////////////////////////////////////////
        bool done = false;
        char mchoice;
        var interactivity = e.Client.GetInteractivityModule();
        int numhunt;
        while (!done)
        {

            await e.Message.RespondAsync($"Employee Managment menu.\n", mchoice, int);
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);
            await e.Message.RespondAsync($"Budget: {0}.\n", budget, int);
            await e.Message.RespondAsync($"Next Month's Paycheck cost: {0}.\n", EList.EmployeeCost, int);
            await e.Message.RespondAsync($"Employee list:\n", mchoice, int);
            stack<Employee> storage;
            //go through each element
            while (!EList.list.empty())
            {
                EList.list.head.printinfo();
                storage.push(EList.list.head);
                EList.list.pop();
            }
            //push back each element
            while (!storage.empty())
            {
                EList.list.push(storage.head);
                storgate.pop();
            }
            await e.Message.RespondAsync($"Press a to hire a new elployee\n", mchoice, int);
            await e.Message.RespondAsync($"press f to fire an employee.\n", mchoice, int);
            await e.Message.RespondAsync($"press c to change an employee's shift and/or pay rate.\n", mchoice, int);
            await e.Message.RespondAsync($"press q to quit.\n", mchoice, int);
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);

            mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.toString(), out var value) && value.isLetter) ? true : false);
            switch (mchoice) //figure out of this is right
            {
                case 'a':
                    EList.AddEmployee();
                    break;
                case 'f':
                    await e.Message.RespondAsync($"Enter employee number (all employees with this number will be fired):", mchoice, int);
                    numhunt == await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                    EList.Fire(numhunt);
                    break;
                case 'c':
                    await e.Message.RespondAsync($"Enter employee number (You will go over all employees with this number):", mchoice, int);
                    numhunt == await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
                    EList.change(numhunt);
                    break;
                case 'q':
                    await e.Message.RespondAsync($"goodbye\n", mchoice, int);
                    done = true;
                    break;
                default:
                    await e.Message.RespondAsync($"Invalid choice. Please try again.\n", mchoice, int);
                    break;
            }
        }
    }
    public async task MenuA()
    {
        bool done = false;
        char mchoice;
        int due;
        var interactivity = e.Client.GetInteractivityModule();
        while (!done)
        {
            await e.Message.RespondAsync($"Accounting menu.\n", mchoice, int);
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);
            await e.Message.RespondAsync($"Budget: {0}.\n", budget, int);
            await e.Message.RespondAsync($"Next Month's Paycheck cost: {0}.\n", EList.EmployeeCost, int);
            await e.Message.RespondAsync($"Next Month's Order cost: {0}.\n", IList.OrderCost, int);
            due = budget - EList.EmployeeCost - IList.OrderCost;
            await e.Message.RespondAsync($"Next month's budget: {0}.\n", due, int);
            await e.Message.RespondAsync($"Press r to roll over month\n", mchoice, int);
            await e.Message.RespondAsync($"press c to alter budget by adding or removing money.\n", mchoice, int);
            await e.Message.RespondAsync($"press q to quit.\n", mchoice, int);
            await e.Message.RespondAsync($"*****************.\n", mchoice, int);

            mchoice = await interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.toString(), out var value) && value.isLetter) ? true : false);
            switch (mchoice)
            {
                case 'r':
                    Acount.NextMonth();
                    break;
                case 'c':
                    Acount.ChangeBudget();
                    break;
                case 'q':
                    await e.Message.RespondAsync($"goodbye\n", mchoice, int);
                    done = true;
                    break;
                default:
                    await e.Message.RespondAsync($"Invalid choice. Please try again.\n", mchoice, int);
                    break;
            }
        }
    }
}

public class EmployeeList
{
    public float EmployeeCost; {   get;   set;  }
public stack<Employee> list; {   get;   set;  }

  public async task AddEmployee()
{
    employee newEm;
    float UInput1;
    int UInput2;
    string UInput3;
    var interactivity = e.Client.GetInteractivityModule();
    //set values   
    await e.Message.RespondAsync($"enter new emloyee's name:", UInput2, int);
    UInput3 = await interactivity.WaitForMessageAsync(x => (X.contain.any()) ? true : false);///////////////*
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

    stack<Employee> storage;

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
        storgate.pop();
    }
    EmployeeCost = Numbers.CalcEmployeeRate(list);
}
public async task Change(int search)
{
    stack<Employee> storage;
    float UInput1;
    int UInput2;
    var interactivity = e.Client.GetInteractivityModule();
    while (!list.empty())
    {//I could do this more efficiently, but this is just a game, and shuldn't have too many user generated elements.
        if (list.head.EmNum == search)
        {
            await e.Message.RespondAsync($"User found.\n", UInput2, int);
            await e.Message.RespondAsync("name: {0}\n", name);
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
    public string name; {   get;   set;  }
public int EmNum; {   get;   set;  }
  public float rate; {   get;   set;  }
  public int StartH; {   get;   set;  }
  public int StartM; {   get;   set;  }
  public int EndH; {   get;   set;  }
  public int EndM; {   get;   set;  }

  public async task printinfo(); {
   //cout << name << " #" << EmNum << " payed:" << rate << " " << StartH << ":" << StartM << "-" << EndH << ":" << EndM << endl;
   await e.Message.RespondAsync("{0} #{1} payed {2} {3} : {4} - {5}:{6}\n", name, EmNum, rate, StartH, StartM, EndH, EndM);
  }
  public float CalcPay(); {
   hours = EndH - Start;
   if (EndM<StartM) {
    hours--;
   } 
   if(hours<0){
    hours = 24 - hours;
   }
   return hours* rate;
  }
 }

 public class InventoryList
{
    public float OrderCost; {   get;   set;  }
public stack<Product> list; {   get;   set;  }

  public async task AddProduct()
{
    Product newP;
    float UInput1;
    int UInput2;
    string UInput3;
    var interactivity = e.Client.GetInteractivityModule();
    await e.Message.RespondAsync($"enter new product's name:", UInput2, int);
    UInput3 = await interactivity.WaitForMessageAsync(x => (X.contain.any()) ? true : false);///////////////*   
    newP.name = UInput3;
    await e.Message.RespondAsync($"enter new products's order price:", UInput2, int);
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
public int AlterOrder()
{
    stack<Product> storage;
    while (!list.empty())
    {
        await e.Message.RespondAsync("New Order value for {0}:", name);
        int input;
        var interactivity = e.Client.GetInteractivityModule();
        input = await interactivity.WaitForMessageAsync(x => (int.TryParse(x.Content.toString(), out var value)) ? true : false);
        list.head.order = input;
        storage.push(list.head);
        list.pop();
    }
    //push back each element
    while (!storage.empty())
    {
        list.push(storage.head);
        storgate.pop();
    }
    OrderCost = Numbers.CalcOrderCost(list);
}
public async task Sell(string search)
{
    stack<Product> storage;
    while (!list.empty())
    {
        await e.Message.RespondAsync("How Many {0} were sold?", name);
        int input;
        var interactivity = e.Client.GetInteractivityModule();
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
    public string name; {   get;   set;  }
public float SellPrice; {   get;   set;  }
  public float BuyPrice; {   get;   set;  }
  public int Stock; {   get;   set;  }
  public int order; {   get;   set;  }

  public async task printinfo(); {
   await e.Message.RespondAsync("{0} Sell:${1} buy:${2} Stock:{3} Next Month's Order {4}\n", name, SellPrice, BuyPrice, Stock, order);
  }
 }

 public class Accounting
{
    public float NextMonth(); {
   if (budget - Numbers.CalcMonthlyCost() < 0.0) {
    budget = budget - Numbers.CalcMonthlyCost();
	await e.Message.RespondAsync($"month rolled over\n", budget, int);
} else {
	await e.Message.RespondAsync($"cannot roll over month unless products are sold\n", budget, int);
   }
  }
  public float ChangeBudget(); {
   float change;
var interactivity = e.Client.GetInteractivityModule();
await e.Message.RespondAsync($"enter value to add to budget (to take away from budget, enter a negative value):", budget, int);
change= await interactivity.WaitForMessageAsync(x => (float.TryParse(x.Content.toString(), out var value)) ? true:false);
   budget = budget + change;
  }
 }

 public sealed class Calculator //still stuff to clean up
{
    public float CalcEmployeeRate(stack<Employee> list) //
    {
        //pop each element
        float total = 0;
        stack<Employee> storage;
        int hours;
        while (!list.empty())
        {
            total += list.head.CalcPay();
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
    public float CalcOrderCost(stack<Product> list) //
    {
        //pop each element
        float total = 0;
        stack<Product> storage;
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