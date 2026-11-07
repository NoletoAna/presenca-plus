using Microsoft.AspNetCore.Identity;

namespace PresencaPlus;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Login { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    public string Perfil { get; set; } = "";

    private static readonly PasswordHasher<Usuario> Hasher = new();

    public void DefinirSenha(string senha) => SenhaHash = Hasher.HashPassword(this, senha);

    public bool VerificarSenha(string senha) =>
        Hasher.VerifyHashedPassword(this, SenhaHash, senha) != PasswordVerificationResult.Failed;
}
