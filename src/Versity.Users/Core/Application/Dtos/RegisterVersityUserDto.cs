namespace Application.Dtos;

public record RegisterVersityUserDto(
    string FirstName, 
    string LastName,
    string Email,
    string Phone,
    string Password);