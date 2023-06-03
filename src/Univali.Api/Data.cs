using Univali.Api.Entities;

namespace Univali.Api
{
    //Classe sem parametro
    public class Data 
    {

        private static Data? _instance;
        public List<Customer> Customers {get; set;}
        private Data() 
        {
            Customers = new List<Customer>()
            {
                new Customer
                {
                    Id = 1,
                    Name = "Linus Torvalds",
                    Cpf = "73473943096",
                    Addresses = new List<Address>()
                    {
                        new Address
                        {
                            Id = 1,
                            Street = "Verão do Cometa",
                            City = "Elvira"
                        },
                        new Address
                        {
                            Id = 2,
                            Street = "Borboletas Psicodélicas",
                            City = "Perobia"
                        }
                    }
                },
                new Customer
                {
                    Id = 2,
                    Name = "Elon Musk",
                    Cpf = "95395994076",
                    Addresses = new List<Address>()
                    {
                        new Address 
                        {
                            Id = 3,
                            Street = "Canção Excêntrica",
                            City = "Salandra"
                        }
                    }
                }
            };
        }

        public static Data Instance 
        {
            get 
            {
                return _instance ??= new Data();
            }
        }
    }
}