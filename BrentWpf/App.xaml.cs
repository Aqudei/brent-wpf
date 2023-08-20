using AutoMapper;
using BrentWpf.Models;
using BrentWpf.Views;
using Prism.DryIoc;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BrentWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Main>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
          
        }
    }
}
