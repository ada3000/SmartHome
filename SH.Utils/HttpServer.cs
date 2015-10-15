using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SH.Utils
{
	public abstract class HttpServer
	{

		protected int Port;
		private TcpListener _listener;
		private bool _isActive = true;
		private Thread _main = null;

		public event EventHandler<ErrorEventArgs> OnError = (o, e) => { };
		public event EventHandler<HttpProcessorEventArgs> OnGetRequest = (o, ev) => { };
		public event EventHandler<HttpProcessorEventArgs> OnPostRequest = (o, ev) => { };

		public HttpServer(int port)
		{
			this.Port = port;
		}

		public void Start()
		{
			Stop();

			_listener = new TcpListener(IPAddress.Any, Port);
			_listener.Start();
			
			_main = new Thread(Listen);
			_main.IsBackground = true;
			_main.Start();

			_isActive = true;
		}

		public void Stop()
		{
			if(!_isActive)
				return;
			try
			{
				_isActive = false;
				_listener.Stop();
				_main.Abort();
			}
			catch { }
		}

		protected void RaiseError(Exception ex)
		{
			var ev = OnError;
			ev(this, new ErrorEventArgs(ex));
		}

		private void Listen()
		{
			while (_isActive)
			{
				try
				{
					TcpClient s = _listener.AcceptTcpClient();
					HttpProcessor processor = new HttpProcessor(s);

					processor.OnGetRequest += Processor_OnGetRequest;
					processor.OnPostRequest += Processor_OnPostRequest;

					Thread thread = new Thread(new ThreadStart(processor.process));
					thread.IsBackground = true;
					thread.Start();
				}
				catch(Exception ex)
				{
					RaiseError(ex);
				}
			}
		}

		void Processor_OnPostRequest(object sender, HttpProcessorEventArgs e)
		{
			var ev = OnPostRequest;
			ev(this, e);
		}

		void Processor_OnGetRequest(object sender, HttpProcessorEventArgs e)
		{
			var ev = OnGetRequest;
			ev(this, e);
		}
	} 
}
