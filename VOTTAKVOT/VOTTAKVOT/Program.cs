using System;

//Порождающий паттерн
public class Task
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public DateTime DueDate { get; set; }

    public class TaskBuilder
    {
        private Task task;

        public TaskBuilder(string title)
        {
            task = new Task { Title = title };
        }

        public TaskBuilder WithDescription(string description)
        {
            task.Description = description;
            return this;
        }

        public TaskBuilder WithPriority(int priority)
        {
            task.Priority = priority;
            return this;
        }

        public TaskBuilder WithDueDate(DateTime dueDate)
        {
            task.DueDate = dueDate;
            return this;
        }

        public Task Build()
        {
            return task;
        }
    }
}

//Структурный паттерн
public interface ITaskService
{
    Task GetTaskById(int id);
}

public class TaskServiceProxy : ITaskService
{
    private TaskService taskService;

    public Task GetTaskById(int id)
    {
        if (taskService == null)
        {
            taskService = new TaskService();
        }

        return taskService.GetTaskById(id);
    }
}

public class TaskService : ITaskService
{
    public Task GetTaskById(int id)
    {
        return new Task.TaskBuilder("Sample Task")
            .WithDescription("Sample task description")
            .WithPriority(1)
            .WithDueDate(DateTime.Now.AddDays(7))
            .Build();
    }
}

//Поведенческий паттерн
public interface ICommand
{
    void Execute();
    void Undo();
}

public class TaskOperationCommand : ICommand
{
    private Task task;
    private TaskService taskService;

    public TaskOperationCommand(Task task, TaskService taskService)
    {
        this.task = task;
        this.taskService = taskService;
    }

    public void Execute()
    {
        taskService.SaveTask(task);

        Console.WriteLine("Задача сохранена");
    }

    public void Undo()
    {
        taskService.DeleteTask(task.Id);

        Console.WriteLine("Операция отменена");
    }
}
public class TaskOperationInvoker
{
    private ICommand command;

    public void SetCommand(ICommand command)
    {
        this.command = command;
    }

    public void ExecuteCommand()
    {
        command.Execute();
    }
}

public class Program
{
    public static void Main()
    {
        Task task = new Task.TaskBuilder("Sample Task")
            .WithDescription("Sample task description")
            .WithPriority(1)
            .WithDueDate(DateTime.Now.AddDays(7))
            .Build();

        ITaskService taskService = new TaskServiceProxy();

        ICommand command = new TaskOperationCommand(task, taskService);

        TaskOperationInvoker invoker = new TaskOperationInvoker();
        invoker.SetCommand(command);

        invoker.ExecuteCommand();
    }
}