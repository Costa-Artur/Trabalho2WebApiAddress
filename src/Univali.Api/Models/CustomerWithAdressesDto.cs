namespace Univali.Api.Models;

public class CustomerWithAdressesDto
{
    public int Id{get;set;}
    public string Name {get;set;} = string.Empty;
    public string Cpf{get;set;} = string.Empty;
    public ICollection<AddressDto> Adresses {get;set;} = new List<AddressDto>();
}