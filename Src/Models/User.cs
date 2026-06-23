namespace GestoreDeFrotas.Models
{
    public class User
    {
        public int Id { get; set; }

        // Dados pessoais
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? Telefone { get; set; }

        // Perfil / Permissões
        public string Role { get; set; } = "Utilizador"; // Admin, Gestor, Condutor, etc.
        public bool Ativo { get; set; } = true;

        // Carta de Condução
        public string? CartaConducaoNumero { get; set; }
        public DateTime? CartaConducaoValidade { get; set; }
        public string? CartaConducaoFicheiro { get; set; }

        // Auditoria
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? UltimoLogin { get; set; }
    }
}
