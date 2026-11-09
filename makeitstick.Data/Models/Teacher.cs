using System.Collections.Generic;

namespace makeitstick.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";

        public List<Klas> Classes { get; set; } = new();
        public List<QuestionSet> QuestionSets { get; set; } = new();
    }
}
