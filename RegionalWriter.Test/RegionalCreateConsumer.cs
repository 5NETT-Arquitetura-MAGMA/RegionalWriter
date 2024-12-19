using RegionalWriter.Model.View;

namespace RegionalWriter.Test;

public class RegionalCreateConsumer(IConnectionStringProvider connectionStringProvider)
{
    public async Task Execute(RegionalDto dto)
    {
        if (string.IsNullOrEmpty(dto.Nome) || dto.Telefone == 0 || dto.DDD == 0 || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Estado) || string.IsNullOrEmpty(dto.Cidade))
        {
            return;
        }

        connectionStringProvider.Insert(dto);
    }
}