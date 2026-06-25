using System.ComponentModel.DataAnnotations.Schema;



[Table("Users")]
public class User
{
    public int Id { get; set; }

    public string Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }

    public string Username { get; set; }
    public string PasswordHash { get; set; }
 

    public string Role { get; set; } = "Utilizador";
    public bool Ativo { get; set; } = true;

    // Carta de Condução
    public string? CartaConducaoNumero { get; set; }
    public DateTime? CartaConducaoValidade { get; set; }
    public string? CartaConducaoFicheiro { get; set; }
    public string? CartaConducaoCategoria { get; set; }  

    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? UltimoLogin { get; set; }
}
