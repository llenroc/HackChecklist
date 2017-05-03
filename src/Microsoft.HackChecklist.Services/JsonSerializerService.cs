//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using Microsoft.HackChecklist.Services.Contracts;

namespace Microsoft.HackChecklist.Services
{
    public class JsonSerializerService : IJsonSerializerService
    {
        public JsonSerializerService()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        public string Serialize<T>(T data)
        {
            return data == null 
                ? string.Empty 
                : JsonConvert.SerializeObject(data);
        }

        public T Deserialize<T>(string strData)
        {
            return string.IsNullOrWhiteSpace(strData)
                ? default(T)
                : JsonConvert.DeserializeObject<T>(strData, new StringEnumConverter());
        }        
    }
}
