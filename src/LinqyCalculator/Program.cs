using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;
using System.IO;
using System.Linq.Expressions;

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
                        var parsed = LangParser.Line.Parse(line);
                        Console.Write("Value is ");
                        CWriteLine(parsed.ToString(), ConsoleColor.Red);
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
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(text);
            Console.ForegroundColor = currentColor;
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
