
using backend_purchase_order.Models;
using Microsoft.EntityFrameworkCore;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Models.Enums;
namespace backend_purchase_order.Services;

public class OrderActionHistoryService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Salva o objeto OrderActionHistory no banco no momento em que a ação for tomada, já com os relacionamentos
    /// entre o usuário responsável pela ação e o pedido 
    /// </summary>
    /// <param name="orderId">O id do Pedido</param>
    /// <param name="actionMakerId">O id do Fazedor da Ação</param>
    /// <param name="actionType">O tipo de ação</param>
    /// <returns></returns>
    public async Task<OrderActionHistory> RecordActionInHistoryNow(int orderId, int actionMakerId, ActionType actionType)
    {
        DateTime nowAction = DateTime.Now;

        DateOnly currentDate = DateOnly.FromDateTime(nowAction);
        TimeOnly currentTime = TimeOnly.FromDateTime(nowAction);

        var orderHistory = new OrderActionHistory(orderId, actionMakerId, currentDate, currentTime, actionType);
        _context.OrderActionHistory.Add(orderHistory);

        await _context.SaveChangesAsync();

        return orderHistory;
    }

    /// <summary>
    /// Realiza as regras de negócio relativas ao aprovador (só pode aprovar ou requisitar review)
    /// </summary>
    /// <param name="approverActionDto">Dto contendo o id do Pedido, o id do Usuário, o papel do Usuário e o tipo da ação</param>
    /// <returns>O objeto criado que representa a ação tomada no pedido pelo aprovador</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<OrderActionHistory> RecordApproverAction(int userId, RecordApproverActionDto approverActionDto)
    {
        var lastActionAtOrder = await _context.OrderActionHistory
            .Include(orderActHist => orderActHist.ResponsibleUser)
            .Include(orderActHist => orderActHist.Order)
            .Where(orderActHist => orderActHist.OrderId == approverActionDto.OrderId)
            .OrderByDescending(orderActHist => orderActHist.Id)
            .FirstOrDefaultAsync();

        // Se o pedido foi concluido/cancelado ou se aguarda revisão ou se não foi criado, o aprovador não pode tomar uma ação
        if (lastActionAtOrder == null
            || lastActionAtOrder.ActionType == ActionType.Concluison
            || lastActionAtOrder.ActionType == ActionType.RequestForReview
            || lastActionAtOrder.ActionType == ActionType.Cancel)
            throw new ArgumentException($"Invalid order to take an action");

        // Se o inteiro de ação recebido no DTO 
        // não for nem de aprovação e nem de solicitar revisão e nem de cancelamento, também levanta um erro
        if (
                approverActionDto.ActionType != ActionType.Approbation
                && approverActionDto.ActionType != ActionType.RequestForReview
                && approverActionDto.ActionType != ActionType.Cancel
            )
            throw new ArgumentException("Invalid action");

        var userRoleInLastOrderAction = lastActionAtOrder.ResponsibleUser.Role;
        var userRoleFromDTO = approverActionDto.ApproverRole;
        var price = lastActionAtOrder.Order.TotalValue;

        // Nenhum usuário de setor algum pode tomar uma ação se o setor anterior não o tomou.
        // Usuários do mesmo setor também não podem alterar uma definição já existente pelo mesmo setor.
        // Esta lógica só funciona caso APENAS colaboradores façam pedidos (como modelado no código)
        if (userRoleFromDTO == userRoleInLastOrderAction || userRoleFromDTO != userRoleInLastOrderAction + 1)
            throw new ArgumentException("Action cannot be taken before the lower hierarchy");

        var approverOrderHistoryAction = await RecordActionInHistoryNow(approverActionDto.OrderId, userId, approverActionDto.ActionType);

        // Se o pedido for de aprovação, então podemos concluí-lo dependendo das aprovações anteriores:
        if (approverActionDto.ActionType == ActionType.Approbation)
            // Se o preco for inferior a 100, também conclui o pedido
            if (price <= 100)
                await RecordActionInHistoryNow(approverActionDto.OrderId, userId, ActionType.Concluison);
            // Caso caso esteja entre 100.00000...1 e 1000, e o setor de Suprimentos já aprovou
            else if (price <= 1000 && userRoleInLastOrderAction == UserRole.SupplyDepartment)
                await RecordActionInHistoryNow(approverActionDto.OrderId, userId, ActionType.Concluison);
            // Caso contrário e o diretor é o último a aprovar, então também podemos concluir o pedido. 
            else if (userRoleInLastOrderAction == UserRole.Manager)
                await RecordActionInHistoryNow(approverActionDto.OrderId, userId, ActionType.Concluison);

        return approverOrderHistoryAction;
    }

    /// <summary>
    /// Realiza as regras de negócio relativas ao colaborador (só pode criar ou reenviar pedidos)
    /// </summary>
    /// <param name="userId">Id do Usuário</param>
    /// <param name="collaboratorActionDto">Dto contendo o id do Pedido e o tipo da ação</param>
    /// <returns>O objeto criado que representa a ação tomada no pedido pelo aprovador</returns>
    /// <exception cref="ArgumentException">Verifica o tipo de ação do parêmetro e se é compatível com ações passadas
    /// neste mesmo pedido</exception>
    public async Task<OrderActionHistory> RecordCollaboratorAction(int userId, RecordActionDto collaboratorActionDto)
    {
        var lastActionAtOrder = await _context.OrderActionHistory
            .Where(orderActHist => orderActHist.OrderId == collaboratorActionDto.OrderId)
            .OrderByDescending(orderActHist => orderActHist.Id)
            .FirstOrDefaultAsync();

        // Se o já há uma última ação no pedido, e não foi de solicitação de revisão, o colaborador não pode tomar uma ação
        if (lastActionAtOrder != null && lastActionAtOrder.ActionType != ActionType.RequestForReview)
            throw new ArgumentException("You can only take action on the request made as long as no revisions have been requested");

        // Se ha uma acao no pedido, e a nova acao do colaborador nao eh de reenvio, ou se nao ha uma acao neste pedido 
        // e a acao do colaborador nao eh de criacao, tambem levanta um erro 
        if (
            (lastActionAtOrder != null && collaboratorActionDto.ActionType != ActionType.Resend)
                ||
            (lastActionAtOrder == null && collaboratorActionDto.ActionType != ActionType.Creation)
        )
            throw new ArgumentException("Invalid action");

        var approverOrderHistoryAction = await RecordActionInHistoryNow(
            collaboratorActionDto.OrderId, userId, collaboratorActionDto.ActionType
        );

        return approverOrderHistoryAction;
    }

    /// <summary>
    /// Registra a ação de acordo com o tipo do usuário e tipo de ação no DTO
    /// </summary>
    /// <param name="userId">Id do usuario</param>
    /// <param name="recordActionDto">DTO agnostico ao tipo do usuario, contendo id do pedido e acao
    /// </param>
    /// <returns>Objeto OrderActionHistory</returns>
    /// <exception cref="ArgumentException">Usuario ou Pedido invalidos</exception>
    public async Task<OrderActionHistory> RecordAgnosticAction(int userId, RecordActionDto recordActionDto)
    {
        var user = await _context.User.FindAsync(userId);
        var order = await _context.Order.FindAsync(recordActionDto.OrderId);

        // Levanta um erro se o usuário ou o pedido não existam após as consultas acima
        if (user == null || order == null)
            throw new ArgumentException("User or Order not found");

        // Se o inteiro de ação recebido no DTO não corresponder a uma ação enumerada, também levanta um erro
        if (!Enum.IsDefined(typeof(ActionType), recordActionDto.ActionType))
            throw new ArgumentException("This action does not exist");

        // Se um papel definido e for um colaborador, chama método para regras de negócio de registro de ação do colaborador, 
        // ou chama o método para regras de negório de registro de ação dos outros pepéis (aprovadores), se ele existir. 
        // Caso contrário levanta um Erro pelo papel do Usuário fornecido ter sido inválido 
        OrderActionHistory orderActionHistory;
        if (Enum.IsDefined(typeof(UserRole), user.Role))
        {
            if (user.Role == UserRole.Collaborator)
            {
                orderActionHistory = await RecordCollaboratorAction(userId, recordActionDto);
            }
            else
            {
                var approverActionDto = new RecordApproverActionDto(
                    recordActionDto.OrderId,
                    recordActionDto.ActionType,
                    user.Role
                );
                orderActionHistory = await RecordApproverAction(userId, approverActionDto);
            }
        }
        else
        {
            throw new ArgumentException("Invalid role indentifier from user");
        }

        return orderActionHistory;
    }

    public async Task<OrderActionHistory?> GetOrderHistoryByIdAsync(int id)
    {
        return await _context.OrderActionHistory.FindAsync(id);
    }
}
