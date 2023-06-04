namespace Univali.Api.Models;

public class CustomerDtoWithAddress
{
    public int Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Cpf {get;set;} = string.Empty;
    public List<AddressDto> Addresses = new List<AddressDto>();
}