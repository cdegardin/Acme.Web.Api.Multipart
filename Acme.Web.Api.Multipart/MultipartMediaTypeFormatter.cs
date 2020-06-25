// <copyright file="MultipartMediaTypeFormatter.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
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
        private static readonly HashSet<string> ValidJsonMediaTypes = new HashSet<string>(GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Select(m => m.MediaType), StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The files
        /// </summary>
        [ThreadStatic]
        private static IDictionary<string, MyHttpPostedFileBase> files;

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
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);
            var provider = new MultipartFileStreamProvider(tempPath);
            await content.ReadAsMultipartAsync(provider, cancellationToken).ConfigureAwait(false);
            var json = provider.FileData.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Trim('"') == "$json$" && ValidJsonMediaTypes.Contains(c.Headers.ContentType?.MediaType));
            if (json == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Missing $json$ part." });
            }

            var files = new Dictionary<string, MyHttpPostedFileBase>();
            foreach (var file in provider.FileData.Where(c => c != json))
            {
                files.Add(file.Headers.ContentDisposition.Name.Trim('"'), new MultipartFile(file, File.OpenRead(file.LocalFileName)));
            }

            if (files.Any())
            {
                context.HttpContext.Current?.AddOnRequestCompleted(c =>
                {
                    foreach (var file in files.Values.OfType<IDisposable>())
                    {
                        file.Dispose();
                    }

                    Directory.Delete(tempPath, true);
                });
            }

            using (var jsonStream = File.OpenRead(json.LocalFileName))
            using (var jsonContent = new StreamContent(jsonStream) { Headers = { json.Headers } })
            {
                try
                {
                    MultipartMediaTypeFormatter.files = files;
                    return await GlobalConfiguration.Configuration.Formatters.JsonFormatter.ReadFromStreamAsync(type, jsonStream, jsonContent, formatterLogger);
                }
                finally
                {
                    MultipartMediaTypeFormatter.files = null;
                }
            }
        }

        /// <summary>
        /// Gets the file for the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The file if found; Otherwise <c>null</c>.</returns>
        internal static MyHttpPostedFileBase GetFile(string key) => files.TryGetValue(key, out var result) ? result : null;
    }
}