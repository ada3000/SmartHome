using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;


namespace Profile.Service
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer
	{
		private const string F_ServiceName="ServiceName";

		public ProjectInstaller()
		{
			InitializeComponent();			
		}

		private void ProjectInstaller_BeforeInstall(object sender, InstallEventArgs e)
		{
			//Инициализания названия сервиса
			InstallerSrv.ServiceName = this.Context.Parameters[F_ServiceName];
		}

		private void ProjectInstaller_BeforeUninstall(object sender, InstallEventArgs e)
		{
			//Инициализания названия сервиса
			InstallerSrv.ServiceName = this.Context.Parameters[F_ServiceName];
		}

		private void InstallerProcess_AfterInstall(object sender, InstallEventArgs e)
		{

		}
	}
}
