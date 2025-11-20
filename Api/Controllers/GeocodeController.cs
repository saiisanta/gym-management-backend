using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeocodeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IMemoryCache _cache;

        public GeocodeController(IHttpClientFactory httpFactory, IMemoryCache cache)
        {
            _httpFactory = httpFactory;
            _cache = cache;
        }

        // Público: el slider necesita esto sin login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { error = "Query 'q' requerida" });

            var cacheKey = $"geocode:{q.Trim().ToLowerInvariant()}";

            if (_cache.TryGetValue<string>(cacheKey, out var cachedJson))
            {
                return Content(cachedJson, "application/json");
            }

            try
            {
                var client = _httpFactory.CreateClient("Nominatim");
                var url = $"search?q={Uri.EscapeDataString(q)}&format=json&limit=1";

                using var resp = await client.GetAsync(url);

                if (!resp.IsSuccessStatusCode)
                {
                    var text = await resp.Content.ReadAsStringAsync();
                    return StatusCode((int)resp.StatusCode, new { error = "Error de Nominatim", details = text });
                }

                var body = await resp.Content.ReadAsStringAsync();

                // Cache corto para reducir peticiones idénticas
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(cacheKey, body, cacheOptions);

                return Content(body, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { error = "Problema de red con Nominatim", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener coordenadas", details = ex.Message });
            }
        }
    }
}
