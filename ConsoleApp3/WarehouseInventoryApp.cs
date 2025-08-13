using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseInventorySystem
{
    //  Marker Interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    //  ElectronicItem Class
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    //  GroceryItem Class
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    //  Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    //  Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => _items.Values.ToList();

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items[id].Quantity = newQuantity;
        }
    }

    //  WareHouseManager Class
    public class WareHouseManager
    {
        public InventoryRepository<ElectronicItem> _electronics = new();
        public InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 10, "Samsung", 12));

            _groceries.AddItem(new GroceryItem(1, "Rice", 50, DateTime.Now.AddMonths(6)));
            _groceries.AddItem(new GroceryItem(2, "Milk", 30, DateTime.Now.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            Console.WriteLine($"\n Inventory of {typeof(T).Name}:");
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var current = repo.GetItemById(id);
                repo.UpdateQuantity(id, current.Quantity + quantity);
                Console.WriteLine($" Updated quantity for {current.Name} to {current.Quantity + quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($" Item with ID {id} removed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }
        }
    }

    //  Main Method
    class Program
    {
        static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            manager.PrintAllItems(manager._groceries);
            manager.PrintAllItems(manager._electronics);

            // Trigger exceptions
            try
            {
                manager._groceries.AddItem(new GroceryItem(1, "Duplicate Rice", 20, DateTime.Now.AddMonths(3)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Duplicate Add Error: {ex.Message}");
            }

            manager.RemoveItemById(manager._electronics, 99); // Non-existent
            manager.IncreaseStock(manager._groceries, 2, -5);  // Invalid quantity

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}

