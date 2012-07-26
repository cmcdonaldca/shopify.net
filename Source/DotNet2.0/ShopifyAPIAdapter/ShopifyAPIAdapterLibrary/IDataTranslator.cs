using System;
using System.Collections.Generic;
using System.Text;

namespace ShopifyAPIAdapterLibrary
{

    /// <summary>
    /// An instance of this interface would translate the data between c# and the Shopify API
    /// </summary>
    public interface IDataTranslator
    {
        /// <summary>
        /// Encode the data in a way that is expected by the Shopify API
        /// </summary>
        /// <param name="data">data that should be encoded for the Shopify API</param>
        /// <returns></returns>
        string Encode(object data);

        /// <summary>
        /// Decode the data returned by the Shopify API
        /// </summary>
        /// <param name="encodedData">data encoded by the Shopify API</param>
        /// <returns></returns>
        object Decode(string encodedData);

        /// <summary>
        /// The Content Type (Mime Type) used by this translator
        /// </summary>
        /// <returns></returns>
        string GetContentType();
    }
}
