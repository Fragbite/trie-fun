using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trie
{
    class Program
    {
        static void Main(string[] args)
        {
            var generated = GenerateValues();
            Console.WriteLine("Values generated");

            var trie = new Trie(generated);
            Console.WriteLine("Trie created");

            Console.WriteLine("Switching control");
            TakeControl(trie);
        }

        private static void TakeControl(Trie trie)
        {
            var userCommand = ReadCommand();
            while (userCommand.ToLowerInvariant() != "q")
            {
                var split = userCommand.Split(new[] { ' ' });
                var cmd = split[0];
                Action action = null;
                switch (cmd)
                {
                    case "add":
                        {
                            var value = split[1];
                            action = (() =>
                                {
                                    trie.AddValue(value);
                                });
                        }
                        break;
                    case "sort":
                        {
                            action = (() =>
                                {
                                    trie.OrderTree();
                                });
                        }
                        break;
                    case "print":
                        {
                            if (split.Length > 1)
                            {
                                var value = split[1];
                                action = (() =>
                                {
                                    trie.SaveToFile(value);
                                });
                            }
                            else
                            {
                                action = (() =>
                                {
                                    trie.PrintTree();
                                });
                            }
                        }
                        break;
                    case "find":
                        {
                            var value = split[1];
                            action = (() =>
                            {
                                trie.Find(value);
                            });
                        }
                        break;
                    case "stats":
                        {
                            action = (() =>
                            {
                                trie.PrintStats();
                            });
                        }
                        break;
                    case "id":
                        {
                            var value = split[1];
                            action = (() =>
                            {
                                trie.FindById(value);
                            });
                        }
                        break;
                    case "generate-random":
                        {
                            var count = int.Parse(split[1]);
                            var minLength = int.Parse(split[2]);
                            var maxLength = int.Parse(split[3]);
                            action = (() =>
                            {
                                Console.WriteLine("Clearing trie");
                                trie.Clear();
                                Console.WriteLine("Generating {0} random values", count);
                                var items = GenerateRandomStrings(count, minLength, maxLength);
                                Console.WriteLine("Building trie");
                                trie = new Trie(items);
                            });
                        }
                        break;
                    case "help":
                        {
                            Console.WriteLine();
                            Console.WriteLine("USAGE:");
                            Console.WriteLine("\t - add {value} - add value to tree");
                            Console.WriteLine("\t - sort - sort the tree");
                            Console.WriteLine("\t - print [{file_name}] - print data to screen (or file)");
                            Console.WriteLine("\t - find {value} - find value in the tree");
                            Console.WriteLine("\t - id {value} - find node by id");
                            Console.WriteLine("\t - stats - display tree stats");
                            Console.WriteLine("\t - generate-random {count} {minLength} {maxLength} - clear tree and generate random values");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }

                if (action != null)
                {
                    ExecuteAndMeasure(action);
                }

                userCommand = ReadCommand();
            }
        }

        private static void ExecuteAndMeasure(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine(new String('-', 10));
            action();
            Console.WriteLine(new String('-', 10));

            stopwatch.Stop();
            Console.WriteLine("Command took: {0} ms / {1} ticks", stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks);
        }

        private static string ReadCommand()
        {
            Console.WriteLine();
            Console.Write("Input command: ");
            return Console.ReadLine();
        }

        private static List<string> ReadDataFromUserInput()
        {
            var input = new List<string>();

            var userInput = Console.ReadKey();
            var currentWord = new StringBuilder();
            while (userInput.Key != ConsoleKey.Enter)
            {
                if (userInput.Key == ConsoleKey.Spacebar)
                {
                    input.Add(currentWord.ToString());
                    currentWord.Clear();
                }
                else
                {
                    currentWord.Append(userInput.KeyChar);
                }
                userInput = Console.ReadKey();
            }

            return input;
        }

        private static List<string> GenerateRandomStrings(int count, int minLenght, int maxlenght)
        {
            var input = new List<string>();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLowerInvariant();
            var random = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < count; i++)
            {
                var length = random.Next(minLenght, maxlenght);
                input.Add(new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()));
            }

            return input;
        }

        private static List<string> GenerateValues()
        {
            return Regex.Replace(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.txt")), @"\.|\,|\'|\-.*|\r\n?|\n", " ").ToLowerInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

    }
}
