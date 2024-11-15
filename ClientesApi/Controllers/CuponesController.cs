using ClientesApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ClientesApi.Interfaces;
using ClientesApi.Models;
using ClientesApi.Services;

[Route("api/[controller]")]
[ApiController]
public class CuponesController : ControllerBase
{
    private readonly HttpClient _httpClient;

    private readonly IClienteService _clienteService;

    public CuponesController(HttpClient httpClient, IClienteService clienteService)
    {
        _httpClient = httpClient;
        _clienteService = clienteService;
    }

    
    [HttpPost("ReclamarCupon")]
    public async Task<IActionResult> EnviarSolicitudACupones([FromBody] ClienteDto clienteDto)
    {
        try
        {
            var respuesta = await _clienteService.SolicitarCupon(clienteDto);
            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    //[HttpPost("ReclamarCupon")]
    //public async Task<IActionResult> ReclamarCupon([FromBody] ClienteDto clienteDto)
    //{
    //    var response = await _httpClient.PostAsJsonAsync("https://localhost:7024/api/SolicitudCupones/SolicitarCupon", clienteDto);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        return Ok(await response.Content.ReadFromJsonAsync<object>());
    //    }

    //    return BadRequest($"Error: {response.ReasonPhrase}");
    //}

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
