using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Mp3
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide path to mp3 file as argument.");
                return;
            }

            string filePath = args[0];

            try
            {
                using (Stream input = new FileStream(filePath, FileMode.Open))
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    var decoder = new Mp3Decoder(input);

                    int count = 0;
                    foreach (Mp3Frame frame in decoder.GetFrames())
                    {
                        count++;
                    }

                    sw.Stop();
                    Console.WriteLine($"read {count} frames in {sw.ElapsedMilliseconds} milliseconds");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message} at {e.StackTrace}");
            }
        }
    }
}
