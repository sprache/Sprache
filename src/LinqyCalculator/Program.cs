using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;
<<<<<<< HEAD
=======
using System.IO;
using System.Linq.Expressions;
>>>>>>> 2fdb01cfa222e4c6837545eca875f9fc99999255

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
<<<<<<< HEAD
                    var parsed = ExpressionParser.ParseExpression(line);
                    Console.WriteLine("Parsed as {0}", parsed);
                    Console.WriteLine("Value is {0}", parsed.Compile()());
=======
                    if (line.ToLowerInvariant().Trim() == "c") Console.Clear();
                    else
                    {
                        var parsed = LangParser.Line.Parse(line);
                        Console.Write("Value is ");
                        CWriteLine(parsed.ToString(), ConsoleColor.Red);
                    }
>>>>>>> 2fdb01cfa222e4c6837545eca875f9fc99999255
                }
                catch (ParseException ex)
                {
                    Console.WriteLine("There was a problem with your input: {0}", ex.Message);
                }

                Console.WriteLine();
            }
        }

<<<<<<< HEAD
=======
        public static void CWriteLine(string text, ConsoleColor c)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(text);
            Console.ForegroundColor = currentColor;
        }

>>>>>>> 2fdb01cfa222e4c6837545eca875f9fc99999255
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
