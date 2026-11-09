using System.Collections.Generic;

namespace makeitstick.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }

        public int QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; } = null!;

        public List<Answer> Answers { get; set; } = new();
    }
}
