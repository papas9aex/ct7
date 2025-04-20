
using System.Diagnostics;
using Serilog;

class TaskItem
{
    public string Title { get; set; }
    public TaskItem(string title) => Title = title;
}

class TaskManager
{
    private static List<TaskItem> tasks = new();

    public static void AddTask(string title)
    {
        var sw = Stopwatch.StartNew();
        Trace.WriteLine("[TRACE] Start AddTask");
        if (string.IsNullOrWhiteSpace(title))
        {
            Log.Warning("Попытка добавить задачу с пустым названием.");
            Trace.WriteLine("[TRACE] End AddTask");
            return;
        }

        tasks.Add(new TaskItem(title));
        Log.Information("Задача \"{Title}\" успешно добавлена.", title);
        Trace.WriteLine("[TRACE] End AddTask");
        sw.Stop();
        Log.Debug("AddTask завершено за {ElapsedMilliseconds} мс.", sw.ElapsedMilliseconds);
    }

    public static void RemoveTask(string title)
    {
        var sw = Stopwatch.StartNew();
        Trace.WriteLine("[TRACE] Start RemoveTask");
        var task = tasks.FirstOrDefault(t => t.Title == title);
        if (task != null)
        {
            tasks.Remove(task);
            Log.Information("Задача \"{Title}\" успешно удалена.", title);
        }
        else
        {
            Log.Error("Задача \"{Title}\" не найдена.", title);
        }
        Trace.WriteLine("[TRACE] End RemoveTask");
        sw.Stop();
        Log.Debug("RemoveTask завершено за {ElapsedMilliseconds} мс.", sw.ElapsedMilliseconds);
    }

    public static void ListTasks()
    {
        Trace.WriteLine("[TRACE] Start ListTasks");
        if (tasks.Count == 0)
        {
            Log.Information("Список задач пуст.");
        }
        else
        {
            Log.Information("Всего задач: {Count}", tasks.Count);
            int i = 1;
            foreach (var task in tasks)
            {
                Console.WriteLine($"\{i++}. \{task.Title}");
                Log.Debug("Задача: {@TaskItem}", task);
            }
        }
        Trace.WriteLine("[TRACE] End ListTasks");
    }
}

class Program
{
    static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(), "log.json", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        Trace.AutoFlush = true;

        Log.Information("Приложение запущено");

        while (true)
        {
            Console.Write("Введите команду (add/remove/list/exit): ");
            var command = Console.ReadLine();

            switch (command)
            {
                case "add":
                    Console.Write("Введите название задачи: ");
                    var title = Console.ReadLine();
                    TaskManager.AddTask(title);
                    break;
                case "remove":
                    Console.Write("Введите название задачи для удаления: ");
                    var removeTitle = Console.ReadLine();
                    TaskManager.RemoveTask(removeTitle);
                    break;
                case "list":
                    TaskManager.ListTasks();
                    break;
                case "exit":
                    Log.Information("Завершение работы программы.");
                    Log.CloseAndFlush();
                    return;
                default:
                    Log.Warning("Неизвестная команда: {Command}", command);
                    break;
            }
        }
    }
}
