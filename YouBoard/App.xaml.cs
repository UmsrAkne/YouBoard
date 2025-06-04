using System;
using System.IO;
using System.Windows;
using DotNetEnv;
using Prism.Ioc;
using YouBoard.Services;
using YouBoard.Views;

namespace YouBoard
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Env.Load(Path.Combine(userDir, @"youtrackInfo\youtrack.env"));

            #if DEBUG
            containerRegistry.RegisterSingleton<IYouTrackProjectClient, FakeYouTrackProjectClient>();
            #else
            var uri = Env.GetString("YOUTRACK_URI");
            var token = Env.GetString("YOUTRACK_TOKEN");
            var client = new YoutrackProjectClient(uri, token);
            containerRegistry.RegisterInstance<IYouTrackProjectClient>(client);
            #endif
        }
    }
}