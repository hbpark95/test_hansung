using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HANSUNG.Core.Common
{
    public class BaseService
    {
        public InitConfig _config = new InitConfig();

        // 로그
        //public Logger _logger = NLog.LogManager.GetLogger("logger");
        //public Logger _exceptionLogger = NLog.LogManager.GetLogger("exceptionLogger");
        public Logger _logger = LogManager.GetCurrentClassLogger();
        public Logger _exceptionLogger = LogManager.GetCurrentClassLogger();
    }
}
