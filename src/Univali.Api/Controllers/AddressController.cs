using Microsoft.AspNetCore.Mvc;
using Univali.Api.Entities;
using Univali.Api.Models;

namespace Univali.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId}/addresses")]
public class AddressController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<AddressDto>> GetAddresses (int customerId)
    {
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(customer =>customer.Id == customerId);

        if(customerFromDatabase == null) return NotFound();

        var addressesToReturn = new List<AddressDto>();

        foreach(var address in customerFromDatabase.Addresses)
        {
            addressesToReturn.Add(new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City
            });
        }
        return Ok(addressesToReturn);
    }

    [HttpGet("{addressId}")]
    public ActionResult<AddressDto> GetAddress (int customerId, int addressId) 
    {
        var addressToReturn = Data.Instance
            .Customers.FirstOrDefault(customer => customer.Id == customerId)
            ?.Addresses.FirstOrDefault(address => address.Id == addressId);

        
        return addressToReturn != null ? Ok(addressToReturn) : NotFound();
    }

    [HttpPost("{customerId}")]

    public ActionResult<AddressDto> AddAddress (AddressForCreationDto addressForCreationDto, int customerId) 
    {
        var addressEntity = new Address 
        {
            Street = addressForCreationDto.Street,
            City = addressForCreationDto.City
        };

        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(c => c.Id == customerId);

        if(customerFromDatabase == null) return NotFound();

        customerFromDatabase.Addresses.Add(addressEntity);

        var addressToReturn = new AddressDto 
        {
            Id = addressEntity.Id,
                Street = addressEntity.Street,
                City = addressEntity.City
        };

        return Ok(addressToReturn);
    }

    [HttpDelete("{idAddress}")]

    public ActionResult DeleteAddressFromCustomer (int addressId, int customerId) 
    {
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(c => c.Id == customerId);

        if(customerFromDatabase == null) return NotFound();

        var addressFromDatabase = customerFromDatabase.Addresses.FirstOrDefault(a => a.Id == addressId);

        if(addressFromDatabase == null) return NotFound();

        customerFromDatabase.Addresses.Remove(addressFromDatabase);

        return NoContent();
    }
    
}