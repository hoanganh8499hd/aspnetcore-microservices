namespace Customer.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IResult> GetCustomersAsync();
        Task<IResult> GetCustomerByUsernameAsync(string username);
    }
}
