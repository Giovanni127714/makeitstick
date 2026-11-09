using System;

namespace makeitstick.Models
{
    // Het antwoord dat een student op een vraag heeft gegeven (de gekozen antwoordoptie).
    public class Answer
    {
        public int Id { get; set; }
        public DateTime AnsweredAt { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; } = null!;

        public int AnswerOptionId { get; set; }
        public AnswerOption AnswerOption { get; set; } = null!;
    }
}
