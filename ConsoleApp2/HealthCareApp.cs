using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    //  Generic Repository
    public class Repository<T>
    {
        private List<T> items = new();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => items;
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    //  Patient Class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    //  Prescription Class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    //  HealthSystemApp Class
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new();
        private Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Albet Tetteh", 30, "Male"));
            _patientRepo.Add(new Patient(2, "Jane Mensah", 25, "Female"));
            _patientRepo.Add(new Patient(3, "Shie Kwasi Bismark", 40, "Male"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Paracetamol", DateTime.Today.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Amoxicillin", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Time Herbal Mixture", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Cough Syrup", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(105, 2, "Vitamin C", DateTime.Today));
        }

        public void BuildPrescriptionMap()
        {
            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine(" All Patients:");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
            }
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"\n Prescriptions for Patient ID {id}:");
            if (_prescriptionMap.TryGetValue(id, out var prescriptions))
            {
                foreach (var p in prescriptions)
                {
                    Console.WriteLine($"Prescription ID: {p.Id}, Medication: {p.MedicationName}, Date: {p.DateIssued.ToShortDateString()}");
                }
            }
            else
            {
                Console.WriteLine("No prescriptions found for this patient.");
            }
        }
    }

    //  Main Method
    class Program
    {
        static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            Console.Write("\nEnter Patient ID to view prescriptions: ");
            if (int.TryParse(Console.ReadLine(), out int patientId))
            {
                app.PrintPrescriptionsForPatient(patientId);
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}

