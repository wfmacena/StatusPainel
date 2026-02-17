namespace StatusPainel.Models
{
    public class GameStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string LastDetection { get; set; }
        public string ReleaseDate { get; set; }
        public string Status { get; set; } // "Atualizado", "Atualizando", "Testando", "Não detectado"
        public DateTime? LastCheck { get; set; }
        public bool IsOnline { get; set; }
    }
}