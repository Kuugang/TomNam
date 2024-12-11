namespace TomNam.Interfaces;
public interface IUnitOfWork{
    Task ExecuteTransactionAsync(Func<Task> operation);
}