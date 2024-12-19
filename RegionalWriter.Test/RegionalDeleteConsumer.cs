using RegionalWriter.Model.View;

public class RegionalDeleteConsumer
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public RegionalDeleteConsumer(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async Task Execute(RegionalDeleteDto regional)
    {
        try
        {
            if (regional == null || regional.Id == 0)
            {
                return;
            }

            var contact = await _connectionStringProvider.GetContactAsync(regional.Id);
            if (contact != null)
            {
                await _connectionStringProvider.DeleteContactAsync(contact);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao deletar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            throw;
        }
    }
}
