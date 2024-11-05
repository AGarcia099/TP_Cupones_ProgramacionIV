using ClientesApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CuponesController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public CuponesController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("ReclamarCupon")]
    public async Task<IActionResult> ReclamarCupon([FromBody] ClienteDto clienteDto)
    {
        var response = await _httpClient.PostAsJsonAsync("https://localhost:7024/api/SolicitudCupones/SolicitarCupon", clienteDto);

        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadFromJsonAsync<object>());
        }

        return BadRequest($"Error: {response.ReasonPhrase}");
    }

    [HttpPost("UsarCupon")]
    public async Task<IActionResult> UsarCupon([FromBody] string nroCupon)
    {
        var response = await _httpClient.PostAsJsonAsync("https://localhost:7024/api/SolicitudCupones/QuemadoCupon", new { NroCupon = nroCupon });

        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadFromJsonAsync<object>());
        }

        return BadRequest($"Error: {response.ReasonPhrase}");
    }
}
