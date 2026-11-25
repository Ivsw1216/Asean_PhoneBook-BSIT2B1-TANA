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
        public string PhoneNumber { get; set; }

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
            Gender = gender;
            Phone = phone;
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
            //

            private void HandleEdit()
            {
            
            }

            private void Handle Search()
            {
            
            }

            private int ReadValidCountryCode(string prompt){}

            // Loops until the user types a valid whole number (int)
            private static int ReadInt(string prompt){}

            // Loops until the user enters 'M' or 'F' (case-insensitive)
            private static char ReadGender(string prompt){}
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
