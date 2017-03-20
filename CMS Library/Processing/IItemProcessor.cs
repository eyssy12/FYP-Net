namespace CMS.Library.Processing
{
    public interface IItemProcessor<T>
    {
        void Process(T item);
    }
}