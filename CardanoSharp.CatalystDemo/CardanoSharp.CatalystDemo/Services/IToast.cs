namespace CardanoSharp.CatalystDemo.Services
{
    public interface IToast
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
