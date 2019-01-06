﻿// <copyright file="MultipartMediaTypeFormatter.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    ///   <see cref="MultipartMediaTypeFormatter"/>.
    /// </summary>
    /// <seealso cref="System.Net.Http.Formatting.MediaTypeFormatter" />
    public class MultipartMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        /// The valid json media types
        /// </summary>
        private static readonly HashSet<string> ValidJsonMediaTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "application/json", "text/json" };

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartMediaTypeFormatter"/> class.
        /// </summary>
        public MultipartMediaTypeFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserialize the type; otherwise, false.
        /// </returns>
        public override bool CanReadType(Type type) => GlobalConfiguration.Configuration.Formatters.JsonFormatter.CanReadType(type);

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can serializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to serialize.</param>
        /// <returns>
        /// <c>false</c> this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can not serialize the type.
        /// </returns>
        public override bool CanWriteType(Type type) => false;

        /// <summary>
        /// Asynchronously deserializes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="readStream">The <see cref="T:System.IO.Stream" /> to read.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent" />, if available. It may be null.</param>
        /// <param name="formatterLogger">The <see cref="T:System.Net.Http.Formatting.IFormatterLogger" /> to log events to.</param>
        /// <param name="cancellationToken">The token to cancel the operation.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> whose result will be an object of the given type.
        /// </returns>
        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken cancellationToken)
        {
            var multipart = await content.ReadAsMultipartAsync(cancellationToken);
            var json = multipart.Contents.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Trim('"') == "$json$" && ValidJsonMediaTypes.Contains(c.Headers.ContentType?.MediaType));
            if (json == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { ReasonPhrase = "Missing $json$ part." });
            }

            var files = new Dictionary<string, HttpPostedFileBase>();
            foreach (var file in multipart.Contents.Where(c => c != json))
            {
                files.Add(file.Headers.ContentDisposition.Name.Trim('"'), new MultipartFile(file, await file.ReadAsStreamAsync()));
            }

            object result = null;
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            using (var jsonStream = await json.ReadAsStreamAsync())
            using (var reader = jsonFormatter.CreateJsonReader(type, jsonStream, jsonFormatter.SelectCharacterEncoding(content.Headers)))
            {
                var serializer = jsonFormatter.CreateJsonSerializer();
                serializer.Error += (o, e) =>
                {
                    var errorContext = e.ErrorContext;
                    formatterLogger.LogError(errorContext.Path, errorContext.Error);
                    errorContext.Handled = true;
                };
                serializer.ContractResolver = new MultipartContractResolver(serializer.ContractResolver, files);
                result = serializer.Deserialize(reader, type);
            }

            if (files.Any())
            {
                HttpContext.Current?.AddOnRequestCompleted(c =>
                {
                    foreach (var file in files.Values.OfType<IDisposable>())
                    {
                        file.Dispose();
                    }
                });
            }

            return result;
        }
    }
}