using System.ComponentModel.DataAnnotations;

namespace Univali.Api.Models;

public class CustomerForCreationDto 
{
    [Required(ErrorMessage = "You should fill out a Name")]
    [MaxLength(100, ErrorMessage = "The name shouldn't have more than 100 charcters")]
    public string Name {get;set;} = string.Empty;

    [Required(ErrorMessage = "You should fill out a cpf")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "The cpf should have 11 characters")]
    public string Cpf {get;set;} = string.Empty;
}