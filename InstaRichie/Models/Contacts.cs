/**
* @author Chase Wilksch-Bergroth
*
* @date - 18 Sep 2018
*/
using SQLite.Net.Attributes;

namespace StartFinance.Models {
    class Contacts {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        [NotNull]
        public string CompanyName { get; set; }

        [NotNull]
        public int MobileNumber { get; set; }
    }
}
