using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TaskManagerWithErrorHandling
{
    class Program
    {
        static List<string> tasks = new List<string>();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;
            bool running = true;
            while (running)
            {
                Console.WriteLine("Выберите операцию: Add / Remove / List / Exit");
                string command = Console.ReadLine();
                try
                {
                    switch (command?.ToLower())
                    {
                        case "add":
                            ExecuteOperation("Add", AddTask);
                            break;
                        case "remove":
                            ExecuteOperation("Remove", RemoveTask);
                            break;
                        case "list":
                            ExecuteOperation("List", ListTasks);
                            break;
                        case "exit":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Неизвестная команда.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogError("Main Loop", ex, "Fatal");
                }
            }
        }

        static void ExecuteOperation(string operationName, Action operation)
        {
            try
            {
                Trace.WriteLine($">>> Начало операции: {operationName} — {DateTime.Now}");
                operation.Invoke();
                Trace.WriteLine($"<<< Операция {operationName} завершена успешно.");
            }
            catch (Exception ex)
            {
                LogError(operationName, ex, "Error");
                Console.WriteLine("!!! ОШИБКА !!!");
            }
        }

        static void LogError(string context, Exception ex, string level)
        {
            Trace.WriteLine($"!!! [{level}] Ошибка в контексте: {context}");
            Trace.WriteLine($"Сообщение: {ex.Message}");
            Trace.WriteLine($"Стек вызовов: {ex.StackTrace}");
            Console.WriteLine($"!!! [{level}] Ошибка: {ex.Message}");
        }

        static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LogError("UnhandledException", ex, "Fatal");
                Console.WriteLine("!!! НЕОБРАБОТАННОЕ ИСКЛЮЧЕНИЕ !!!");
            }
        }

        static void AddTask()
        {
            Console.Write("Введите название задачи: ");
            string task = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(task))
                throw new ArgumentException("Задача не может быть пустой.");

            tasks.Add(task);
            Console.WriteLine("Задача добавлена.");
        }

        static void RemoveTask()
        {
            Console.Write("Введите название задачи для удаления: ");
            string task = Console.ReadLine();
            if (tasks.Remove(task))
                Console.WriteLine("Задача удалена.");
            else
                Console.WriteLine("Задача не найдена.");
        }

        static void ListTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            Console.WriteLine("Список задач:");
            foreach (var task in tasks)
            {
                Console.WriteLine($"- {task}");
            }
        }
    }
}
