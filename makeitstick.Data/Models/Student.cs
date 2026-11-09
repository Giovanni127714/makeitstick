using System.Collections.Generic;

namespace makeitstick.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";

        public int KlasId { get; set; }
        public Klas Klas { get; set; } = null!;

        public List<Answer> Answers { get; set; } = new();
    }
}
