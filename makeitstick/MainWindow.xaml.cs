using makeitstick.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace makeitstick
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await using var db = DbFactory.Create();
                // Voert openstaande migrations uit en vult een lege database met testdata (zie DbSeeder).
                await db.Database.MigrateAsync();

                StatusText.Text = $"Verbonden met database '{db.Database.GetDbConnection().Database}'. " +
                    $"{await db.Students.CountAsync()} studenten, {await db.Classes.CountAsync()} klassen, " +
                    $"{await db.QuestionSets.CountAsync()} vragen, {await db.Answers.CountAsync()} antwoorden.";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Kan geen verbinding maken met de database: {ex.Message}";
            }
        }
    }
}
