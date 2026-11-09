using System.Collections.Generic;

namespace makeitstick.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }

        public List<Reference> References { get; set; } = new();
        public List<QuestionSet> QuestionSets { get; set; } = new();
    }
}
