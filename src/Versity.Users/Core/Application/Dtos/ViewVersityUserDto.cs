using Domain.Models;

namespace Application.Dtos;

public record ViewVersityUserDto(
    string Id,
    string FirstName, 
    string LastName,
    string Email,
    string Phone,
    IEnumerable<string> Role)
{
    public static ViewVersityUserDto MapFromModel(VersityUser model, IEnumerable<string> roles)
    {
        return new ViewVersityUserDto(
            model.Id, 
            model.FirstName, 
            model.LastName, 
            model.Email,
            model.PhoneNumber,
            roles);
    }
}