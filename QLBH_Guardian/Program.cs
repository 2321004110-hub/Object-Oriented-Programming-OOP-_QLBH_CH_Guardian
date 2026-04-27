using QLBH_Guardian.Forms;

namespace QLBH_Guardian
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmLogin());
        }
    }
}