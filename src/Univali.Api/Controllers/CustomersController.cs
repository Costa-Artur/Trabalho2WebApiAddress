using Microsoft.AspNetCore.Mvc;
using Univali.Api.Entities;

namespace Univali.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Customer>> GetCustomers()
    {
        var result = Data.Instance.Customers;
        return Ok(result);
    }

    [HttpGet("{id}", Name = "GetCustomerById")]
    public ActionResult<Customer> GetCustomerById (int id) 
    {
        Console.WriteLine($"id: {id}");
        var result = Data.Instance.Customers.FirstOrDefault(c => c.Id == id);

        if(result != null) return Ok(result);

        return NotFound();
    }

    [HttpGet("cpf/{cpf}")]
    public ActionResult<Customer> GetCustomerByCpf (string cpf) 
    {
        Console.WriteLine($"cpf: {cpf}");
        var result = Data.Instance.Customers.FirstOrDefault(c => c.Cpf == cpf);

        if(result != null) return Ok(result);

        return NotFound();
    }
    [HttpPost]
    public ActionResult<Customer> CreateCustomer (Customer customer) 
    {
        var newCustomer = new Customer 
        {
            Id = Data.Instance.Customers.Max(c => c.Id)+1,
            Name = customer.Name,
            Cpf = customer.Cpf
        };

        Data.Instance.Customers.Add(newCustomer);
        return CreatedAtRoute
        (
            "GetCustomerById",
            new {id = newCustomer.Id },
            newCustomer
        );
    }
}