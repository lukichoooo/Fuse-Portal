namespace Core.Dtos;

public class ValidatorSettings
{
    // atuh
    public int PasswordMinLength { get; set; }
    public int PasswordMaxLength { get; set; }
    public int NameMinLength { get; set; }
    public int NameMaxLength { get; set; }

    // address
    public int CityMinLength { get; set; }
    public int CityMaxLength { get; set; }

    // university
    public int UniNameMaxLength { get; set; }
    public int UniNameMinLength { get; set; }

    // chat
    public int ChatNameMaxLength { get; set; }
    public int MessageMaxLength { get; set; }


}
