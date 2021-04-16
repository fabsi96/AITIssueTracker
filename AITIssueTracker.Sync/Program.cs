using System;

namespace AITIssueTracker.Sync
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== GitHub Synchronisierung ===");
            Console.Write($"Projektname: ");
            string projectName = Console.ReadLine();
            Console.WriteLine($"{projectName}");



            Console.ReadKey();
        }
    }
}
