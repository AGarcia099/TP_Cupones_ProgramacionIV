using CuponesApiTp.Interfaces;

namespace CuponesApiTp.Services
{
    public class CuponesServices : ICuponesServices
    {
        public async Task<string> GenerarNroCupon()
        {
            var NroCupon = "123-456-789";
            return NroCupon;
        }
    }
}
