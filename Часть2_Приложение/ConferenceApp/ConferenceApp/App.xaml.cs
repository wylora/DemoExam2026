using System.IO;
using System.Windows;
namespace ConferenceApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // папка Images и подпапки для фото рядом с exe (по методичке)
            string images = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            foreach (string sub in new[] { "Events", "Jury", "Moderators", "Organizers", "Participants" })
                Directory.CreateDirectory(Path.Combine(images, sub));
            base.OnStartup(e);
        }
    }
}
