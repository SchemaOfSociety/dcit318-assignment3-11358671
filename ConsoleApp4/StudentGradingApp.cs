using System;
using System.Collections.Generic;
using System.IO;

namespace StudentGradingSystem
{
    //  Custom Exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    //  Student Class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            return Score switch
            {
                >= 80 => "A",
                >= 70 => "B",
                >= 60 => "C",
                >= 50 => "D",
                _ => "F"
            };
        }
    }

    //  StudentResultProcessor Class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using var reader = new StreamReader(inputFilePath);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Missing fields in line: {line}");

                if (!int.TryParse(parts[0], out int id))
                    throw new MissingFieldException($"Invalid ID format in line: {line}");

                string fullName = parts[1].Trim();

                if (!int.TryParse(parts[2], out int score))
                    throw new InvalidScoreFormatException($"Invalid score format in line: {line}");

                students.Add(new Student(id, fullName, score));
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var writer = new StreamWriter(outputFilePath);
            foreach (var student in students)
            {
                string summary = $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}";
                writer.WriteLine(summary);
            }
        }
    }

    //  Main Method
    class Program
    {
        static void Main()
        {
            var processor = new StudentResultProcessor();
            string inputPath = "students.txt";
            string outputPath = "report.txt";

            try
            {
                var students = processor.ReadStudentsFromFile(inputPath);
                processor.WriteReportToFile(students, outputPath);
                Console.WriteLine(" Report generated successfully.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(" Input file not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($" Score format error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($" Missing field error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Unexpected error: {ex.Message}");
            }

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}

