using System;
using System.Collections.Generic;
using System.Text;
using Core_Discord.CoreCommon;
using Core_Discord.CoreDatabase.Models;
using Core_Discord.CoreServices.Interfaces;

namespace Core_Discord.CoreServices
{
    public class BotConfigProvider : IBotConfigProvider
    {
        
        private readonly DbService _db;

        public BotConfig BotConfig { get; set; }
        
        public BotConfigProvider(DbService db, BotConfig b)
        {
            _db = db;
            BotConfig = b;
        }

        public bool Edit(CoreBotConfigEditType type, string newValue)
        {
            using (var uow = _db.UnitOfWork)
            {
                var b = uow.BotConfig.GetOrCreate(set => set);
                switch (type)
                {
                    case CoreBotConfigEditType.CurrencyGenerationChance:
                        if(float.TryParse(newValue, out var gc) && gc > 0 && gc <= 1)
                        {
                            b.CurrencyGenerationChance = gc;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case CoreBotConfigEditType.CurrencyGenerationCooldown:
                        if(int.TryParse(newValue, out var cd) && cd >= 1)
                        {
                            b.CurrencyGenerationCooldown = cd;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case CoreBotConfigEditType.CurrencyName:
                        b.CurrencyName = newValue ?? "~";
                        break;
                    case CoreBotConfigEditType.CurrencyPluralName:
                        b.CurrencyPlural = newValue ?? b.CurrencyName + "s";
                        break;
                    case CoreBotConfigEditType.CurrencySign:
                        b.CurrencyIcon = newValue ?? "~";
                        break;
                    case CoreBotConfigEditType.CurrencyDropAmount:
                        if(int.TryParse(newValue, out var amt) && amt > 0)
                        {
                            b.CurrencyDropAmount = amt;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case CoreBotConfigEditType.CurrencyDropAmountMax:
                        if(newValue == null)
                        {
                            b.CurrencyMaxDropAmount = null;
                        }
                        else if(int.TryParse(newValue, out var cmax) && cmax > 0)
                        {
                            b.CurrencyMaxDropAmount = cmax;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    //case CoreBotConfigEditType.DmHelpString:
                    //    bc.DMHelpString = string.IsNullOrWhiteSpace(newValue)
                    //        ? "-"
                    //        : newValue;
                    //    break;
                    //case CoreBotConfigEditType.HelpString:
                    //    bc.HelpString = string.IsNullOrWhiteSpace(newValue)
                    //        ? "-"
                    //        : newValue;
                    //    break;
                    case CoreBotConfigEditType.ExpPerMessage:
                        if(int.TryParse(newValue,out var xpm) && xpm > 0)
                        {
                            b.ExpPerMessage = xpm;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case CoreBotConfigEditType.ExpMinutesTimeout:
                        if (int.TryParse(newValue, out var xpt) && xpt> 0)
                        {
                            b.ExpPerMessage = xpt;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
                BotConfig = b;
                uow.Complete();
            }
            return true; //any other response that somehow made it here, return as true
        }

        public void Reload()
        {
            using (var uow = _db.UnitOfWork)
            {
                BotConfig = uow.BotConfig.GetOrCreate();
            }
        }

    }
}
