﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToDoListApp
{
    public class Task
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDone { get; set; }
        public string Project { get; set; }

        public override string ToString()
        {
            return $"{Title},{DueDate.ToString("yyyy-MM-dd")},{IsDone},{Project}";
        }

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

    public class TaskManager
    {
        private List<Task> tasks = new List<Task>();
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tasks.txt");

        public void LoadTasks()
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                tasks = lines.Select(Task.FromString).ToList();
            }
        }

        public void SaveTasks()
        {
            try
            {
                var lines = tasks.Select(t => t.ToString()).ToArray();
                File.WriteAllLines(filePath, lines);
                Console.WriteLine($"Uppgifter sparade framgångsrikt till: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade vid sparandet: {ex.Message}");
            }
        }

        public void AddTask(Task task)
        {
            tasks.Add(task);
        }

        public void EditTask(int index, Task task)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks[index] = task;
            }
        }

        public void MarkAsDone(int index)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks[index].IsDone = true;
            }
        }

        public void RemoveTask(int index)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks.RemoveAt(index);
            }
        }

        public List<Task> GetSortedTasks(string sortBy)
        {
            return sortBy switch
            {
                "date" => tasks.OrderBy(t => t.DueDate).ToList(),
                "project" => tasks.OrderBy(t => t.Project).ToList(),
                _ => tasks
            };
        }

        public int TotalTasks => tasks.Count;
        public int CompletedTasks => tasks.Count(t => t.IsDone);
    }

    public class Program
    {
        private static TaskManager taskManager = new TaskManager();

        public static void Main(string[] args)
        {
            taskManager.LoadTasks();
            bool running = true;

            while (running)
            {
                Console.Clear();
                DisplayHeader();

                Console.WriteLine($"Du har {taskManager.TotalTasks} saker att göra och {taskManager.CompletedTasks} saker klara!");
                Console.WriteLine("Välj ett alternativ:");
                Console.WriteLine("(1) Visa Lista (sortera Datum eller Projekt)");
                Console.WriteLine("(2) Lägg till ny");
                Console.WriteLine("(3) Uppdatera (Uppdatera, markera som klar, ta bort)");
                Console.WriteLine("(4) Spara och Stäng");

                switch (Console.ReadLine())
                {
                    case "1":
                        ShowTasks();
                        break;
                    case "2":
                        AddNewTask();
                        break;
                    case "3":
                        UpdateTask();
                        break;
                    case "4":
                        taskManager.SaveTasks();
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Ogiltigt alternativ! Försök igen.");
                        break;
                }
            }
        }

        private static void DisplayHeader()
        {
            Console.WriteLine("Din Minnesbank");
            Console.WriteLine(new string('-', 30));
        }

        private static void ShowTasks()
        {
            Console.Clear();
            Console.WriteLine("Sortera listan efter (1) Datum eller (2) Projekt?");
            string sortBy = Console.ReadLine() == "1" ? "date" : "project";

            var sortedTasks = taskManager.GetSortedTasks(sortBy);
            for (int i = 0; i < sortedTasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {sortedTasks[i]}");
            }

            Console.WriteLine("Tryck på valfri tangent för att gå tillbaka.");
            Console.ReadKey();
        }

        private static void AddNewTask()
        {
            Console.Clear();
            var newTask = new Task();

            Console.Write("Ange uppgiftens titel: ");
            newTask.Title = Console.ReadLine();
            Console.Write("Ange förfallodatum (YYYY-MM-DD): ");
            newTask.DueDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Ange status (pending/done): ");
            newTask.IsDone = Console.ReadLine().ToLower() == "done";
            Console.Write("Ange projekt: ");
            newTask.Project = Console.ReadLine();

            taskManager.AddTask(newTask);
            Console.WriteLine("Uppgift tillagd! Tryck på valfri tangent för att gå tillbaka.");
            Console.ReadKey();
        }

        private static void UpdateTask()
        {
            Console.Clear();
            ShowTasks();
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
                        EditTask(index);
                        break;
                    case "2":
                        taskManager.MarkAsDone(index);
                        Console.WriteLine("Uppgift markerad som klar!");
                        break;
                    case "3":
                        taskManager.RemoveTask(index);
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

            taskManager.EditTask(index, updatedTask);
            Console.WriteLine("Uppgift uppdaterad!");
        }
    }
}
