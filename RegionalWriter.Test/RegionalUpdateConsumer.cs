using RegionalWriter.Model.View;

public class RegionalUpdateConsumer
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public RegionalUpdateConsumer(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async Task Execute(RegionalUpdateDto dto)
    {
        try
        {
            if (dto == null || dto.Id == 0 || string.IsNullOrEmpty(dto.Nome) || dto.Telefone == 0 || dto.DDD == 0 || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Estado) || string.IsNullOrEmpty(dto.Cidade))
            {
                return;
            }

            var contact = await _connectionStringProvider.GetContactAsync(dto.Id);
            if (contact != null)
            {
                contact.Nome = dto.Nome;
                contact.Telefone = dto.Telefone ?? 0;
                contact.DDD = dto.DDD ?? 0;
                contact.Email = dto.Email;
                contact.Estado = dto.Estado;
                contact.Cidade = dto.Cidade;

                await _connectionStringProvider.UpdateContactAsync(contact);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao atualizar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            throw; // Re-throw the exception to ensure it can be caught in tests
        }
    }
}
