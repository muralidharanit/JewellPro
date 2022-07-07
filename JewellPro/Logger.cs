using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JewellPro
{
    public class Logger
    {
        public static void LogInfo(string info)
        {

        }

        public static void LogWarning(string warning)
        {

        }

        public static void LogError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //Write into log file
        }
    }
}
