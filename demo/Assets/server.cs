using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Text;


public class server : MonoBehaviour {
	HttpListener listener;

	// Use this for initialization
	void Start () {	
		Debug.Log("start !");
			
		System.Threading.Tasks.Task.Run( () => {			
			InitServer();

			while (true)
			{
				HttpListenerContext context = listener.GetContext();
				HttpListenerResponse res = context.Response;
				res.StatusCode = 200;

				byte[] content = Encoding.UTF8.GetBytes("HELLO");
				res.OutputStream.Write(content, 0, content.Length);

				res.Close();
				
			}
		});	

		Debug.Log("end!");		
	}
	
	// Update is called once per frame
	void Update () {
	}

	void InitServer() {
		listener = new HttpListener();
		listener.Prefixes.Add("http://localhost:8080/");
		listener.Start();
	}
}
