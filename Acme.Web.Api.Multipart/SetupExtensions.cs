// <copyright file="SetupExtensions.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System.Linq;
#if NET452
    using System.Web.Http;
#endif

    /// <summary>
    ///   <see cref="SetupExtensions"/>.
    /// </summary>
    public static class SetupExtensions
    {
#if NET452
        /// <summary>
        /// Registers the multipart formatter.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public static void RegisterMultipartFormatter(this HttpConfiguration configuration)
        {
            var formatters = configuration.Formatters;
            if (!formatters.OfType<MultipartMediaTypeFormatter>().Any())
            {
                formatters.Add(new MultipartMediaTypeFormatter());
            }

            var contractResolver = formatters.JsonFormatter.SerializerSettings.ContractResolver;
            if (!(contractResolver is MultipartContractResolver))
            {
                formatters.JsonFormatter.SerializerSettings.ContractResolver = new MultipartContractResolver(contractResolver);
            }
        }
#endif
    }
}