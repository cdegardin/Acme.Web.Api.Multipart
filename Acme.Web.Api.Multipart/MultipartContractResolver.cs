// <copyright file="MultipartContractResolver.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Newtonsoft.Json.Serialization;

    /// <summary>
    ///   <see cref="MultipartContractResolver"/>.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.IContractResolver" />
    public class MultipartContractResolver : IContractResolver
    {
        /// <summary>
        /// The contract resolver
        /// </summary>
        private readonly IContractResolver contractResolver;

        /// <summary>
        /// The files
        /// </summary>
        private readonly IDictionary<string, HttpPostedFileBase> files;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartContractResolver"/> class.
        /// </summary>
        /// <param name="contractResolver">The contract resolver.</param>
        /// <param name="files">The files.</param>
        public MultipartContractResolver(IContractResolver contractResolver, IDictionary<string, HttpPostedFileBase> files)
        {
            this.contractResolver = contractResolver;
            this.files = files;
        }

        /// <summary>
        /// Resolves the contract for a given type.
        /// </summary>
        /// <param name="type">The type to resolve a contract for.</param>
        /// <returns>
        /// The contract for a given type.
        /// </returns>
        public JsonContract ResolveContract(Type type)
        {
            var contract = this.contractResolver.ResolveContract(type);
            if (contract is JsonObjectContract objectContract)
            {
                var httpPostedFileBaseType = typeof(HttpPostedFileBase);
                if (objectContract.UnderlyingType == httpPostedFileBaseType && objectContract.Converter == null)
                {
                    objectContract.Converter = new MultipartFileJsonConverter(this.files);
                }
                else
                {
                    var listOfFileType = typeof(IEnumerable<HttpPostedFileBase>);
                    foreach (var property in objectContract.Properties.Where(p => p.PropertyType == httpPostedFileBaseType || listOfFileType.IsAssignableFrom(p.PropertyType)))
                    {
                        property.Ignored = false;
                    }
                }
            }

            return contract;
        }
    }
}