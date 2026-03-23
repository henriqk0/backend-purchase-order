namespace backend_purchase_order.Models.Enums;

/// <summary>
/// Constantes imutáveis para indicar o tipo das ações 
/// (criação, aprovação, solicitação de revisão, reenvio e conclusão)
/// </summary>
public enum ActionType
{
    Creation = 1,
    Approbation = 2,
    RequestForReview = 3,
    Resend = 4,
    Concluison = 5
}