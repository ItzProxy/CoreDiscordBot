using Core_Discord.CoreDatabase.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Core_Discord.CoreModule.Pokemon.Models
{
    //Modelled off the PokeAPI move models
    //https://pokeapi.co/docsv2/#moves-section
    public class Move : DbEntity
    {

        //The identifier for this move resource
        [JsonProperty("id")]
        public new int Id { get; internal set; }

        //The name for this move resource
        [JsonProperty("name")]
        public string Name { get; internal set; }

        //The percent value of how likely this move is to be successful
        [JsonProperty("accuracy")]
        public int Accuracy { get; internal set; }

        //The percent value of how likely uit is this move's effect will trigger
        [JsonProperty("effect_chance")]
        public int EffectChance { get; internal set; }

        //Power points. The number times a move can be used
        [JsonProperty("pp")]
        public int Pp { get; internal set; }
    }

    public class ContestComboSets
    {

    }

    public class ContestComboDetail
    {
    }
}
