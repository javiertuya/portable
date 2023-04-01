using NLog;
using System;

namespace Giis.Portable.Util
{
    /// <summary>
    /// Implementation of log methods mapped from slf4j to NLog
    /// </summary>
    public class NLogUtil
    {
        /// <summary>
        /// Returns a new loger with the name of the clazz passed as parameter
        /// </summary>
        public static Logger GetLogger(Type clazz)
        {
            return LogManager.GetLogger(clazz.ToString());
        }

        /// <summary>
        /// Returns a new loger with the name of the current class name
        /// </summary>
        public static Logger GetLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }


        /// <summary>
        /// Las ultimas versiones de NLog cambian el orden de los parametros respecto de los usados por slf4j
        /// en Error y Warn
        /// Cuando se traduce a csharp se tendra un warning de deprecated, pero si se ejecuta dentro
        /// de una libreria causa error, a menos que se desactive este mensajes.
        /// Se opta por cambiar todas las referencias a log.Error(... por SqlRules.Log.Error(log, ...
        /// tras convertir a C# con Sharpen, que ejecutara el log con el orden adecuado de parametros
        /// </summary>
        public static void Error(Logger log, string message, Exception e)
        {
            log.Error(e, message);
        }
        public static void Error(Logger log, string message)
        {
            log.Error(message);
        }
        public static void Error(Logger log, Exception e)
        {
            log.Error(e);
        }
        public static void Warn(Logger log, string message, Exception e)
        {
            log.Warn(e, message);
        }
        public static void Warn(Logger log, string message)
        {
            log.Warn(message);
        }
        public static void Warn(Logger log, Exception e)
        {
            log.Warn(e);
        }

    }
}
