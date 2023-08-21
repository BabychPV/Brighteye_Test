using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Brighteye_Test
{
    public class ExceptionHandler
    {
        public static void HandleException(string operationName, Exception ex)
        {
            string errorMessage = $"An error occurred while {operationName}. Message: {ex.Message}; InnerException: {ex.InnerException}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
