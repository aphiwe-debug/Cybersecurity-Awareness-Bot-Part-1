using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybersecurity_Awareness_Bot_Part_1
{
    // SERVICE: AsciiArtService — logo displayed on launch

    public static class AsciiArtService
    {
        private const string Logo = @"
_________        ___.                                                  .__  __          
\_   ___ \___.__.\_ |__   ___________  ______ ____   ____  __ _________|__|/  |_ ___.__.
/    \  \<   |  | | __ \_/ __ \_  __ \/  ___// __ \_/ ___\|  |  \_  __ \  \   __<   |  |
\     \___\___  | | \_\ \  ___/|  | \/\___ \\  ___/\  \___|  |  /|  | \/  ||  |  \___  |
 \______  / ____| |___  /\___  >__|  /____  >\___  >\___  >____/ |__|  |__||__|  / ____|
        \/\/          \/     \/           \/     \/     \/                       \/     
 
              A W A R E N E S S   B O T                          
";

        public static void Display()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(Logo);
            Console.ResetColor();
        }
    }

}
