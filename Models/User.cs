using backend_purchase_order.Models.Enums;


namespace backend_purchase_order.Models;

/// <summary>
/// Classe dos usuários.
/// O papel do usuário é representado por um Papel/Role único
/// O usuário tem uma senha que é criptografada (com BCrypt) no banco e um email, que são
/// utilizados para a autenticação e uso dos endpoints   
/// </summary>
public class User
{
    public int Id { get; private set; }

    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }

    protected User() { }

    public User(string email, string password, UserRole role)
    {
        SetEmail(email);
        SetPassword(password);
        Role = role;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");

        Email = email;
    }

    public void SetPassword(string password)
    {
        // Se a senha for vazia, o sistema não permite a criação do Usuário
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty");

        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Para verificar se a senha de entrada é igual à senha criptografada do objeto Usuário
    public bool ValidatePassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }
}