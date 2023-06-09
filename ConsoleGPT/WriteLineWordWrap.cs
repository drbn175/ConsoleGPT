﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGPT
{
    public class WriteLineWordWrap
    {
        public static int WriteLine(string paragraph, int tabSize = 8)
        {
            var counter = 0;
            string[] lines = paragraph
                .Replace("\t", new String(' ', tabSize))
                .Split(new string[] { "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string process = lines[i];
                List<String> wrapped = new List<string>();

                while (process.Length > Console.WindowWidth)
                {
                    int wrapAt = process.LastIndexOf(' ', Math.Min(Console.WindowWidth - 1, process.Length));
                    if (wrapAt <= 0) break;

                    wrapped.Add(process.Substring(0, wrapAt));
                    process = process.Remove(0, wrapAt + 1);
                }

                foreach (string wrap in wrapped)
                {
                    foreach(char c in wrap)
                    {
                        Console.Write(c);
                        Thread.Sleep(50);
                    }
                    Console.WriteLine();
                    counter++;
                }

                foreach (char c in process)
                {
                    Console.Write(c);
                    Thread.Sleep(50);
                }
                Console.WriteLine();

                counter++;
            }
            return counter;
        }
    }
    
}
