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

class SafetySpeaker {
	FiltClient filt;
	UnityChanLipSync  speaker;
	public SafetySpeaker(UnityChanLipSync speaker) {
		string url = Environment.GetEnvironmentVariable("Filt");
		filt = new FiltClient(url);
		this.speaker = speaker;
	}

	public void Speak(String message) {
		Debug.Log(message);
		
		if(isSafe(message)) {
			speaker.Talk(message);
		} else {
			speaker.Talk(message + "、はアニメのタイトルを含んでいます。");
		}
	}
	private bool isSafe(String data) {
		byte[] target = System.Text.Encoding.UTF8.GetBytes(data);
		FiltRequest request = new FiltRequest(target);
		FiltResponse response = filt.Send(request, false);

		Debug.Log("hit:" + response.Hit);
		Debug.Log("success:" + response.Success);

		return (!response.Hit) && response.Success;
	}
}

class Message
{
	[JsonProperty("message")]
	public string message { get; set; }
}

public class MessageController : WebApiController
{
	static SafetySpeaker speaker;

	public static void Setup(WebServer server, UnityChanLipSync lipSync)
	{
		speaker = new SafetySpeaker(lipSync);

		server.RegisterModule(new WebApiModule());
		server.Module<WebApiModule>().RegisterController<MessageController>();

		string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/FiltDemo/Web/";
		path = path.Replace("\\", "/");

		server.RegisterModule(new StaticFilesModule(path));
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

			speaker.Speak(message.message);

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
