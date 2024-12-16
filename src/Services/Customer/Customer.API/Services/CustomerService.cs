using AutoMapper;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Customer;

namespace Customer.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IResult> GetCustomerByUsernameAsync(string username)
        {
            var entity = await _repository.GetCustomerByUserNameAsync(username);
            var result = _mapper.Map<CustomerDto>(entity);

            return Results.Ok(result);
        }

        public async Task<IResult> GetCustomersAsync()
        {
            var entity = await _repository.FindAll().ToListAsync();
            var result = _mapper.Map<List<CustomerDto>>(entity);
            return Results.Ok(result);
        }
    }
}
