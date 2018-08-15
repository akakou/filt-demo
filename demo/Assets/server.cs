using UnityEngine;

//using System.Net;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Unosquare.Swan.Attributes;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO;
using Unosquare.Net;


using Filt;

class Message
{
	[JsonProperty("message")]
	public string message { get; set; }
}

public class MessageController : WebApiController
{
	bool isSafe(String data) {
		byte[] target = System.Text.Encoding.ASCII.GetBytes(data + "\n");
		FiltRequest request = new FiltRequest(target);
		string url = Environment.GetEnvironmentVariable("Filt");
		FiltClient filt = new FiltClient(url);
		FiltResponse response = filt.Send(request, false);
		Debug.Log(response.Hit);
		Debug.Log(response.Success);

		return (!response.Hit) && response.Success;
	}
	static UnityChanLipSync  speaker;
	public static void Setup(WebServer server, UnityChanLipSync  _speaker)
	{
		server.RegisterModule(new WebApiModule());
		server.Module<WebApiModule>().RegisterController<MessageController>();

		string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/FiltDemo/Web/";
		path = path.Replace("\\", "/");

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
			
			if(isSafe(message.message)) {
				speaker.Talk(message.message);
			} else {
				speaker.Talk("発言をブロックしました。");
			}

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
	FiltClient filt;

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
