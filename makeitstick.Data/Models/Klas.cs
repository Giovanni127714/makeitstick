using System.Collections.Generic;

namespace makeitstick.Models
{
    // "Klas" i.p.v. "Class" om verwarring met het C#-keyword te voorkomen; de tabel heet wel "classes".
    public class Klas
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public List<Student> Students { get; set; } = new();
    }
}
