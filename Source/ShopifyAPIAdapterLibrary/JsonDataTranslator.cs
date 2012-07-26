﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShopifyAPIAdapterLibrary
{
    /// <summary>
    /// This class is used to translate to and from C# object and JSON strings 
    /// </summary>
    public class JsonDataTranslator : IDataTranslator
    {
        /// <summary>
        /// Given a C# object, return a JSON string that can be used by the Shopify API
        /// </summary>
        /// <param name="data">a c# object that maps to a JSON object</param>
        /// <returns>JSON string</returns>
        public string Encode(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Given a JSON String, return a corresponding C# object
        /// </summary>
        /// <param name="encodedData">JSON string return from the Shopify API</param>
        /// <returns>c# object corresponding to the JSON data return from the Shopify API</returns>
        public object Decode(string encodedData)
        {
            return JObject.Parse(encodedData);
        }

        /// <summary>
        /// The content type used by JSON
        /// </summary>
        /// <returns></returns>
        public string GetContentType()
        {
            return ContentType;
        }

        /// <summary>
        /// The content type used by JSON
        /// </summary>
        public static readonly string ContentType = "application/json";
    }
}