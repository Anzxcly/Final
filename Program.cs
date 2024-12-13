using System;
using System.Collections.Generic;
using System.IO;
using Spectre.Console;

namespace HealthcareManagementSystem
{
    public class HealthcareEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public HealthcareEntity(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;
        }
    }

    class HealthcareFacility : HealthcareEntity
    {
        public HealthcareFacility(string name, string address, string phone)
            : base(name, address, phone)
        {
        }
    }

    class FacilityManager : HealthcareEntity
    {
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public string ExpirationDate { get; set; }
        public string Type { get; set; }

        public FacilityManager(string name, string address, string phone, string itemID, int quantity, string expirationDate, string type)
            : base(name, address, phone)
        {
            ItemID = itemID;
            Quantity = quantity;
            ExpirationDate = expirationDate;
            Type = type;
        }

        public void UpdateItem(string newItemID, int newQuantity, string newExpirationDate, string newType)
        {
            ItemID = newItemID;
            Quantity = newQuantity;
            ExpirationDate = newExpirationDate;
            Type = newType;
        }
    }

    class InventoryManager
    {
        private List<FacilityManager> items = new List<FacilityManager>();
        private string inventoryFilePath = "inventory.txt";

        public void AddItem(FacilityManager item)
        {
            items.Add(item);

            string itemData = $"{item.ItemID},{item.Name},{item.Quantity},{item.ExpirationDate},{item.Type}";
            File.AppendAllText(inventoryFilePath, itemData + Environment.NewLine);
            Console.WriteLine("Item added to inventory successfully.");
        }

        public void ViewItems()
        {
            Console.Clear();

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Item Name");
            table.AddColumn("Quantity");
            table.AddColumn("Expiration Date");
            table.AddColumn("Type");

            try
            {
                if (items.Count == 0)
                {
                    LoadItemsFromFile();
                }

                foreach (var item in items)
                {
                    table.AddRow(item.ItemID, item.Name, item.Quantity.ToString(), item.ExpirationDate, item.Type);
                }

                AnsiConsole.Render(table);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading inventory file: {ex.Message}");
            }

            Console.ReadKey();
        }

        public void LoadItemsFromFile()
        {
            items.Clear();
            try
            {
                using (StreamReader reader = new StreamReader(inventoryFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');
                        if (data.Length == 5)
                        {
                            items.Add(new FacilityManager(
                                data[1].Trim(), "N/A", "N/A", data[0].Trim(), int.Parse(data[2].Trim()), data[3].Trim(), data[4].Trim()));
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Inventory file not found. Please add items to inventory.");
            }
        }

        public void SaveItemsToFile()
        {
            using (StreamWriter writer = new StreamWriter(inventoryFilePath))
            {
                foreach (var item in items)
                {
                    writer.WriteLine($"{item.ItemID},{item.Name},{item.Quantity},{item.ExpirationDate},{item.Type}");
                }
            }
        }

        public FacilityManager SearchItemByID(string itemID)
        {
            return items.Find(i => i.ItemID.Equals(itemID, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateItem(string itemID, string newItemID, int newQuantity, string newExpirationDate, string newType)
        {
            var item = SearchItemByID(itemID);
            if (item != null)
            {
                item.UpdateItem(newItemID, newQuantity, newExpirationDate, newType);
                SaveItemsToFile();
                Console.WriteLine("Item updated successfully.");
            }
            else
            {
                Console.WriteLine("Item not found.");
            }

            Console.ReadKey();
        }

        public void DeleteItem(string itemID)
        {
            var item = SearchItemByID(itemID);
            if (item != null)
            {
                items.Remove(item);
                SaveItemsToFile();
                Console.WriteLine("Item deleted successfully.");
            }
            else
            {
                Console.WriteLine("Item not found.");
            }

            Console.ReadKey();
        }
    }

    class Program
    {
        static List<HealthcareFacility> facilities = new List<HealthcareFacility>();
        static InventoryManager inventoryManager = new InventoryManager();
        private static string facilitiesFilePath = "facilities.txt";

        static void Main(string[] args)
        {
            LoadFacilitiesFromFile();
            inventoryManager.LoadItemsFromFile();

            int choice;
            do
            {
                Console.Clear();
                Console.WriteLine("--- Cebu Healthcare Facility Management System ---");
                Console.WriteLine("1. Add a Healthcare Facility");
                Console.WriteLine("2. Search Healthcare Facility by Name");
                Console.WriteLine("3. Update Healthcare Facility Information");
                Console.WriteLine("4. Delete Healthcare Facility Record");
                Console.WriteLine("5. View Healthcare Facilities");
                Console.WriteLine("6. Manage Inventory");
                Console.WriteLine("7. Exit");
                Console.Write("Please choose an option (1-7): ");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AddHealthcareFacility();
                        break;
                    case 2:
                        SearchHealthcareFacility();
                        break;
                    case 3:
                        UpdateHealthcareFacility();
                        break;
                    case 4:
                        DeleteHealthcareFacility();
                        break;
                    case 5:
                        ViewHealthcareFacilities();
                        break;
                    case 6:
                        ManageInventory(); 
                        break;
                    case 7:
                        Console.WriteLine("Exiting the program...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                PauseMenu();

            } while (choice != 7); 
        }

        static void ViewHealthcareFacilities()
        {
            Console.Clear();
            Console.WriteLine("--- View Healthcare Facilities ---");

            if (facilities.Count == 0)
            {
                Console.WriteLine("No healthcare facilities available.");
                return;
            }

            var table = new Table();
            table.AddColumn("Facility Name");
            table.AddColumn("Address");
            table.AddColumn("Phone");

            foreach (var facility in facilities)
            {
                table.AddRow(facility.Name, facility.Address, facility.Phone);
            }

            AnsiConsole.Render(table);

            Console.ReadKey();
        }

        static void AddHealthcareFacility()
        {
            Console.Clear();
            Console.WriteLine("--- Add a Healthcare Facility ---");
            Console.Write("Enter Facility Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Facility Address: ");
            string address = Console.ReadLine();
            Console.Write("Enter Facility Phone Number: ");
            string phone = Console.ReadLine();

            facilities.Add(new HealthcareFacility(name, address, phone));
            SaveFacilitiesToFile();

            Console.WriteLine($"Healthcare Facility '{name}' added successfully.");

            Console.ReadKey();
        }

        static void SearchHealthcareFacility()
        {
            Console.Clear();
            Console.WriteLine("--- Search Healthcare Facility by Name ---");
            Console.Write("Enter Facility Name to search: ");
            string name = Console.ReadLine();

            var facility = facilities.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Address");
            table.AddColumn("Phone");

            if (facility != null)
            {
                // Add the found facility to the table
                table.AddRow(facility.Name, facility.Address, facility.Phone);
                AnsiConsole.Render(table); // Display the table
            }
            else
            {
                Console.WriteLine($"Healthcare Facility '{name}' not found.");
            }

            Console.ReadKey();
        }

        static void UpdateHealthcareFacility()
        {
            Console.Clear();
            Console.WriteLine("--- Update Healthcare Facility Information ---");
            Console.Write("Enter Facility Name to update: ");
            string name = Console.ReadLine();

            var facility = facilities.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (facility != null)
            {
                Console.Write("Enter New Facility Address: ");
                string newAddress = Console.ReadLine();
                Console.Write("Enter New Facility Phone: ");
                string newPhone = Console.ReadLine();

                facility.Name = name;
                facility.Address = newAddress;
                facility.Phone = newPhone;

                SaveFacilitiesToFile();
                Console.WriteLine("Facility information updated successfully.");
            }
            else
            {
                Console.WriteLine($"Facility '{name}' not found.");
            }

            Console.ReadKey();
        }

        static void DeleteHealthcareFacility()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Healthcare Facility Record ---");
            Console.Write("Enter Facility Name to delete: ");
            string name = Console.ReadLine();

            var facility = facilities.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (facility != null)
            {
                facilities.Remove(facility);
                SaveFacilitiesToFile();
                Console.WriteLine($"Facility '{name}' deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Facility '{name}' not found.");
            }

            Console.ReadKey();
        }

        static void ManageInventory()
        {
            int choice;
            do
            {
                Console.Clear();
                Console.WriteLine("--- Manage Inventory ---");
                Console.WriteLine("1. Add New Item");
                Console.WriteLine("2. View Items");
                Console.WriteLine("3. Update Item");
                Console.WriteLine("4. Delete Item");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Choose an option: ");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AddNewItem();
                        break;
                    case 2:
                        inventoryManager.ViewItems();
                        break;
                    case 3:
                        UpdateInventoryItem();
                        break;
                    case 4:
                        DeleteInventoryItem();
                        break;
                    case 5:
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            } while (choice != 5);
        }

        static void AddNewItem()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Item to Inventory ---");
            Console.Write("Enter Item ID: ");
            string itemID = Console.ReadLine();
            Console.Write("Enter Item Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Quantity: ");
            int quantity = int.Parse(Console.ReadLine());
            Console.Write("Enter Expiration Date (YYYY-MM-DD): ");
            string expirationDate = Console.ReadLine();
            Console.Write("Enter Item Type: ");
            string type = Console.ReadLine();

            FacilityManager newItem = new FacilityManager(name, "N/A", "N/A", itemID, quantity, expirationDate, type);
            inventoryManager.AddItem(newItem);

            Console.ReadKey();
        }

        static void UpdateInventoryItem()
        {
            Console.Clear();
            Console.WriteLine("--- Update Inventory Item ---");
            Console.Write("Enter Item ID to update: ");
            string itemID = Console.ReadLine();
            Console.Write("Enter New Item ID: ");
            string newItemID = Console.ReadLine();
            Console.Write("Enter New Quantity: ");
            int newQuantity = int.Parse(Console.ReadLine());
            Console.Write("Enter New Expiration Date (YYYY-MM-DD): ");
            string newExpirationDate = Console.ReadLine();
            Console.Write("Enter New Item Type: ");
            string newType = Console.ReadLine();

            inventoryManager.UpdateItem(itemID, newItemID, newQuantity, newExpirationDate, newType);

            Console.ReadKey();
        }

        static void DeleteInventoryItem()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Inventory Item ---");
            Console.Write("Enter Item ID to delete: ");
            string itemID = Console.ReadLine();
            inventoryManager.DeleteItem(itemID);
        }

        static void SaveFacilitiesToFile()
        {
            using (StreamWriter writer = new StreamWriter(facilitiesFilePath))
            {
                foreach (var facility in facilities)
                {
                    writer.WriteLine($"{facility.Name},{facility.Address},{facility.Phone}");
                }
            }
        }

        static void LoadFacilitiesFromFile()
        {
            try
            {
                if (File.Exists(facilitiesFilePath))
                {
                    using (StreamReader reader = new StreamReader(facilitiesFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var data = line.Split(',');
                            if (data.Length == 3)
                            {
                                facilities.Add(new HealthcareFacility(data[0], data[1], data[2]));
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error loading facility data: {ex.Message}");
            }
        }

        static void PauseMenu()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
