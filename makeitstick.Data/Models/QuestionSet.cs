using System;
using System.Collections.Generic;

namespace makeitstick.Models
{
    // Een vraag met de bijbehorende antwoordopties, gemaakt door een docent binnen een hoofdstuk.
    public class QuestionSet
    {
        public int Id { get; set; }
        public string Question { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public List<AnswerOption> AnswerOptions { get; set; } = new();
        public List<Answer> Answers { get; set; } = new();
    }
}
