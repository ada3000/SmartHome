namespace Profile.Service
{
	partial class ProjectInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.InstallerProcess = new System.ServiceProcess.ServiceProcessInstaller();
			this.InstallerSrv = new System.ServiceProcess.ServiceInstaller();
			// 
			// InstallerProcess
			// 
			this.InstallerProcess.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.InstallerProcess.Password = null;
			this.InstallerProcess.Username = null;
			this.InstallerProcess.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.InstallerProcess_AfterInstall);
			// 
			// InstallerSrv
			// 
			this.InstallerSrv.DisplayName = "Smart Monitoring Service";
			this.InstallerSrv.ServiceName = "SH.Service";
			this.InstallerSrv.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.InstallerProcess,
            this.InstallerSrv});
			this.BeforeInstall += new System.Configuration.Install.InstallEventHandler(this.ProjectInstaller_BeforeInstall);
			this.BeforeUninstall += new System.Configuration.Install.InstallEventHandler(this.ProjectInstaller_BeforeUninstall);

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller InstallerProcess;
		private System.ServiceProcess.ServiceInstaller InstallerSrv;
	}
}