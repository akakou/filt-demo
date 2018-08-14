using UnityEngine;

//using System.Net;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Unosquare.Swan.Attributes;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO;
using Unosquare.Net;
using System.IO;



class Message
{
	[JsonProperty("message")]
	public string message { get; set; }
}

public class MessageController : WebApiController
{
	static UnityChanLipSync  speaker;
	public static void Setup(WebServer server, UnityChanLipSync  _speaker)
	{
		server.RegisterModule(new WebApiModule());
		server.Module<WebApiModule>().RegisterController<MessageController>();

		string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/FiltDemo/Web/";
		path = path.Replace("\\", "/");
		//string path = Application.dataPath + "/../../Web/";
		Debug.Log(path);
		server.RegisterModule(new StaticFilesModule(path));

		speaker = _speaker;
	}

	[WebApiHandler(HttpVerbs.Post, "/message")]
	public async Task<bool> GetMessage(WebServer server, HttpListenerContext context)
	{
		try
		{
			Message message = context.ParseJson<Message>();
			if(message.message == null || message.message.Length == 0) {
				throw new Exception();
			}

			Debug.Log(message.message);
			speaker.Talk(message.message);

			context.Response.StatusCode = (int) System.Net.HttpStatusCode.Created;
			return await context.StringResponseAsync(string.Empty);
		}
		catch (Exception ex)
		{
			return await context.JsonExceptionResponseAsync(ex);
		}
	}
}

public class server : MonoBehaviour {
	System.Net.HttpListener listener;

	// Use this for initialization
	async Task Start () {	
		Debug.Log("start !");
		var lipSync = GetComponent<UnityChanLipSync>();
			
		var url = $@"http://*:9000";

		using (var server = new WebServer(url, RoutingStrategy.Regex))
		{
			MessageController.Setup(server, lipSync);
			await server.RunAsync();
		}

		Debug.Log("end!");
	}
}
