using System;

public class Task
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public DateTime DueDate { get; set; }

    public Task(string title)
    {
        Title = title;
    }
}

public interface ITaskBuilder
{
    ITaskBuilder WithDescription(string description);
    ITaskBuilder WithPriority(int priority);
    ITaskBuilder WithDueDate(DateTime dueDate);
}

public class ConcreteTaskBuilder : ITaskBuilder
{
    private Task task;

    public ConcreteTaskBuilder(string title)
    {
        task = new Task(title);
    }

    public ITaskBuilder WithDescription(string description)
    {
        task.Description = description;
        return this;
    }

    public ITaskBuilder WithPriority(int priority)
    {
        task.Priority = priority;
        return this;
    }

    public ITaskBuilder WithDueDate(DateTime dueDate)
    {
        task.DueDate = dueDate;
        return this;
    }

    public Task GetTask()
    {
        return task;
    }
}

public interface ITaskService
{
    Task GetTaskById(int id);
}

public class TaskService : ITaskService
{
    public Task GetTaskById(int id)
    {
        ITaskBuilder taskBuilder = new ConcreteTaskBuilder("Sample Task")
            .WithDescription("Sample task description")
            .WithPriority(1)
            .WithDueDate(DateTime.Now.AddDays(7));

        return taskBuilder.GetTask();
    }
}

public class TaskServiceProxy : ITaskService
{
    private readonly ITaskService taskService;

    public TaskServiceProxy(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    public Task GetTaskById(int id)
    {
        return taskService.GetTaskById(id);
    }
}

public interface ICommand
{
    void Execute();
    void Undo();
}

public class TaskOperationCommand : ICommand
{
    private readonly Task task;
    private readonly ITaskService taskService;

    public TaskOperationCommand(Task task, ITaskService taskService)
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
        ITaskBuilder taskBuilder = new ConcreteTaskBuilder("Sample Task")
            .WithDescription("Sample task description")
            .WithPriority(1)
            .WithDueDate(DateTime.Now.AddDays(7));

        Task task = ((ConcreteTaskBuilder)taskBuilder).GetTask();
        ITaskService taskService = new TaskServiceProxy(new TaskService());
        Task retrievedTask = taskService.GetTaskById(1);
        ICommand command = new TaskOperationCommand(task, taskService);

        TaskOperationInvoker invoker = new TaskOperationInvoker();
        invoker.SetCommand(command);

        invoker.ExecuteCommand();
    }
}   