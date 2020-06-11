using System;

namespace GeoPicky.Console.Utils
{
  /// <summary>
  ///   Multitasking.
  /// </summary>
  public class Multitask
  {
    /// <summary>
    ///   The pool size
    /// </summary>
    private static int poolSize;

    /// <summary>
    ///   Initializes static members of the <see cref="Multitask" /> class.
    /// </summary>
    static Multitask()
    {
      PoolSize = Environment.ProcessorCount;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="QueryQueue" /> class.
    /// </summary>
    public Multitask() : this(PoolSize)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Multitask" /> class.
    /// </summary>
    /// <param name="maxThreads">The maximum threads.</param>
    public Multitask(int maxThreads)
    {
      Queue = new QueryQueue(GetType().Name.GetValidThreadName(), maxThreads);
    }

    /// <summary>
    ///   The default threads count.
    /// </summary>
    public static int PoolSize
    {
      get => poolSize;

      set
      {
        if (value < 1) throw new ArgumentException("value");
        ;

        poolSize = value;
      }
    }

    /// <summary>
    ///   Gets the queue.
    /// </summary>
    public QueryQueue Queue { get; }
  }
}