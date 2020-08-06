using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ECC.Servicos.Util
{
    public class SmsGateway
    {
        private string _baseUri = "http://smsgateway.me/api/v3/";
        private string _user;
        private string _password;
        private List<dynamic> _listMessage;
        public bool Result { get; private set; }

        public SmsGateway(string user, string password)
        {
            this._user = user;
            this._password = password;
            this._listMessage = new List<dynamic>();
        }

        /// <summary>
        /// Envia a mensagem para a API do SmsGateway
        /// </summary>
        /// <param name="device"></param>
        /// <param name="number"></param>
        /// <param name="message"></param>
        /// <param name="send_at">'send_at' => strtotime('+10 minutes'), // Send the message in 10 minutes</param>
        /// <param name="expires_at">'expires_at' => strtotime('+1 hour') // Cancel the message in 1 hour if the message is not yet sent</param>
        public void SendMessage(string device, string number, string message, string send_at = null, string expires_at = null, Action<JToken> success = null, Action<string> error = null)
        {
            this.post(new { email = this._user, password = this._password, device = device, number = number, message = message, send_at = send_at, expires_at = expires_at }, success, error);
        }

        /// <summary>
        /// Envia a mensagem para a API do SmsGateway
        /// </summary>
        /// <param name="device"></param>
        /// <param name="number"></param>
        /// <param name="message"></param>
        /// <param name="send_at">'send_at' => strtotime('+10 minutes'), // Send the message in 10 minutes</param>
        /// <param name="expires_at">'expires_at' => strtotime('+1 hour') // Cancel the message in 1 hour if the message is not yet sent</param>
        public void SendMessageToManyNumbers(string device, string[] number, string message, string send_at = null, string expires_at = null, Action<JToken> success = null, Action<string> error = null)
        {
            this.post(new { email = this._user, password = this._password, device = device, number = number, message = message, send_at = send_at, expires_at = expires_at }, success, error);
        }

        public void AddMessage(string device, string number, string message, string send_at = null, string expires_at = null)
        {
            this._listMessage.Add(new { device = device, number = number, message = message, send_at = send_at, expires_at = expires_at });
        }

        public void AddMessage(string device, string[] number, string message, string send_at = null, string expires_at = null, Action success = null, Action<string> error = null)
        {
            this._listMessage.Add(new { device = device, number = number, message = message, send_at = send_at, expires_at = expires_at });
        }

        public void SendManyMessages(Action<JToken> success = null, Action<string> error = null)
        {
            this.post(new { email = this._user, password = this._password, data = _listMessage.ToArray() }, (result) =>
            {
                if (success != null)
                    success.Invoke(result);

                this._listMessage.Clear();
            }, error);
        }

        public void GetDevices(Action<List<string>> success = null, Action<string> error = null)
        {
            this.get("devices", (result) =>
            {
                if (success != null)
                {
                    try
                    {
                        var array = result["data"] as JArray;
                        var list = array.Where(x => UnixTimeStampToDateTime(double.Parse(x["last_seen"].ToString())) > DateTime.Now.AddMinutes(-5)).Select(x => x["id"].ToString()).ToList();

                        success(list);
                    }
                    catch (Exception ex)
                    {
                        if (error != null)
                            error.Invoke(ex.Message);
                    }
                }
            }, error);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void GetMessage(string id, Action<JToken> success = null, Action<string> error = null)
        {
            this.get(string.Format("messages/view/{0}", id), success, error);
        }

        private async void post(object parameters, Action<JToken> success, Action<string> error)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this._baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsJsonAsync("messages/send", parameters);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsAsync<dynamic>();
                        var json = result.Result as JObject;

                        if (!json["result"]["fails"].HasValues)
                        {
                            this.Result = true;

                            if (success != null)
                                success.Invoke(json["result"]["success"]);

                            return;
                        }
                        else
                        {
                            this.Result = true;

                            if (error != null)
                                error.Invoke(json["result"]["fails"].ToString());

                            return;
                        }
                    }
                    else
                    {
                        if (error != null)
                            error.Invoke("Not response.IsSuccessStatusCode.");
                    }
                }

                this.Result = false;
            }
            catch (Exception ex)
            {
                if (error != null)
                    error.Invoke(ex.Message);
            }
        }

        private async void get(string method, Action<JToken> success, Action<string> error)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this._baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(string.Format("{0}?email={1}&password={2}", method, this._user, this._password));

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsAsync<dynamic>();
                        var json = result.Result as JObject;

                        if (json.GetValue("success").Value<bool>())
                        {
                            this.Result = true;

                            if (success != null)
                                success.Invoke(json["result"]);

                            return;
                        }
                        else
                        {
                            this.Result = true;

                            if (error != null)
                                error.Invoke(json.ToString());

                            return;
                        }
                    }
                    else
                    {
                        if (error != null)
                            error.Invoke("Not response.IsSuccessStatusCode.");
                    }

                    this.Result = false;
                }
            }
            catch (Exception ex)
            {
                if (error != null)
                    error.Invoke(ex.Message);
            }
        }
    }
}
