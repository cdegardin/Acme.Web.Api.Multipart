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
        /// Initializes a new instance of the <see cref="MultipartContractResolver"/> class.
        /// </summary>
        /// <param name="contractResolver">The contract resolver.</param>
        public MultipartContractResolver(IContractResolver contractResolver)
        {
            this.contractResolver = contractResolver;
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
                if (type == httpPostedFileBaseType && objectContract.Converter == null)
                {
                    objectContract.Converter = new MultipartFileJsonConverter();
                }
                else
                {
                    var listOfFileType = typeof(IEnumerable<HttpPostedFileBase>);
                    foreach (var property in objectContract.Properties.Where(p => !p.Ignored && (p.PropertyType == httpPostedFileBaseType || listOfFileType.IsAssignableFrom(p.PropertyType))))
                    {
                        property.ShouldSerialize = e => false;
                    }
                }
            }

            return contract;
        }
    }
}