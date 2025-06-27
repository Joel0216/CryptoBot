using CryptoBot.Domain.Entities;
using CryptoBot.DTOs;
using CryptoBot.Services.Features.Crypto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBot.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize] // 👈 Token obligatorio en TODOS los endpoints
    public class CryptoController : ControllerBase
    {
        private readonly CryptoService _service;

        public CryptoController(CryptoService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id:int:min(1)}")] // solo números positivos
        public IActionResult GetById(int id)
        {
            var palabra = _service.GetById(id);
            if (palabra == null)
                return NotFound(new { mensaje = $"No se encontró una palabra con ID {id}" });

            return Ok(palabra);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PalabraRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nueva = _service.Add(new Palabra
            {
                Texto = request.Texto,
                Desplazamiento = request.Desplazamiento
            });

            return Ok(new
            {
                nueva.Id,
                nueva.Texto,
                nueva.Desplazamiento,
                nueva.Resultado
            });
        }

        // POST: desencriptar texto sin guardar en DB
        [HttpPost("desencriptar")]
        public IActionResult Desencriptar([FromBody] PalabraRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = _service.Desencriptar(request.Texto, request.Desplazamiento);
            return Ok(new
            {
                textoOriginal = request.Texto,
                desplazamiento = request.Desplazamiento,
                resultado
            });
        }

        [HttpPut("{id:int:min(1)}")] // solo ID numérico positivo
        public IActionResult Put(int id, [FromBody] PalabraRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var actualizado = _service.Update(id, new Palabra
            {
                Id = id,
                Texto = request.Texto,
                Desplazamiento = request.Desplazamiento
            });

            if (!actualizado)
                return NotFound(new { mensaje = $"No se encontró el ID {id} para editar." });

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")] // solo ID positivo
        public IActionResult Delete(int id)
        {
            var eliminado = _service.Delete(id);
            if (!eliminado)
                return NotFound(new { mensaje = $"No se encontró el ID {id} para eliminar." });

            return NoContent();
        }
    }
}