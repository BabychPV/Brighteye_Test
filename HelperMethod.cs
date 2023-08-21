using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Brighteye_Test
{
    static class HelperMethod
    {
        public static int GenerateRandomValue()
        {
            Random random = new Random();
            return random.Next(0, 11); // Generates a random value between 0 (inclusive) and 11 (exclusive)
        }

        public static bool HasProperty(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }


    }
}
