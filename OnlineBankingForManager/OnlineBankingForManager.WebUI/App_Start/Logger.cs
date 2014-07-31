// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="">
//   
// </copyright>
// <summary>
//   The logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using log4net;
using log4net.Config;

/// <summary>
/// The logger.
/// </summary>
/// Logger - shell for log4net logger
public static class Logger
{
    /// <summary>
    /// The log.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

    /// <summary>
    /// Gets the log.
    /// </summary>
    public static ILog Log
    {
        get
        {
            return log;
        }
    }

    /// <summary>
    /// The init logger.
    /// </summary>
    public static void InitLogger()
    {
        XmlConfigurator.Configure();
    }
}