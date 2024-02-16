namespace Data.Interfaces
{
    public interface IDataProvider<T>
    {
        public T Data { get; }
    }
}