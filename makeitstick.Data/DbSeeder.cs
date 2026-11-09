using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using makeitstick.Models;
using Microsoft.EntityFrameworkCore;

namespace makeitstick.Data
{
    // Testdata. Draait automatisch na Migrate() / 'dotnet ef database update', maar alleen als de database nog leeg is.
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Teachers.Any()) return;

            AddTestData(db);
            db.SaveChanges();
        }

        public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
        {
            if (await db.Teachers.AnyAsync(cancellationToken)) return;

            AddTestData(db);
            await db.SaveChangesAsync(cancellationToken);
        }

        private static void AddTestData(AppDbContext db)
        {
            // Seeding draait binnen de migratie-transactie; Pomelo kan de automatische savepoint daarin niet vrijgeven.
            db.Database.AutoSavepointsEnabled = false;

            var docent1 = new Teacher { FirstName = "Anke", LastName = "Jansen", Email = "a.jansen@school.nl" };
            var docent2 = new Teacher { FirstName = "Tom", LastName = "Bakker", Email = "t.bakker@school.nl" };

            var klas1 = new Klas { Name = "SD1A", Teacher = docent1 };
            var klas2 = new Klas { Name = "SD1B", Teacher = docent2 };

            var students = new List<Student>
            {
                new() { StudentNumber = "100001", FirstName = "Daan", LastName = "de Vries", Email = "100001@student.school.nl", Klas = klas1 },
                new() { StudentNumber = "100002", FirstName = "Sanne", LastName = "Visser", Email = "100002@student.school.nl", Klas = klas1 },
                new() { StudentNumber = "100003", FirstName = "Noah", LastName = "Smit", Email = "100003@student.school.nl", Klas = klas1 },
                new() { StudentNumber = "100004", FirstName = "Emma", LastName = "Meijer", Email = "100004@student.school.nl", Klas = klas2 },
                new() { StudentNumber = "100005", FirstName = "Lucas", LastName = "Mulder", Email = "100005@student.school.nl", Klas = klas2 },
                new() { StudentNumber = "100006", FirstName = "Rowan", LastName = "de Bont", Email = "100006@student.school.nl", Klas = klas2 },
            };

            var book = "Make It Stick";
            var authors = "";

            var chapter1 = new Chapter
            {
                Number = 1,
                Title = "Leren wordt verkeerd begrepen",
                Description = "Waarom herlezen en stampen minder effectief zijn dan we denken.",
                References = { new Reference { Title = book, Author = authors } },
            };
            var chapter2 = new Chapter
            {
                Number = 2,
                Title = "Leren door ophalen",
                Description = "Het toetseffect: jezelf overhoren versterkt het geheugen.",
                References =
                {
                    new Reference { Title = book, Author = authors },
                    new Reference
                    {
                        Title = "Leren versterkt door toetsen",
                        Author = "Roediger & Karpicke (2006)",
                        Url = "https://doi.org/10.1111/j.1467-9280.2006.01693.x",
                    },
                },
            };
            var chapter3 = new Chapter
            {
                Number = 3,
                Title = "Wissel je oefening af",
                Description = "Gespreid en afgewisseld oefenen.",
                References = { new Reference { Title = book, Author = authors } },
            };

            var questionSets = new List<QuestionSet>
            {
                Question(chapter1, docent1, "Welke studiestrategie is volgens het boek het minst effectief?",
                    ("Herhaaldelijk herlezen", true),
                    ("Jezelf overhoren", false),
                    ("Gespreid oefenen", false)),
                Question(chapter2, docent1, "Wat is het toetseffect?",
                    ("Informatie ophalen uit je geheugen versterkt het onthouden", true),
                    ("Toetsen zorgen voor stress en dus slechter leren", false),
                    ("Je onthoudt beter wat je vaker leest", false)),
                Question(chapter2, docent2, "Wanneer kun je jezelf het beste overhoren?",
                    ("Na een korte pauze, als het ophalen wat moeite kost", true),
                    ("Direct na het lezen, als alles nog vers is", false),
                    ("Alleen vlak voor de toets", false)),
                Question(chapter3, docent2, "Wat betekent afgewisseld oefenen?",
                    ("Verschillende soorten opgaven door elkaar oefenen", true),
                    ("EÃ©n onderwerp afmaken voordat je aan het volgende begint", false),
                    ("Leerstof in Ã©Ã©n lange sessie doornemen", false)),
            };

            // Elke student beantwoordt elke vraag; ongeveer twee derde goed, zodat er realistische resultaten zijn.
            var answers = new List<Answer>();
            var random = new Random(42);
            var start = new DateTime(2026, 9, 1, 9, 0, 0);
            foreach (var student in students)
            {
                foreach (var questionSet in questionSets)
                {
                    var chosen = random.Next(3) < 2
                        ? questionSet.AnswerOptions.First(o => o.IsCorrect)
                        : questionSet.AnswerOptions.Where(o => !o.IsCorrect).ElementAt(random.Next(2));

                    answers.Add(new Answer
                    {
                        Student = student,
                        QuestionSet = questionSet,
                        AnswerOption = chosen,
                        AnsweredAt = start.AddHours(random.Next(24 * 30)),
                    });
                }
            }

            db.Teachers.AddRange(docent1, docent2);
            db.Classes.AddRange(klas1, klas2);
            db.Students.AddRange(students);
            db.Chapters.AddRange(chapter1, chapter2, chapter3);
            db.QuestionSets.AddRange(questionSets);
            db.Answers.AddRange(answers);
        }

        private static QuestionSet Question(Chapter chapter, Teacher teacher, string question,
            params (string Text, bool IsCorrect)[] options)
        {
            return new QuestionSet
            {
                Chapter = chapter,
                Teacher = teacher,
                Question = question,
                CreatedAt = new DateTime(2026, 8, 25),
                AnswerOptions = options.Select(o => new AnswerOption { Text = o.Text, IsCorrect = o.IsCorrect }).ToList(),
            };
        }
    }
}
