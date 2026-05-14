using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFGBACKEN.Models;
using TFGBacken.Data.Interfaces;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly IFavoritosRepository _repository;

        public FavoritosController(IFavoritosRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("{productoId}")]
        [Authorize]
        public async Task<IActionResult> ToggleFavorito(int productoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            var favorito = await _repository.GetAsync(usuarioId, productoId);

            if (favorito != null)
            {
                await _repository.DeleteAsync(favorito);
                return Ok(new { favorito = false });
            }

            await _repository.AddAsync(new Favorito
            {
                UsuarioId = usuarioId,
                ProductoId = productoId
            });

            return Ok(new { favorito = true });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFavoritos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            var productos = await _repository.GetProductosFavoritosAsync(usuarioId);

            return Ok(productos);
        }
        [HttpDelete("{productoId}")]
        [Authorize]
        public async Task<IActionResult> EliminarFavorito(int productoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            var favorito = await _repository.GetAsync(usuarioId, productoId);

            if (favorito == null)
                return NotFound("No está en favoritos");

            await _repository.DeleteAsync(favorito);

            return Ok("Eliminado de favoritos");
        }
    }
}