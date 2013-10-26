using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;
using System.IO;

namespace LinqyCalculator
{
    class Program
    {
        static void Main()
        {
            var line = "";

            Console.WriteLine("Linqy Calculator");
            Console.WriteLine("Type an expression to evaluate it");
            Console.WriteLine("Type q to quit, and c to clear");
            Console.WriteLine("");

            while (Prompt(out line))
            {
                try
                {
                    if (line.ToLowerInvariant().Trim() == "c") Console.Clear();
                    else
                    {
                        var parsed = ExpressionParser.ParseExpression(line);
                        Console.WriteLine("Parsed as {0}", parsed);
                        Console.Write("Value is ", parsed.Compile()());
                        CWriteLine(parsed.Compile()().ToString(), ConsoleColor.Red);
                    }
                }
                catch (ParseException ex)
                {
                    Console.WriteLine("There was a problem with your input: {0}", ex.Message);
                }

                Console.WriteLine();
            }
        }

        public static void CWriteLine(string text, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static bool Prompt(out string value)
        {
            Console.Write(">> ");
            var line = Console.ReadLine();
            if (line.ToLowerInvariant().Trim() == "q")
            {
                value = null;
                return false;
            }

            value = line;
            return true;
        }
    }
}
