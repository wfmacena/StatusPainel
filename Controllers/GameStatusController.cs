using Microsoft.AspNetCore.Mvc;
using StatusPainel.Models;
using StatusPainel.Services;

namespace StatusPainel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.Now });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class GameStatusController : ControllerBase
    {
        private readonly GameStatusService _gameStatusService;

        public GameStatusController(GameStatusService gameStatusService)
        {
            _gameStatusService = gameStatusService;
        }

        [HttpGet]
        public IActionResult GetAllStatus()
        {
            var games = _gameStatusService.GetAllGames();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public IActionResult GetGameStatus(int id)
        {
            var game = _gameStatusService.GetAllGames().FirstOrDefault(g => g.Id == id);
            if (game == null)
                return NotFound();
            return Ok(game);
        }

        [HttpPost("{id}/update")]
        public async Task<IActionResult> UpdateGameStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            await _gameStatusService.UpdateGameStatus(id, request.Status, request.LastDetection);
            return Ok(new { message = "Status atualizado com sucesso" });
        }

        [HttpPost("salvar-todos")]
        public async Task<IActionResult> SalvarTodos([FromBody] List<GameStatus> jogosAtualizados)
        {
            try
            {
                await _gameStatusService.UpdateAllGames(jogosAtualizados);
                return Ok(new { message = "Todos os dados foram salvos com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("adicionar")]
        public async Task<IActionResult> AdicionarJogo([FromBody] GameStatus novoJogo)
        {
            try
            {
                var jogo = await _gameStatusService.AddGame(novoJogo);
                return Ok(new { message = "Jogo adicionado com sucesso", jogo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarJogo(int id)
        {
            try
            {
                await _gameStatusService.DeleteGame(id);
                return Ok(new { message = "Jogo deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAllStatus()
        {
            await _gameStatusService.CheckRealGameStatuses();
            return Ok(new { message = "Verificação de status iniciada" });
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string LastDetection { get; set; } = string.Empty;
    }
}