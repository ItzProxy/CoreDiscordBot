using System;
using System.ComponentModel.DataAnnotations;

namespace Core_Discord.CoreDatabase.Models
{
    /// <summary>
    /// Provides primary key to a model when entered into database
    /// </summary>
    public class DbEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.UtcNow;
    }
}
