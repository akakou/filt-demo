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
	public static void Setup(WebServer server)
	{
		server.RegisterModule(new WebApiModule());
		server.Module<WebApiModule>().RegisterController<MessageController>();

		string path = Application.dataPath + "/Web/";
		server.RegisterModule(new StaticFilesModule(path));
	}

	[WebApiHandler(HttpVerbs.Post, "/message")]
	public async Task<bool> GetMessage(WebServer server, HttpListenerContext context)
	{
		try
		{
			Message message = context.ParseJson<Message>();
			if(message.message == null) {
				throw new Exception();
			}

			Debug.Log(message.message);

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
			
		var url = $@"http://*:9000";

		using (var server = new WebServer(url, RoutingStrategy.Regex))
		{
			MessageController.Setup(server);
			await server.RunAsync();
		}

		Debug.Log("end!");
	}
}
