namespace Univali.Api.Models;

public class CustomerForCreationWithAddressDto 
{
    public string Name {get;set;} = string.Empty;
    public string Cpf {get;set;} = string.Empty;
    public List<AddressForCreationDto> Addresses {get;set;} = new List<AddressForCreationDto>();
}