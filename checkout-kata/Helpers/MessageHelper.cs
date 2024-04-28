using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkout_kata.Helpers
{
    public class MessageHelper : IMessageHelper
    {
        public void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
