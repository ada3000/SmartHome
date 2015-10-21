using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SH.Utils
{
	public class HttpProcessor
	{
		public TcpClient Socket;
		
		private Stream _inputStream;
		public StreamWriter OutputStream;

		public String HttpMethod;
		public String HttpUrl;
		public String HttpProtocolVersionstring;
		public Hashtable HttpHeaders = new Hashtable();

		private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB
		private const int BUF_SIZE = 4096;

		public event EventHandler<HttpProcessorEventArgs> OnGetRequest = (o, ev) => { };
		public event EventHandler<HttpProcessorEventArgs> OnPostRequest = (o, ev) => { };
		public event EventHandler<ErrorEventArgs> OnError = (o, ev) => { };

		private bool _hasWriteResponce = false;

		public HttpProcessor(TcpClient s)
		{
			this.Socket = s;
		}

		private string streamReadLine(Stream inputStream)
		{
			int next_char;
			string data = "";
			while (true)
			{
				next_char = inputStream.ReadByte();
				if (next_char == '\n') { break; }
				if (next_char == '\r') { continue; }
				if (next_char == -1) { Thread.Sleep(1); continue; };
				data += Convert.ToChar(next_char);
			}
			return data;
		}
		public void process()
		{
			// we can't use a StreamReader for input, because it buffers up extra data on us inside it's
			// "processed" view of the world, and we want the data raw after the headers
			using (_inputStream = new BufferedStream(Socket.GetStream()))
			{
				using (OutputStream = new StreamWriter(new BufferedStream(Socket.GetStream())))
				{
					try
					{
						ParseRequest();
						ReadHeaders();

						if (HttpMethod.Equals("GET"))
						{
							OnGetRequest(this, new HttpProcessorEventArgs(this));
						}
						else if (HttpMethod.Equals("POST"))
						{
							HandlePostRequest();
						}

						if (!_hasWriteResponce)
							WriteSuccess();
					}
					catch (Exception e)
					{
						var badEx = new BadRequestException(HttpUrl, HttpMethod, e);
						OnError(this, new ErrorEventArgs(badEx));

						WriteFailure();
					}

					OutputStream.Flush();
					Socket.Close();
				}
			}
		}

		public void ParseRequest()
		{
			string request = streamReadLine(_inputStream);
			
			string[] tokens = request.Split(' ');
			if (tokens.Length != 3)
			{
				throw new Exception("invalid http request line");
			}

			HttpMethod = tokens[0].ToUpper();
			HttpUrl = tokens[1];
			HttpProtocolVersionstring = tokens[2];

			Console.WriteLine("starting: " + request);
		}

		public void ReadHeaders()
		{
			Console.WriteLine("readHeaders()");
			string line;
			while ((line = streamReadLine(_inputStream)) != null)
			{
				if (line.Equals(""))
				{
					Console.WriteLine("got headers");
					return;
				}

				int separator = line.IndexOf(':');
				if (separator == -1)
				{
					throw new Exception("invalid http header line: " + line);
				}
				String name = line.Substring(0, separator);
				int pos = separator + 1;
				while ((pos < line.Length) && (line[pos] == ' '))
				{
					pos++; // strip any spaces
				}

				string value = line.Substring(pos, line.Length - pos);
				Console.WriteLine("header: {0}:{1}", name, value);
				HttpHeaders[name] = value;
			}
		}

		public void HandlePostRequest()
		{
			// this post data processing just reads everything into a memory stream.
			// this is fine for smallish things, but for large stuff we should really
			// hand an input stream to the request processor. However, the input stream 
			// we hand him needs to let him see the "end of the stream" at this content 
			// length, because otherwise he won't know when he's seen it all! 
				Console.WriteLine("get post data start");
				int contentLen = 0;

				MemoryStream ms = new MemoryStream();
				if (this.HttpHeaders.ContainsKey("Content-Length"))
				{
					contentLen = Convert.ToInt32(this.HttpHeaders["Content-Length"]);
					if (contentLen > MAX_POST_SIZE)
					{
						throw new Exception(
							String.Format("POST Content-Length({0}) too big for this simple server",
							  contentLen));
					}
					byte[] buf = new byte[BUF_SIZE];
					int to_read = contentLen;
					while (to_read > 0)
					{
						Console.WriteLine("starting Read, to_read={0}", to_read);

						int numread = this._inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
						Console.WriteLine("read finished, numread={0}", numread);
						if (numread == 0)
						{
							if (to_read == 0)
							{
								break;
							}
							else
							{
								throw new Exception("client disconnected during post");
							}
						}
						to_read -= numread;
						ms.Write(buf, 0, numread);
					}
					ms.Seek(0, SeekOrigin.Begin);
				}
				Console.WriteLine("get post data end");

				using (ms)
				using (StreamReader sr = new StreamReader(ms))
					OnPostRequest(this, new HttpProcessorEventArgs(this, sr));
		}

		public void WriteSuccess(string content = null, string contentType = "text/html")
		{
			_hasWriteResponce = true;
			// this is the successful HTTP response line
			OutputStream.WriteLine("HTTP/1.0 200 OK");
			// these are the HTTP headers...          
			OutputStream.WriteLine("Content-Type: " + contentType);
			OutputStream.WriteLine("Connection: close");
			// ..add your own headers here if you like

			OutputStream.WriteLine(""); // this terminates the HTTP headers.. everything after this is HTTP body..

			if(!string.IsNullOrEmpty(content))
				OutputStream.WriteLine(content);
		}

		public void WriteFailure(string content=null)
		{
			_hasWriteResponce = true;
			// this is an http 404 failure response
			OutputStream.WriteLine("HTTP/1.0 404 File not found");
			// these are the HTTP headers
			OutputStream.WriteLine("Connection: close");
			// ..add your own headers here

			OutputStream.WriteLine(""); // this terminates the HTTP headers.

			if (!string.IsNullOrEmpty(content))
				OutputStream.WriteLine(content);
		}
	}
}
