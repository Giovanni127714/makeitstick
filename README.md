# Make It Stick

WinUI 3-desktopapp (.NET 8) waarmee docenten vragen per hoofdstuk aanmaken en studenten die beantwoorden.
Data staat in MySQL en wordt benaderd met Entity Framework Core 9 (Pomelo-provider).

## Snel starten

**Vereisten:** Visual Studio 2022 (workload *WinUI application development*), .NET 8 SDK of nieuwer, MySQL 8 (bv. via Laragon).

1. Start MySQL. Standaard verbindt de app met `localhost:3306` als `root` zonder wachtwoord.
   Andere instellingen? Maak `makeitstick.Data/appsettings.Local.json` (staat in `.gitignore`):
   ```json
   { "ConnectionStrings": { "Default": "Server=localhost;Port=3306;Database=makeitstick;User=root;Password=geheim;" } }
   ```
2. Open `makeitstick.slnx` in Visual Studio, kies platform **x64** en druk op **F5**.

Bij het opstarten voert de app automatisch alle openstaande migrations uit en vult een lege database met testdata.
Het hoofdvenster laat zien of de verbinding gelukt is en hoeveel records er zijn.

Database aanmaken zonder de app te starten:
```powershell
dotnet tool restore                               # installeert dotnet-ef 9 (zie dotnet-tools.json)
dotnet ef database update --project makeitstick.Data
```

## Projectstructuur

| Project | Inhoud |
|---|---|
| `makeitstick/` | WinUI 3-app (UI). |
| `makeitstick.Data/` | Alles rond de database: `Models/`, `AppDbContext`, `DbFactory`, `DbSeeder`, `Migrations/`, `appsettings.json`. |

De datalaag is een aparte class library omdat `dotnet ef` een WinUI-project niet kan laden.

Een context gebruiken in de app:
```csharp
await using var db = DbFactory.Create();
var studenten = await db.Students.Include(s => s.Klas).ToListAsync();
```

## Datamodel

| Tabel | Model | Belangrijkste velden | Relaties |
|---|---|---|---|
| `teachers` | `Teacher` | FirstName, LastName, Email (uniek) | heeft klassen en vraagsets |
| `classes` | `Klas` | Name (uniek) | hoort bij 1 docent, heeft studenten |
| `students` | `Student` | StudentNumber (uniek), FirstName, LastName, Email (uniek) | hoort bij 1 klas, heeft antwoorden |
| `chapters` | `Chapter` | Number (uniek), Title, Description | heeft bronnen en vraagsets |
| `references` | `Reference` | Title, Author, Url | hoort bij 1 hoofdstuk |
| `question_sets` | `QuestionSet` | Question, CreatedAt | hoort bij 1 hoofdstuk en 1 docent, heeft antwoordopties |
| `answer_options` | `AnswerOption` | Text, IsCorrect | hoort bij 1 vraagset |
| `answers` | `Answer` | AnsweredAt | student + vraagset + gekozen antwoordoptie |

Het model heet `Klas` (niet `Class`) om verwarring met het C#-keyword te voorkomen.
`references` is een gereserveerd woord in MySQL; gebruik in handgeschreven SQL backticks: `` `references` ``.

Verwijderen: docenten met klassen/vragen en klassen met studenten kunnen niet verwijderd worden (Restrict).
Een hoofdstuk of vraagset verwijderen verwijdert ook de bijbehorende bronnen, opties en antwoorden (Cascade).

## Testdata (`DbSeeder`)

Alle testdata is in het Nederlands.
2 docenten, 2 klassen (SD1A, SD1B), 6 studenten, 3 hoofdstukken uit *Make It Stick* met 4 bronnen,
4 vragen met elk 3 antwoordopties, en 24 antwoorden (elke student beantwoordt elke vraag, ~2/3 goed).
De seeder draait alleen als de tabel `teachers` leeg is.

Database leegmaken en opnieuw vullen:
```powershell
dotnet ef database drop --project makeitstick.Data --force
dotnet ef database update --project makeitstick.Data
```

## Model wijzigen

1. Pas de klasse in `makeitstick.Data/Models` en/of de configuratie in `AppDbContext.OnModelCreating` aan.
2. `dotnet ef migrations add <NaamVanWijziging> --project makeitstick.Data`
3. `dotnet ef database update --project makeitstick.Data` (of start de app).

Let op: houd alle EF Core-packages en `dotnet-ef` op **9.x**; Pomelo 9 werkt niet met EF Core 10.
