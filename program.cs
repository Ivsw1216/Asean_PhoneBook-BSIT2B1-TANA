using System;

namespace AseanPhonebookApp
{
  public sealed class CountryInfo
  {
    public int MenuChoice {get;}
    public string Name {get;}
    public string ShortLabel {get;}
    public int CountryCode {get;}

    public CountryInfo(int menuChoice, string name, string shortLabel, int countryCode)
    {
      MenuChoice = menuChoice;
      Name = name;
      ShortLabel = shortLabel;
      CountryCode = countryCode;
    }
  }

  public sealed class PhoneNumber
  {
    public int CountryCode {get; set;}
    public int AreaCode {get; set;}
    public string Number {get; set;}
  }
  public PhoneNumber(int countryCode, int areaCode, string Number)
  {
    
  }
  
}
