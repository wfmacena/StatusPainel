using StatusPainel.Models;
using System.Text.Json;

namespace StatusPainel.Services
{
    public class GameStatusService
    {
        private List<GameStatus> _games;
        private readonly ILogger<GameStatusService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _dataFilePath;
        private readonly object _fileLock = new object();

        public GameStatusService(ILogger<GameStatusService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            // Define o caminho do arquivo JSON
            var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            _dataFilePath = Path.Combine(dataDirectory, "games.json");

            // Carrega ou inicializa os dados
            LoadGames();
        }

        private void LoadGames()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    var json = File.ReadAllText(_dataFilePath);
                    _games = JsonSerializer.Deserialize<List<GameStatus>>(json);
                    _logger.LogInformation($"Dados carregados do arquivo: {_games.Count} jogos");
                }
                else
                {
                    // Se não existir, cria com dados iniciais
                    InitializeDefaultGames();
                    SaveGames();
                    _logger.LogInformation("Arquivo de dados criado com jogos padrão");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados do arquivo");
                InitializeDefaultGames();
            }
        }

        private void InitializeDefaultGames()
        {
            _games = new List<GameStatus>
            {
                // Linha 1
                new GameStatus { Id = 1, Name = "Invasores ARC", DisplayName = "Invasores ARC",
                    ReleaseDate = "1º de maio de 2025", Status = "Não detectado", LastDetection = "Há 18 dias" },

                new GameStatus { Id = 2, Name = "Arau Reforger", DisplayName = "Arau Reforger",
                    ReleaseDate = "31 de janeiro de 2025", Status = "Atualizado", LastDetection = "Nunca" },

                new GameStatus { Id = 3, Name = "Apex Legends", DisplayName = "Apex Legends",
                    ReleaseDate = "11 de setembro de 2021", Status = "Testando", LastDetection = "Há 20 meses" },

                // Linha 2
                new GameStatus { Id = 4, Name = "Invasores ARG", DisplayName = "Invasores ARG",
                    ReleaseDate = "1º de maio de 2025", Status = "Atualizado", LastDetection = "Há 21 dias" },

                new GameStatus { Id = 5, Name = "Amma Reforger", DisplayName = "Amma Reforger",
                    ReleaseDate = "31 de janeiro de 2025", Status = "Atualizado", LastDetection = "Nunca" },

                new GameStatus { Id = 6, Name = "Black Ops 6", DisplayName = "Black Ops 6",
                    ReleaseDate = "30 de agosto de 2024", Status = "Atualizado", LastDetection = "Nunca" },

                // Linha 3
                new GameStatus { Id = 7, Name = "Black Ops 7", DisplayName = "Black Ops 7",
                    ReleaseDate = "2 de setembro de 2025", Status = "Atualizado", LastDetection = "Nunca" },

                new GameStatus { Id = 8, Name = "CS2", DisplayName = "CS: Counter-Strike 2",
                    ReleaseDate = "1º de setembro de 2023", Status = "Atualizado", LastDetection = "Nunca" },

                new GameStatus { Id = 9, Name = "DayZ", DisplayName = "MYZ: DayZ",
                    ReleaseDate = "29 de agosto de 2022", Status = "Atualizado", LastDetection = "Há 25 meses" },

                // Linha 4
                new GameStatus { Id = 10, Name = "Escape from Tarkov", DisplayName = "TROPHY: Escape from Tarkov",
                    ReleaseDate = "4 de dezembro de 2022", Status = "Atualizado", LastDetection = "Há 25 meses" },

                new GameStatus { Id = 11, Name = "Marvel Rivals", DisplayName = "Rivals da Marvel",
                    ReleaseDate = "26 de janeiro de 2025", Status = "Testando", LastDetection = "Nunca" },

                new GameStatus { Id = 12, Name = "Modern Warfare III", DisplayName = "Modern Warfare III",
                    ReleaseDate = "12 de outubro de 2023", Status = "Atualizado", LastDetection = "Há 29 meses" }
            };
        }

        private void SaveGames()
        {
            try
            {
                lock (_fileLock)
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var json = JsonSerializer.Serialize(_games, options);
                    File.WriteAllText(_dataFilePath, json);
                    _logger.LogInformation("Dados salvos no arquivo");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar dados no arquivo");
            }
        }

        public List<GameStatus> GetAllGames()
        {
            return _games;
        }

        public async Task UpdateGameStatus(int id, string newStatus, string lastDetection)
        {
            var game = _games.FirstOrDefault(g => g.Id == id);
            if (game != null)
            {
                game.Status = newStatus;
                game.LastDetection = lastDetection;
                game.LastCheck = DateTime.Now;

                SaveGames(); // Salva automaticamente após atualizar
                _logger.LogInformation($"Status do jogo {game.Name} atualizado para {newStatus}");
            }
            await Task.CompletedTask;
        }

        // Método para atualizar todos os dados de uma vez
        public async Task UpdateAllGames(List<GameStatus> updatedGames)
        {
            _games = updatedGames;
            SaveGames();
            _logger.LogInformation("Todos os jogos foram atualizados");
            await Task.CompletedTask;
        }

        // Método para adicionar um novo jogo
        public async Task<GameStatus> AddGame(GameStatus newGame)
        {
            newGame.Id = _games.Max(g => g.Id) + 1;
            _games.Add(newGame);
            SaveGames();
            _logger.LogInformation($"Novo jogo adicionado: {newGame.DisplayName}");
            return newGame;
        }

        // Método para deletar um jogo
        public async Task DeleteGame(int id)
        {
            var game = _games.FirstOrDefault(g => g.Id == id);
            if (game != null)
            {
                _games.Remove(game);
                SaveGames();
                _logger.LogInformation($"Jogo deletado: {game.DisplayName}");
            }
            await Task.CompletedTask;
        }

        public async Task CheckRealGameStatuses()
        {
            try
            {
                // Aqui você implementaria verificações reais
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar status dos jogos");
            }
        }
    }
}