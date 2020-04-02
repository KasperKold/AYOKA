using SQLite;

namespace FallDetectionApp.Models
{
    public class Contact
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string PhoneNr { get; set; }
        //public bool Done { get; set; }


    }
}

