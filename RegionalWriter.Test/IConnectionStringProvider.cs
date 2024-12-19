using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;

public interface IConnectionStringProvider
{
    string GetConnectionString(string name);
    void Insert(RegionalDto dto);
    Task UpdateContactAsync(Contact contact);
    Task DeleteContactAsync(Contact contact);
    Task<Contact> GetContactAsync(int id);
}
