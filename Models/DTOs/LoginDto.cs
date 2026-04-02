namespace backend_purchase_order.Models.DTOs;

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}