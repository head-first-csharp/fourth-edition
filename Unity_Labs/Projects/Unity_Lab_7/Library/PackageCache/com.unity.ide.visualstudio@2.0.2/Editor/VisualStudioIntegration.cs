/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Unity.VisualStudio.Editor.Messaging;
using UnityEditor;
using UnityEngine;
using MessageType = Microsoft.Unity.VisualStudio.Editor.Messaging.MessageType;

namespace Microsoft.Unity.VisualStudio.Editor
{
	[InitializeOnLoad]
	internal class VisualStudioIntegration
	{
		private static Messager _messager;

		private static readonly Queue<Message> Incoming = new Queue<Message>();
		private static readonly object IncomingLock = new object();

		static VisualStudioIntegration()
		{
			if (!VisualStudioEditor.IsEnabled)
				return;

			RunOnceOnUpdate(() =>
			{
				// Despite using ReuseAddress|!ExclusiveAddressUse, we can fail here:
				// - if another application is using this port with exclusive access
				// - or if the firewall is not properly configured
				var messagingPort = MessagingPort();
				
				try {
					_messager = Messager.BindTo(messagingPort);
					_messager.ReceiveMessage += ReceiveMessage;
				}
				catch (SocketException)
				{
					// We'll have a chance to try to rebind on next domain reload
					Debug.LogWarning($"Unable to use UDP port {messagingPort} for VS/Unity messaging. You should check if another process is already bound to this port or if your firewall settings are compatible.");
				}

				RunOnShutdown(Shutdown);
			});

			EditorApplication.update += OnUpdate;
		}

		private static void RunOnceOnUpdate(Action action)
		{
			var callback = null as EditorApplication.CallbackFunction;

			callback = () =>
			{
				EditorApplication.update -= callback;
				action();
			};

			EditorApplication.update += callback;
		}

		private static void RunOnShutdown(Action action)
		{
			// Mono on OSX has all kinds of quirks on AppDomain shutdown
			if (!VisualStudioEditor.IsWindows)
				return;

			AppDomain.CurrentDomain.DomainUnload += (_, __) => action();
		}

		private static int DebuggingPort()
		{
			return 56000 + (System.Diagnostics.Process.GetCurrentProcess().Id % 1000);
		}

		private static int MessagingPort()
		{
			return DebuggingPort() + 2;
		}

		private static void ReceiveMessage(object sender, MessageEventArgs args)
		{
			OnMessage(args.Message);
		}

		private static void OnUpdate()
		{
			lock (IncomingLock)
			{
				while (Incoming.Count > 0)
				{
					ProcessIncoming(Incoming.Dequeue());
				}
			}
		}

		private static void AddMessage(Message message)
		{
			lock (IncomingLock)
			{
				Incoming.Enqueue(message);
			}
		}

		private static void ProcessIncoming(Message message)
		{
			switch (message.Type)
			{
				case MessageType.Ping:
					Answer(message, MessageType.Pong);
					break;
				case MessageType.Play:
					Shutdown();
					EditorApplication.isPlaying = true;
					break;
				case MessageType.Stop:
					EditorApplication.isPlaying = false;
					break;
				case MessageType.Pause:
					EditorApplication.isPaused = true;
					break;
				case MessageType.Unpause:
					EditorApplication.isPaused = false;
					break;
				case MessageType.Build:
					// Not used anymore
					break;
				case MessageType.Refresh:
					Refresh();
					break;
				case MessageType.Version:
					Answer(message, MessageType.Version, PackageVersion());
					break;
				case MessageType.UpdatePackage:
					// Not used anymore
					break;
				case MessageType.ProjectPath:
					Answer(message, MessageType.ProjectPath, Path.GetFullPath(Path.Combine(Application.dataPath, "..")));
					break;
			}
		}

		private static string PackageVersion()
		{
			var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(VisualStudioIntegration).Assembly);
			return package.version;
		}

		private static void Refresh()
		{
			RunOnceOnUpdate(AssetDatabase.Refresh);
		}

		private static void OnMessage(Message message)
		{
			AddMessage(message);
		}

		private static void Answer(Message message, MessageType answerType, string answerValue = "")
		{
			var targetEndPoint = message.Origin;

			Answer(
				targetEndPoint,
				answerType,
				answerValue);
		}

		private static void Answer(IPEndPoint targetEndPoint, MessageType answerType, string answerValue)
		{
			_messager?.SendMessage(targetEndPoint, answerType, answerValue);
		}

		private static void Shutdown()
		{
			if (_messager == null)
				return;

			_messager.ReceiveMessage -= ReceiveMessage;
			_messager.Dispose();
			_messager = null;
		}
	}
}
