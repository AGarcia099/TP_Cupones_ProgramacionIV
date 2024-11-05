using CuponesApiTp.Interfaces;

namespace CuponesApiTp.Services
{
    public class CuponesServices : ICuponesServices
    {
        public async Task<string> GenerarNroCupon()
        {
            Random random = new Random();

            int parte1 = random.Next(100, 1000);
            int parte2 = random.Next(100, 1000);
            int parte3 = random.Next(100, 1000);

            string nroCupon = $"{parte1}-{parte2}-{parte3}";

            return nroCupon;
        }
    }
}
