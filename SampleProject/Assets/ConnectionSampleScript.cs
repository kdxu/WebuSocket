﻿using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using WebuSocketCore;


/**
	webuSocket connection sample.
*/
public class ConnectionSampleScript : MonoBehaviour {
	
	WebuSocket webSocket;
	
	bool opened = false;

	void Start () {

		webSocket = new WebuSocket(
			// url.
			"wss://echo.websocket.org:443/",

			// buffer size.
			1024,

			// handler for connection established to server.
			() => {
				opened = true;
				Debug.Log("connected to websocket echo-server. send hello to echo-server");
				webSocket.SendString("hello!");
				webSocket.SendString("wooooo!");
				webSocket.SendString("looks!");
				webSocket.SendString("fine!");
			},

			// handler for receiving data from server. 
			datas => {
				/*
					this handler is called from system thread. not Unity's main thread.
					
					and, datas is ArraySegment<byte> x N. 

					SHOULD COPY byte data from datas HERE.

					do not copy ArraySegment<byte> itself.
					these data array will be destroyed soon after leaving this block.
				*/
				while (0 < datas.Count) {
					ArraySegment<byte> data = datas.Dequeue();

 					byte[] bytes = new byte[data.Count];
					Buffer.BlockCopy(data.Array, data.Offset, bytes, 0, data.Count);

					Debug.Log("message:" + Encoding.UTF8.GetString(bytes));
				}
			},
			() => {
				Debug.LogError("server ping received. automatically pong.");
			},
			closeReason => {
				Debug.Log("closed, closeReason:" + closeReason);
			},
			(errorEnum, exception) => {
				Debug.LogError("error, errorEnum:" + errorEnum + " exception:" + exception);
			},
			new Dictionary<string, string>{
				// set WebSocket connection header parameters here!
			}
		);
	}

	int frame = 0;
	void Update () {
		frame++;

		// interval basis check for connection status.
		// check connection / 1 sec.
		if (opened && frame % (60 * 1) == 0) {
			if (!webSocket.IsConnected()) {
				Debug.LogError("disconnection detected. frame:" + frame);
				opened = false;
			}
		}
	}

	void OnApplicationQuit () {
		webSocket.Disconnect(true);
	}
}