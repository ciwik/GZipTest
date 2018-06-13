namespace GZipLibrary.Processors
{
    public interface IProcessor
    {
        void Run();
        void Cancel();
    }
}
