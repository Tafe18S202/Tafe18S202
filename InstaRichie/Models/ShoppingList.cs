using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class ShoppingList
    {
        [PrimaryKey][AutoIncrement]
        public int ShoppingItemID { get; set; }
        [NotNull]
        public string ShopName { get; set; }
        [NotNull]
        public string NameOfItem { get; set; }
        [NotNull]
        public string ShoppingDate { get; set; } // TO DO, implement dateTime.
        [NotNull]
        public double PriceQuoted { get; set; }
    }
}
