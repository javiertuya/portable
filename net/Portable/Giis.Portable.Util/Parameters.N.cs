using System.Net;

namespace Giis.Portable.Util
{
    /// <summary>
    /// Platform specific configuration for Java/C# compatibility
    /// </summary>
    public static class Parameters
    {
        //plataforma donde se ejecuta [netcore|netframework]
        //A partir de la v2.2 abandona netframework, siempre sera netcore
        private static readonly string PlatformName = "netcore";
        //cuatro niveles arriba (el proyecto se ejecuta en proyecto/bin/debug/netcoreappX.X
        public static readonly string DefaultProjectRoot = FileUtil.GetPath("..", "..", "..", "..");
        public static readonly string DefaultReportSubdir = "reports";

        //path relativo a la raiz del proyecto tal como se ve en tiempo de ejecucion:
        private static string ProjectRoot;
	    //path relativo a projectRoot donde se encuentran los reports generados por un build
        private static string ReportSubdir;

        static Parameters()
        {
            SetDefault();
        }
        public static void SetDefault()
        {
            //No hay forma estandar de determinar si se esta ejecutando en entorno hosted (web) o no (consola o test)
            //por lo que primero examina una variable de entorno que si esta configurada determina la ruta relativa a la raiz
            //(usado cuando se ejecuta en containers)
            //Los metodos como usar HostingEnvironment comprobando IsHosted requieren las librerias de asp.net
            string envRoot = System.Environment.GetEnvironmentVariable("HOSTED_APP_ROOT");
            if (!string.IsNullOrEmpty(envRoot))
                ProjectRoot = envRoot;
            else
                ProjectRoot = DefaultProjectRoot;
            ReportSubdir = DefaultReportSubdir;
        }

        public static string GetPlatformName()
        {
            return PlatformName;
        }
        public static bool IsJava()
        {
            return false;
        }
        public static bool IsNetCore()
        {
            return PlatformName == "netcore";
        }

        public static string GetProjectRoot()
        {
            return ProjectRoot;
        }
        public static void SetProjectRoot(string root)
        {
            ProjectRoot = root;
        }
        public static string GetReportSubdir()
        {
            return ReportSubdir;
        }
        public static void SetReportSubdir(string subdir)
        {
            ReportSubdir = subdir;
        }

        /// <summary>
        /// Direccion ip V4 del equipo donde se ejecuta este programa. Si hay varias devuelve la primera
        /// </summary>
        public static string GetIpV4Address()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            for (int i = 0; i < addr.Length; i++)
            {
                if (addr[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return addr[i].ToString();
            }
            throw new PortableException("GetIpV4Address: No IP V4 address found");
        }
    }
}
