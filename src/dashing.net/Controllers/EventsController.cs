using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.UI;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using dashing.net.Infrastructure;
using dashing.net.common;
using dashing.net.jobs;
using dashing.net.streaming;

namespace dashing.net.Controllers
{
    public class EventsController : Hub
    {
        private static readonly ConcurrentQueue<StreamWriter> _streamWriter = new ConcurrentQueue<StreamWriter>();
        private static readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();

        private static bool _hasInstantiated = false;

        public void Send() {
            if (_hasInstantiated == false) {
                LoadJobs();
            }
        }

        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task"/>.
        /// </returns>
        public override Task OnConnected() {
            if (_hasInstantiated == false) {
                LoadJobs();

                _hasInstantiated = true;
            }

            //Action<string> messageWriter = serialised => Clients.Caller.sendMessage(serialised);
            //Action<dynamic> sender = (msg) => SendMessageinternal(msg, messageWriter);

            //foreach (CommonJobs.IJob job in CommonJobs.Jobs.GetJobCollection()) {
            //    job.SendInitialState(sender);
            //}

            return base.OnConnected();
        }

        ///// <summary>
        ///// Inspiration http://techbrij.com/real-time-chart-html5-push-sse-asp-net-web-api
        ///// </summary>
        //public HttpResponseMessage Get(HttpRequestMessage request)
        //{
        //    HttpResponseMessage response = request.CreateResponse();
        //    response.Content = new PushStreamContent(WriteToStream, "text/event-stream");

        //    if (!_hasInstantiated)
        //    {
        //        LoadJobs();

        //        _hasInstantiated = true;
        //    }

        //    return response;
        //}

        private void WriteToStream(Stream outputStream, HttpContent headers, TransportContext context)
        {
            var streamWriter = new StreamWriter(outputStream);
            _streamWriter.Enqueue(streamWriter);
        }

        private void SendMessage(dynamic message)
        {
            var updatedAt = TimeHelper.ElapsedTimeSinceEpoch();

            if (message.GetType() == typeof(JObject))
            {
                message.updatedAt = updatedAt;
            }
            else
            {
                message = JsonHelper.Merge(message, new { updatedAt });
            }

            var serialized = JsonConvert.SerializeObject(message);
            _messageQueue.TryAdd(serialized);
        }

        private void ProcessQueue()
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                Clients.All.sendMessage(message);
                //for (int i = 0; i < _streamWriter.Count; i++)
                //{
                //    StreamWriter streamWriter;
                //    if (_streamWriter.TryDequeue(out streamWriter))
                //    {
                //        try
                //        {
                //            streamWriter.WriteLine(message);
                //            streamWriter.Flush();
                //        }
                //        catch (Exception)
                //        {
                //            // dont re-add the stream as an error ocurred presumable the client has lost connection
                //            break;
                //        }

                //        _streamWriter.Enqueue(streamWriter);
                //    }
                //}
            }
        }

        private void LoadJobs()
        {
            Dashing.SendMessage = SendMessage;

            Task.Factory.StartNew(ProcessQueue);

            Jobs.Start();
        }
    }
}
