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


class Message
{
	[JsonProperty("message")]
	public int Id { get; set; }
}

public class MessageController : WebApiController
{
        public static void Setup(WebServer server)
        {
            server.RegisterModule(new WebApiModule());
            server.Module<WebApiModule>().RegisterController<MessageController>();
        }


	    [WebApiHandler(HttpVerbs.Get, "/test")]
        public async Task<bool> GetMessage(WebServer server, HttpListenerContext context,
            int id)
        {
                context.Response.StatusCode = (int) System.Net.HttpStatusCode.OK;
                return await context.StringResponseAsync("Hello world");
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
