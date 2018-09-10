/**
* @author Pablo Paramo
*
* @date - 27 Aug 2018
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class PersonalInfo
    {
        [PrimaryKey /*, AutoIncrement*/]
        public int personalID { get; set; }

        [Unique]
        public string firstName { get; set; }
        public string lastName { get; set; }

        public string DOB { get; set; }

        [NotNull]
        public string gender { get; set; }
        [NotNull, Unique]
        public string email { get; set; }
        [NotNull]
        public string address { get; set; }
        [NotNull]
        public int mobileNumber { get; set; }
    }
}
