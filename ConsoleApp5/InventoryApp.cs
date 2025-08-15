using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryLoggingSystem
{
    //  Marker Interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    //  Immutable Inventory Record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    //  Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item) => _log.Add(item);

        public List<T> GetAll() => _log;

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                Console.WriteLine(" Inventory saved to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine(" File not found. No data loaded.");
                    return;
                }

                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                {
                    _log = items;
                    Console.WriteLine(" Inventory loaded from file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error loading from file: {ex.Message}");
            }
        }
    }

    //  InventoryApp Class
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Printer", 3, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Router", 7, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 4, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Keyboard", 10, DateTime.Now));
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            Console.WriteLine("\n Inventory Items:");
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
            }
        }
    }

    //  Main Method
    class Program
    {
        static void Main()
        {
            string filePath = "inventory.json";
            var app = new InventoryApp(filePath);

            // Simulate first session
            app.SeedSampleData();
            app.SaveData();

            // Simulate new session
            Console.WriteLine("\n Simulating new session...");
            var newApp = new InventoryApp(filePath);
            newApp.LoadData();
            newApp.PrintAllItems();

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}


