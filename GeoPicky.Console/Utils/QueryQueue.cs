using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GeoPicky.Console.Utils
{
  public sealed class QueryQueue
  {
    /// <summary>
    ///   The maximum threads.
    /// </summary>
    private readonly int maxThreads;

    /// <summary>
    ///   The name.
    /// </summary>
    private readonly string name;

    /// <summary>
    ///   The tasks.
    /// </summary>
    private readonly List<Task> tasks;

    /// <summary>
    ///   Initializes a new instance of the <see cref="QueryQueue" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="maxThreads">The maximum threads.</param>
    internal QueryQueue(string name, int maxThreads)
    {
      this.name = name;
      this.maxThreads = maxThreads < 1 ? 1 : maxThreads;
      tasks = new List<Task>();
    }

    /// <summary>
    ///   Starts the specified action.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="parameters">The parameters.</param>
    public void Start<TParam>(Action<TParam, int> action, IReadOnlyList<TParam> parameters)
    {
      for (var i = 0; i < parameters.Count; i++)
      {
        var parameter = parameters[i];
        var index = i;

        lock (tasks)
        {
          if (tasks.Count == maxThreads) Task.WaitAny(tasks.ToArray());

          tasks.RemoveAll(t => t.IsCompleted);
          var newTask = Task.Run(() =>
          {
            try
            {
              Trace.WriteLine($"Spawning thread {name}_{tasks.Count}/{maxThreads}");
              action(parameter, index);
            }
            catch (Exception e)
            {
              System.Console.Error.WriteLine(e);
            }
          });

          tasks.Add(newTask);
          Debug.Assert(tasks.Count <= maxThreads, "Thread spawn violation");
        }
      }

      lock (tasks)
      {
        Task.WaitAll(tasks.ToArray());
        tasks.RemoveAll(t => t.IsCompleted);
      }
    }
  }
}