namespace CuponesApiTp.Interfaces
{
    public interface ISendEmailService
    {
        Task EnviarEmailCliente(string emailCliente, string nroCupon);
    }
}
