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

    [HttpGet("{addressId}", Name = "GetAddress")]
    public ActionResult<AddressDto> GetAddress (int customerId, int addressId) 
    {
        var addressToReturn = Data.Instance
            .Customers.FirstOrDefault(customer => customer.Id == customerId)
            ?.Addresses.FirstOrDefault(address => address.Id == addressId);

        
        return addressToReturn != null ? Ok(addressToReturn) : NotFound();
    }

    [HttpPost]

    public ActionResult<AddressDto> AddAddress (AddressForCreationDto addressForCreationDto, int customerId) 
    {
        var addressEntity = new Address 
        {
            Id = Data.Instance.Customers.SelectMany(c => c.Addresses).Max(a => a.Id) + 1,
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

        return CreatedAtRoute(
            "GetAddress",
            new {
                customerId = customerFromDatabase.Id,
                addressId = addressToReturn.Id
            }
        );
    }

    [HttpDelete("{addressId}")]
    public ActionResult DeleteAddressFromCustomer (int addressId, int customerId) 
    {
        Console.WriteLine($"id: {customerId}");
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(c => c.Id == customerId);

        if(customerFromDatabase == null)
        {
            Console.WriteLine("usuario nao encontrado"); 
            return NotFound();
        } 

        var addressFromDatabase = customerFromDatabase.Addresses.FirstOrDefault(a => a.Id == addressId);

        if(addressFromDatabase == null)
        {
            Console.WriteLine("usuario nao encontrado"); 
            return NotFound();
        } 

        customerFromDatabase.Addresses.Remove(addressFromDatabase);

        return NoContent();
    }

    [HttpPut]
    public ActionResult UpdateAddressesFromCustomer (int customerId, List<AddressForUpdateDto> addressList) 
    {
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(customer => customer.Id == customerId);

        if(customerFromDatabase == null) return NotFound();

        if(customerFromDatabase.Addresses.Any())
        {
            customerFromDatabase.Addresses = new List<Address>();

            foreach(AddressForUpdateDto address in addressList) {
                var addressEntity = new Address {
                Id = Data.Instance.Customers.SelectMany(c => c.Addresses).Max(a => a.Id) + 1,
                Street = address.Street,
                City = address.City
                };
                customerFromDatabase.Addresses.Add(addressEntity);
            }
        } else {
            return BadRequest();
        }
        return NoContent();
    }

}