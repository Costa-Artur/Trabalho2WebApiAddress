using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Univali.Api.Entities;
using Univali.Api.Models;

namespace Univali.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CustomerDto>> GetCustomers()
    {
        var customersToReturn = Data.Instance.Customers.Select(customer => new CustomerDto()
        {
            Id = customer.Id,
            Name = customer.Name,
            Cpf = customer.Cpf
        });
        return Ok(customersToReturn);
    }

    [HttpGet("{id}", Name = "GetCustomerById")]
    public ActionResult<CustomerDto> GetCustomerById(int id)
    {
        Console.WriteLine($"id: {id}");
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(c => c.Id == id);

        if (customerFromDatabase == null) return NotFound();

        CustomerDto customerToReturn = new CustomerDto()
        {
            Id = customerFromDatabase.Id,
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };
        return Ok(customerToReturn);
    }

    [HttpGet("cpf/{cpf}")]
    public ActionResult<CustomerDto> GetCustomerByCpf(string cpf)
    {
        Console.WriteLine($"cpf: {cpf}");
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(c => c.Cpf == cpf);

        if (customerFromDatabase == null) return NotFound();

        CustomerDto customerToReturn = new CustomerDto()
        {
            Id = customerFromDatabase.Id,
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };
        return Ok(customerToReturn);
    }

    [HttpPost]
    public ActionResult<CustomerDto> CreateCustomer(
        CustomerForCreationDto customerForCreationDto)
    {
        if(!ModelState.IsValid)
        {
            Response.ContentType = "application/problem+json";
            //Cria a fábrica de um objeto de detalhes de problemas da aplicação
            var problemDetailsFactory = HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            //Cria um objeto de detalhes de problema de validação
            var validationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(HttpContext, ModelState);

            //Atribui status code no corpo do response

            validationProblemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            return UnprocessableEntity(validationProblemDetails);
        } 
        
        var customerEntity = new Customer
        {
            Id = Data.Instance.Customers.Max(c => c.Id) + 1,
            Name = customerForCreationDto.Name,
            Cpf = customerForCreationDto.Cpf
        };

        Data.Instance.Customers.Add(customerEntity);

        var customerToReturn = new CustomerDto()
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Cpf = customerEntity.Cpf
        };

        return CreatedAtRoute
        (
            "GetCustomerById",
            new { id = customerToReturn.Id },
            customerToReturn
        );
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCustomer(int id,
        CustomerForUpdateDto customerForUpdateDto)
    {
        if (id != customerForUpdateDto.Id) return BadRequest();
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        customerFromDatabase.Name = customerForUpdateDto.Name;
        customerFromDatabase.Cpf = customerForUpdateDto.Cpf;

        return NoContent();
    }

    [HttpDelete("{id}")]

    public ActionResult DeleteCustomer(int id)
    {
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        Data.Instance.Customers.Remove(customerFromDatabase);

        return NoContent();
    }

    //https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0 = AspNetCore.JsonPatch e AspNetCore.Mvc.NewtonsoftJson - MyJPIF.cs
    //Utiliza o JsonPatch para utilizar o metodo Patch do http
    //E o newtonsoftJson é um requerimento do pacote JsonPatch, pois o padrao é o System.text e precisa se instalar e configurar os pacotes
    //Configuração adicional no Program.cs

    //===========================================================================================================================================================

    //dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson --version 7.0.4
    //dotnet add package Microsoft.AspNetCore.JsonPatch --version 7.0.4

    [HttpPatch("{id}")]
    public ActionResult PartiallyUpdateCustomer(
     [FromBody] JsonPatchDocument<CustomerForPatchDto> patchDocument,
     [FromRoute] int id)
    {
        var customerFromDatabase = Data.Instance.Customers
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        var customerToPatch = new CustomerForPatchDto
        {
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };

        patchDocument.ApplyTo(customerToPatch);

        customerFromDatabase.Name = customerToPatch.Name;
        customerFromDatabase.Cpf = customerToPatch.Cpf;

        return NoContent();
    }

    [HttpGet("with-address")]
    public ActionResult<IEnumerable<CustomerWithAdressesDto>> GetCustomersWithAddresses ()
    {
        var customersFromDatabase = Data.Instance.Customers;

        var customersToReturn = customersFromDatabase
            .Select(customer => new CustomerWithAdressesDto
            {
                Id = customer.Id,
                Name= customer.Name,
                Cpf = customer.Cpf,
                Adresses = customer.Addresses
                    .Select(address =>  new AddressDto
                    {
                        Id = address.Id,
                        City = address.City,
                        Street = address.Street
                    }).ToList()
            }
        );

        return Ok(customersToReturn); //O toList não está aqui pois o IEnumerable não tem a própria lista e sim instruções para fazer a lista, que serão ativadas com o toList
    }

    [HttpPost("with-address")]

    public ActionResult<CustomerForCreationWithAddressDto> CreateCustomerWithAddresses (CustomerForCreationWithAddressDto customerForCreationDto)
    {
        var customerEntity = new Customer
        {
            Id = Data.Instance.Customers.Max(c => c.Id) + 1,
            Name = customerForCreationDto.Name,
            Cpf = customerForCreationDto.Cpf,
            Addresses = new List<Address>()
        };

        foreach (var address in customerForCreationDto.Addresses) 
        {
            var addressEntity = new Address 
            {
                Id = Data.Instance.Customers.SelectMany(c => c.Addresses).Max(a => a.Id) + 1,
                Street = address.Street,
                City = address.City
            };
            Console.WriteLine($"AddressId {addressEntity.Id}");
            customerEntity.Addresses.Add(addressEntity);
        }

        Data.Instance.Customers.Add(customerEntity);

        var customerToReturn = new CustomerDtoWithAddress
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Cpf = customerEntity.Cpf,
        };

        foreach(var address in customerEntity.Addresses) 
        {
            var addressToReturn = new AddressDto 
            {
                Id = address.Id,
                City = address.City,
                Street = address.Street
            };

            customerToReturn.Addresses.Add(addressToReturn);
        }

        return Ok(customerToReturn);
        
    }
}