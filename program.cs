using AseanPhonebookApp;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics.Metrics;
using System.Numerics;

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
    }

    public sealed class Student
    {
        public string StudentNumber { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }

        public string Occupation { get; set; }
        public char Gender { get; set; }

        public int CountryCode { get; set; }
        public int AreaCode { get; set; }
        public string PhoneNumber { get; set; }

        public Student(
            string studentNumber,
            string surname,
            string firstName,
            string occupation,
            char gender,
            int countryCode,
            int areaCode,
            string phoneNumber)
        {
            StudentNumber = studentNumber;
            Surname = surname;
            FirstName = firstName;

            Occupation = occupation;
            Gender = gender;

            CountryCode = countryCode;
            AreaCode = areaCode;
            PhoneNumber = phoneNumber;
        }
    }

    public sealed class CountryCatalog
    {

    }

    public sealed class PhonebookManager
    {

    }

    public sealed class ConsoleApp
    {

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
