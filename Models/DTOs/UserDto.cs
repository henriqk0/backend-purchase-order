using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo o Email do Usuario, a Senha do Usuario o Papel do Usuario
/// </summary>
public class UserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
}