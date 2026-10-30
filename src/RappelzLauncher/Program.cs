namespace RappelzLauncher;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            Application.Run(new MainForm());
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Launcher Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
