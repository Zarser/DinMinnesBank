using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToDoListApp
{
    // Klass som representerar en enskild uppgift
    public class Task
    {
        public string Title { get; set; } // Titel på uppgiften
        public DateTime DueDate { get; set; } // Förfallodatum för uppgiften
        public bool IsDone { get; set; } // Status om uppgiften är klar eller ej
        public string Project { get; set; } // Projekt som uppgiften tillhör

        // Konverterar uppgiften till en sträng som kan sparas i en textfil
        public override string ToString()
        {
            return $"{Title},{DueDate.ToString("yyyy-MM-dd")},{IsDone},{Project}";
        }

        // Skapar en uppgift från en sparad sträng
        public static Task FromString(string taskString)
        {
            var parts = taskString.Split(',');
            return new Task
            {
                Title = parts[0],
                DueDate = DateTime.Parse(parts[1]),
                IsDone = bool.Parse(parts[2]),
                Project = parts[3]
            };
        }
    }

    // Klass som hanterar listan av uppgifter
    public class TaskManager
    {
        private List<Task> tasks = new List<Task>(); // Lista över alla uppgifter
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tasks.txt"); // Sökväg till textfilen

        // Laddar uppgifter från textfilen om den finns
        public void LoadTasks()
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath); // Läser alla rader från filen
                tasks = lines.Select(Task.FromString).ToList(); // Skapar uppgifter från varje rad
            }
        }

        // Sparar alla uppgifter till textfilen
        public void SaveTasks()
        {
            try
            {
                var lines = tasks.Select(t => t.ToString()).ToArray(); // Konverterar alla uppgifter till strängar
                File.WriteAllLines(filePath, lines); // Skriver alla strängar till filen
                Console.WriteLine($"Uppgifter sparade framgångsrikt till: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade vid sparandet: {ex.Message}");
            }
        }

        // Lägger till en ny uppgift
        public void AddTask(Task task)
        {
            tasks.Add(task);
        }

        // Uppdaterar en befintlig uppgift
        public void EditTask(int index, Task task)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks[index] = task; // Ersätter uppgiften på det angivna indexet
            }
        }

        // Markerar en uppgift som klar
        public void MarkAsDone(int index)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks[index].IsDone = true; // Ändrar status till klar
            }
        }

        // Tar bort en uppgift från listan
        public void RemoveTask(int index)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks.RemoveAt(index); // Tar bort uppgiften från listan
            }
        }

        // Returnerar en sorterad lista av uppgifter baserat på valt kriterium (datum eller projekt)
        public List<Task> GetSortedTasks(string sortBy)
        {
            return sortBy switch
            {
                "date" => tasks.OrderBy(t => t.DueDate).ToList(), // Sorterar efter datum
                "project" => tasks.OrderBy(t => t.Project).ToList(), // Sorterar efter projekt
                _ => tasks // Ingen sortering om inget val görs
            };
        }

        // Egenskap för att få totala antalet uppgifter
        public int TotalTasks => tasks.Count;

        // Egenskap för att få antalet uppgifter som är klara
        public int CompletedTasks => tasks.Count(t => t.IsDone);
    }

    public class Program
    {
        private static TaskManager taskManager = new TaskManager(); // Instans av TaskManager

        public static void Main(string[] args)
        {
            taskManager.LoadTasks(); // Laddar uppgifter från filen
            bool running = true;

            while (running)
            {
                Console.Clear();
                DisplayHeader(); // Visar programmets rubrik

                // Visar hur många uppgifter som är klara och kvar att göra
                Console.WriteLine($"Du har {taskManager.TotalTasks} saker att göra och {taskManager.CompletedTasks} saker klara!");
                Console.WriteLine("Välj ett alternativ:");
                Console.WriteLine("(1) Visa Lista (sortera Datum eller Projekt)");
                Console.WriteLine("(2) Lägg till ny");
                Console.WriteLine("(3) Uppdatera (Uppdatera, markera som klar, ta bort)");
                Console.WriteLine("(4) Spara och Stäng");

                // Hanterar användarens val
                switch (Console.ReadLine())
                {
                    case "1":
                        ShowTasks(); // Visar alla uppgifter
                        break;
                    case "2":
                        AddNewTask(); // Lägger till en ny uppgift
                        break;
                    case "3":
                        UpdateTask(); // Uppdaterar en befintlig uppgift
                        break;
                    case "4":
                        taskManager.SaveTasks(); // Sparar och avslutar programmet
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Ogiltigt alternativ! Försök igen.");
                        break;
                }
            }
        }

        // Visar programmets rubrik
        private static void DisplayHeader()
        {
            Console.WriteLine("Din Minnesbank");
            Console.WriteLine(new string('-', 30));
        }

        // Visar listan av uppgifter sorterade efter datum eller projekt
        private static void ShowTasks()
        {
            Console.Clear();
            Console.WriteLine("Sortera listan efter (1) Datum eller (2) Projekt?");
            string sortBy = Console.ReadLine() == "1" ? "date" : "project";

            var sortedTasks = taskManager.GetSortedTasks(sortBy); // Hämtar sorterade uppgifter
            for (int i = 0; i < sortedTasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {sortedTasks[i]}");
            }

            Console.WriteLine("Tryck på valfri tangent för att gå tillbaka.");
            Console.ReadKey();
        }

        // Lägger till en ny uppgift genom användarinmatning
        private static void AddNewTask()
        {
            Console.Clear();
            var newTask = new Task();

            Console.Write("Ange uppgiftens titel: ");
            newTask.Title = Console.ReadLine(); // Tar emot titel från användaren
            Console.Write("Ange förfallodatum (YYYY-MM-DD): ");
            newTask.DueDate = DateTime.Parse(Console.ReadLine()); // Tar emot förfallodatum från användaren
            Console.Write("Ange status (pending/done): ");
            newTask.IsDone = Console.ReadLine().ToLower() == "done"; // Tar emot status från användaren
            Console.Write("Ange projekt: ");
            newTask.Project = Console.ReadLine(); // Tar emot projekt från användaren

            taskManager.AddTask(newTask); // Lägger till uppgiften i listan
            Console.WriteLine("Uppgift tillagd! Tryck på valfri tangent för att gå tillbaka.");
            Console.ReadKey();
        }

        // Uppdaterar en befintlig uppgift eller markerar den som klar eller tar bort den
        private static void UpdateTask()
        {
            Console.Clear();
            ShowTasks(); // Visar alla uppgifter
            Console.Write("Ange numret på uppgiften att uppdatera: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0)
            {
                int index = taskNumber - 1;

                Console.WriteLine("Vad vill du göra?");
                Console.WriteLine("(1) Uppdatera uppgift");
                Console.WriteLine("(2) Markera som klar");
                Console.WriteLine("(3) Ta bort uppgift");

                switch (Console.ReadLine())
                {
                    case "1":
                        EditTask(index); // Uppdaterar uppgiften
                        break;
                    case "2":
                        taskManager.MarkAsDone(index); // Markerar uppgiften som klar
                        Console.WriteLine("Uppgift markerad som klar!");
                        break;
                    case "3":
                        taskManager.RemoveTask(index); // Tar bort uppgiften
                        Console.WriteLine("Uppgift borttagen!");
                        break;
                    default:
                        Console.WriteLine("Ogiltigt alternativ!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt uppgiftsnummer.");
            }

            Console.WriteLine("Tryck på valfri tangent för att gå tillbaka.");
            Console.ReadKey();
        }

        // Uppdaterar en befintlig uppgift
        private static void EditTask(int index)
        {
            var existingTask = taskManager.GetSortedTasks("date")[index];
            var updatedTask = new Task();

            Console.Write("Ange ny titel (lämna tom för att behålla): ");
            string titleInput = Console.ReadLine();
            updatedTask.Title = string.IsNullOrWhiteSpace(titleInput) ? existingTask.Title : titleInput;

            Console.Write("Ange nytt förfallodatum (YYYY-MM-DD, lämna tomt för att behålla): ");
            string dateInput = Console.ReadLine();
            updatedTask.DueDate = string.IsNullOrWhiteSpace(dateInput) ? existingTask.DueDate : DateTime.Parse(dateInput);

            Console.Write("Ange nytt projekt (lämna tomt för att behålla): ");
            string projectInput = Console.ReadLine();
            updatedTask.Project = string.IsNullOrWhiteSpace(projectInput) ? existingTask.Project : projectInput;

            taskManager.EditTask(index, updatedTask); // Sparar den uppdaterade uppgiften
            Console.WriteLine("Uppgift uppdaterad!");
        }
    }
}
