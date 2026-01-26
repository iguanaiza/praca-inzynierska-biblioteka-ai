using System;
using System.Threading.Tasks;

public class GlobalUiService
{
    public event Action<string, string>? OnShowToast;

    public event Func<string, string, Task<bool>>? OnConfirmRequested;

    public void ShowToast(string message, string type = "success")
    {
        OnShowToast?.Invoke(message, type);
    }

    public async Task<bool> ConfirmAsync(string title, string message)
    {
        if (OnConfirmRequested != null)
        {
            return await OnConfirmRequested.Invoke(title, message);
        }
        return false;
    }
}