using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace LinqyCalculator
{
    class Program
    {
        static void Main()
        {
            var line = "";
            while (Prompt(out line))
            {
                try
                {
                    var parsed = ExpressionParser.ParseExpression(line);
                    Console.WriteLine("Parsed as {0}", parsed);
                    Console.WriteLine("Value is {0}", parsed.Compile()());
                }
                catch (ParseException ex)
                {
                    Console.WriteLine("There was a problem with your input: {0}", ex.Message);
                }

                Console.WriteLine();
            }
        }

        static bool Prompt(out string value)
        {
            Console.Write("Enter a numeric expression, or 'q' to quit: ");
            var line = Console.ReadLine();
            if (line.ToLowerInvariant().Trim() == "q")
            {
                value = null;
                return false;
            }
            else
            {
                value = line;
                return true;
            }
        }
    }
}