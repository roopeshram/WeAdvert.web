using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using AdvertApi.Models;
using AutoMapper;
using System.Text;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        private readonly string _baseAddress;

        public AdvertApiClient(IConfiguration configuration,HttpClient client, IMapper mapper)
        {
            _configuration = configuration;
            _client = client;
            _mapper = mapper;

            _baseAddress = configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
            //_client.DefaultRequestHeaders.Add("Content-type", "application/json");

        }

        public async Task<bool> Confirm(ConfirmAdvertRequest model)
        {
            var advertModel = _mapper.Map<ConfirmAdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertModel);
            var response = await _client
                .PutAsync(new Uri($"{_baseAddress}/confirm"),
                    new StringContent(jsonModel))
                .ConfigureAwait(false);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<AdvertResponse> Create(CreateAdvertModel model)
        {
            var advertapimodel = _mapper.Map<AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertapimodel);
            var response = await _client.PostAsync(new Uri($"{_baseAddress}/Create"), new StringContent(jsonModel, Encoding.UTF8, "application/json")).ConfigureAwait(false); 
            var responsejson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var createadvertresponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responsejson);
            var advertResponse = _mapper.Map<AdvertResponse>(createadvertresponse);
            return advertResponse;
        }
    }
}
