using log4net;
using log4net.Config;
using System.Reflection;
using System.Xml;

namespace TMSClient.AppLogger
{
    public class LoggerManager : ILoggerManager
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
        public LoggerManager()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(File.OpenRead("log4net.config"));
                var repo = LogManager.CreateRepository(Assembly.GetExecutingAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                XmlConfigurator.Configure(repo, xmlDocument["log4net"]);
                _logger.Info("Log System Initialized");
            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }
        public void LogInformation(string message)
        {
            _logger.Info(message);
        }
    }

}
