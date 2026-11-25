using AseanPhonebookApp;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace AseanPhonebookApp
{
    public sealed class CountryInfo
    {
        public int MenuChoice { get; }
        public string Name { get; }
        public string ShortLabel { get; }
        public int CountryCode { get; }

        public CountryInfo(
            int menuChoice, 
            string name, 
            string shortLabel, 
            int countryCode)
        {
            MenuChoice = menuChoice;
            Name = name;
            ShortLabel = shortLabel;
            CountryCode = countryCode;
        }
    }

    public sealed class PhoneNumber
    {
        public int CountryCode { get; set; }
        public int AreaCode { get; set; }
        public string Number { get; set; }

        public PhoneNumber(
            int countryCode, 
            int areaCode, 
            string number)
        {
            CountryCode = countryCode;
            AreaCode = areaCode;
            Number = number;
        }
        public string ToDashedString() => $"{CountryCode}-{AreaCode}-{Number}";
    }

    public sealed class Student
    {
        public string StudentNumber { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Occupation { get; set; }
        public char Gender { get; set; }
        public PhoneNumber Phone { get; set; }

        public Student(
            string studentNumber,
            string surname,
            string firstName,
            string occupation,
            char gender,
            PhoneNumber phone)
        {
            StudentNumber = studentNumber;
            Surname = surname;
            FirstName = firstName;
            Occupation = occupation;
            Gender = char.ToUpperInvariant(gender);
            Phone = phone;
        }

        //Used a refference code
        public string PossessivePronoun => Gender == 'F' ? "Her" : "His";
        public string FormatEditSummary()
        {
            return $"{FirstName} {Surname} is a {Occupation}. {PossessivePronoun} number is {Phone.ToDashedString()}";
        }
        public string FormatSearchEntry()
        {
            return $"{Surname}, {FirstName}, with student number {StudentNumber}, is a {Occupation}. {PossessivePronoun} phone number is {Phone.ToDashedString()}";
        }
    }

    //This should hold the five asean countries
    public sealed class CountryCatalog
    {
        private readonly List<CountryInfo> _countries;
        public CountryCatalog()
        {
            _countries = new List<CountryInfo>
            {
                new CountryInfo(1, "Republic of the Philippines", "Philippines", 63),
                new CountryInfo(2, "Kingdom of Thailand", "Thailand", 66),
                new CountryInfo(3, "Republic of Singapore", "Singapore", 65),
                new CountryInfo(4, "Republic of Indonesia", "Indonesia", 62),
                new CountryInfo(5, "Federation of Malaysia", "Malaysia", 60)
            };
        }

        /* Used refference code */
        // Exposes the country list as read-only
        public IReadOnlyList<CountryInfo> AllCountries => _countries;
        // Checks whether a given country code matches one of the five allowed ASEAN countries
        public bool IsValidCountryCode(int code) => _countries.Any(c => c.CountryCode == code);
        // Finds a CountryInfo entry matching a search menu number (1-5), or null if not found
        public CountryInfo? FindByMenuChoice(int choice) => _countries.FirstOrDefault(c => c.MenuChoice == choice);
        // Finds a CountryInfo entry matching a numeric country code, or null if not found
        public CountryInfo? FindByCountryCode(int code) => _countries.FirstOrDefault(c => c.CountryCode == code);
    }

    
    //This should manage the in-memory collection of students objects, also the phonebook operation
    public sealed class PhonebookManager
    {
        private readonly List<Student> _students = new List<Student>();
        public IReadOnlyList<Student> Students => _students;

        public void AddStudent(Student student)
        {
            _students.Add(student);
        }

        // Searches for a student by student number
        //Used refference cod
        public Student? FindByStudentNumber(string studentNumber)
        {
            return _students.FirstOrDefault(s =>
                string.Equals(s.StudentNumber, studentNumber, StringComparison.OrdinalIgnoreCase));
        }

        // Filters students by selected country codes (or includes all if includeAll is true) and sorts by surname
        //Used refference code
        public List<Student> SearchByCountryCodes(IEnumerable<int> countryCodes, bool includeAll)
        {
            IEnumerable<Student> query = _students;

            // Filter by country code unless the user selected [6] ALL
            if (!includeAll)
            {
                var codeSet = new HashSet<int>(countryCodes);
                query = query.Where(s => codeSet.Contains(s.Phone.CountryCode));
            }

            // Sort matching students alphabetically by surname, then by first name for ties
            return query
                .OrderBy(s => s.Surname, StringComparer.OrdinalIgnoreCase)
                .ThenBy(s => s.FirstName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    
    //Handles console menus, user prompts, input validation, and calls domain/manager methods
    public sealed class ConsoleApp
    {

        //used a refference codes
        // Reference to the ASEAN country catalog used for validation and menu lookup
        private readonly CountryCatalog _catalog;
        // Reference to the phonebook manager that stores, finds, and searches students
        private readonly PhonebookManager _phonebook;
        // Injects CountryCatalog and PhonebookManager instances into ConsoleApp
        public ConsoleApp(CountryCatalog catalog, PhonebookManager phonebook)
        {
            _catalog = catalog;
            _phonebook = phonebook;
        }
        
        public void Run()
        {
            while (true)
            {
                ShowMainMenu();
                int choice = ReadInt("Enter choice: ");

                switch (choice)
                {
                    case 1:
                        HandleStore();
                        break;
                    case 2:
                        HandleEdit();
                        break;
                    case 3:
                        HandleSearch();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.\n");
                        break;
                }
            }
            private static void ShowMainMenu()
            {
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("[1] Store to ASEAN phonebook");
                Console.WriteLine("[2] Edit entry in ASEAN phonebook");
                Console.WriteLine("[3] Search ASEAN phonebook by country");
                Console.WriteLine("[4] Exit");
            }

            // Prompts for student fields, creates PhoneNumber and Student objects, and stores them
            private void HandleStore()
            {
                // Allow adding one or more students in a row
                do
                {
                    Console.Write("Enter student number: ");
                    string studentNumber = (Console.ReadLine() ?? string.Empty).Trim();
                    Console.Write("Enter surname: ");
                    string surname = (Console.ReadLine() ?? string.Empty).Trim();
                    Console.Write("Enter first name: ");
                    string firstName = (Console.ReadLine() ?? string.Empty).Trim();
                    Console.Write("Enter occupation: ");
                    string occupation = (Console.ReadLine() ?? string.Empty).Trim();
                
                    char gender = ReadGender("Enter gender (M for male, F for female): ");
                    int countryCode = ReadValidCountryCode("Enter country code: ");
                    int areaCode = ReadInt("Enter area code: ");
                
                    Console.Write("Enter number: ");
                    string number = (Console.ReadLine() ?? string.Empty).Trim();

                    // Instantiate PhoneNumber and Student objects and save them to PhonebookManager
                    var phone = new PhoneNumber(countryCode, areaCode, number);
                    var student = new Student(studentNumber, surname, firstName, occupation, gender, phone);
                    _phonebook.AddStudent(student);
                }
                while (ReadYesNo("Do you want to enter another entry [Y/N]? "));

                Console.WriteLine();
            }

            private void HandleEdit()
            {
                Console.Write("Enter student number: ");
                string targetNumber = (Console.ReadLine() ?? string.Empty).Trim();

                Student? student = _phonebook.FindByStudentNumber(targetNumber);
                if (student is null)
                {
                    Console.WriteLine($"Error: Student number '{targetNumber}' was not found.\n");
                    return;
                }

                while (true)
                {
                    Console.WriteLine($"Here is the existing information about {student.StudentNumber}:");
                    Console.WriteLine(student.FormatEditSummary());
                    Console.WriteLine("Which of the following information do you wish to change?");
                    Console.WriteLine("[1] Student number  [2] Surname       [3] Gender       [4] Occupation");
                    Console.WriteLine("[5] Country code    [6] Area code     [7] Phone number");
                    Console.WriteLine("[8] None Go back to main menu");

                    int option = ReadInt("Enter choice: ");
                    switch (option)
                    {
                        case 1:
                            Console.Write("Enter new student number: ");
                            student.StudentNumber = (Console.ReadLine() ?? string.Empty).Trim();
                            break;
                        case 2:
                            Console.Write("Enter new surname: ");
                            student.Surname = (Console.ReadLine() ?? string.Empty).Trim();
                            break;
                        case 3:
                            student.Gender = ReadGender("Enter new gender (M for male, F for female): ");
                            break;
                        case 4:
                            Console.Write("Enter new occupation: ");
                            student.Occupation = (Console.ReadLine() ?? string.Empty).Trim();
                            break;
                        case 5:
                            student.Phone.CountryCode = ReadValidCountryCode("Enter new country code: ");
                            break;
                        case 6:
                            student.Phone.AreaCode = ReadInt("Enter new area code: ");
                            break;
                        case 7:
                            Console.Write("Enter new number: ");
                            student.Phone.Number = (Console.ReadLine() ?? string.Empty).Trim();
                            break;
                        case 8:
                            Console.WriteLine();
                            return;
                        default:
                            Console.WriteLine("Invalid edit option. Please choose 1 to 8.");
                            break;
                    }
                    Console.WriteLine();
                }
            }

            private void Handle Search()
            {
                Console.WriteLine("From which country:");
                Console.WriteLine("[1] Philippines  [2] Thailand  [3] Singapore  [4] Indonesia");
                Console.WriteLine("[5] Malaysia     [6] ALL       [0] No More");

                var selectedCodes = new List<int>();
                var selectedLabels = new List<string>();
                bool includeAll = false;
                int step = 1;

                // Collect country choices until user enters 0 (No More) or 6 (ALL)
                while (true)
                {
                    int choice = ReadInt($"Enter choice {step}: ");
                    step++;

                    if (choice == 0)
                    {
                        break;
                    }

                    if (choice == 6)
                    {
                        includeAll = true;
                        break;
                    }

                    //Map menu selection (1-5) to a CountryInfo record
                    CountryInfo? country = _catalog.FindByMenuChoice(choice);
                    if (country is null)
                    {
                        Console.WriteLine("Invalid country option. Choose 0-6.");
                        step--;
                        continue;
                    }

                    //Avoid adding duplicate country codes if the same option is picked twice
                    if (!selectedCodes.Contains(country.CountryCode))
                    {
                        selectedCodes.Add(country.CountryCode);
                        selectedLabels.Add(country.ShortLabel);
                    }
                }

                //Retrieve matching students
                List<Student> matches = _phonebook.SearchByCountryCodes(selectedCodes, includeAll);

                if (includeAll)
                {
                    Console.WriteLine("Here are the students from ALL ASEAN countries:");
                }
                else if (selectedLabels.Count > 0)
                {
                    Console.WriteLine($"Here are the students from {string.Join(" and ", selectedLabels)}:");
                }
                else
                {
                    Console.WriteLine("No countries selected.");
                }

                //this print each matching student formatted entry
                foreach (Student s in matches)
                {
                    Console.WriteLine(s.FormatSearchEntry());
                }
                Console.WriteLine();
            }

            private int ReadValidCountryCode(string prompt){}

            // Loops until the user types a valid whole number (int)
            private static int ReadInt(string prompt){}

            // Loops until the user enters 'M' or 'F' (case-insensitive)
            private static char ReadGender(string prompt){}
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var catalog = new CountryCatalog();
            var phonebook = new PhonebookManager();
            var app = new ConsoleApp(catalog, phonebook);
            app.Run();
        }
    }
}

/* MAIN MENU */
//[1] Store to ASEAN phonebook
//[2] Edit entry in ASEAN phonebook
//[3] Search ASEAN phonebook by country
//[4] Exit[1] Store to ASEAN phonebook
//[2] Edit entry in ASEAN phonebook
//[3] Search ASEAN phonebook by country
//[4] Exit

/* STORE TO ASEAN PHONEBOOK */
//Enter student number: 2004-56
//Enter surname: Lee
//Enter first name: Sukarno
//Enter occupation: Doctor
//Enter gender (M for male, F for female): M
//Enter country code: 63
//Enter area code: 2
//Enter number: 4567890
//Do you want to enter another entry [Y/N]? NEnter student number: 2004-56
//Enter surname: Lee
//Enter first name: Sukarno
//Enter occupation: Doctor
//Enter gender (M for male, F for female): M
//Enter country code: 63
//Enter area code: 2
//Enter number: 4567890
//Do you want to enter another entry [Y/N]? N

/* EDIT ENTRY IN ASEAN PHONEBOOK */
//Enter student number: 2004-56
//Here is the existing information about 2004-56:
//Sukarno Lee is a Doctor. His number is 63-2-4567890
//Which of the following information do you wish to change?
//[1] Student number [2] Surname [3] Gender [4] Occupation
//[5] Country code [6] Area code [7] Phone number
//[8] None – Go back to main menu
//Enter choice: 1
//Enter new student number: 2005-67Enter student number: 2004-56
//Here is the existing information about 2004-56:
//Sukarno Lee is a Doctor. His number is 63-2-4567890
//Which of the following information do you wish to change?
//[1] Student number [2] Surname [3] Gender [4] Occupation
//[5] Country code [6] Area code [7] Phone number
//[8] None – Go back to main menu
//Enter choice: 1
//Enter new student number: 2005-67
