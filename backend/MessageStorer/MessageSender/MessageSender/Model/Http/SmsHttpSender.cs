﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class SmsHttpSender : HttpSender
    {
        private const string URL = "api/messages";
        private const string SYNC_DATE_TIME_POSTFIX = "syncDateTime";

        public SmsHttpSender(string baseAddress) : base(baseAddress)
        {
        }

        public async Task<SmsWithId> Send(Sms sms)
        {
            var json = JsonConvert.SerializeObject(sms);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _http.PostAsync(URL, data);
            await CheckResponse(result);
            var body = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SmsWithId>(body);
        }
        public async Task<SyncDateTime> GetLastSyncTime()
        {
            var result = await _http.GetAsync($"{URL}/{SYNC_DATE_TIME_POSTFIX}");
            await CheckResponse(result);
            var body = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SyncDateTime>(body);
        }
    }
}
