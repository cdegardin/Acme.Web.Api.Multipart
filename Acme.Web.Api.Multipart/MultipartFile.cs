// <copyright file="MultipartFile.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    ///   <see cref="MultipartFile"/>.
    /// </summary>
    /// <seealso cref="MyHttpPostedFileBase" />
    /// <seealso cref="IDisposable" />
    public sealed class MultipartFile : MyHttpPostedFileBase, IDisposable
    {
        /// <summary>
        /// The content
        /// </summary>
        private readonly HttpContent content;

        /// <summary>
        /// The headers
        /// </summary>
        private readonly HttpContentHeaders headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFile" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="stream">The stream.</param>
        public MultipartFile(MultipartFileData file, Stream stream)
        {
            this.headers = file.Headers;
            this.InputStream = stream;
        }

        /// <summary>
        /// Gets the size of an uploaded file, in bytes.
        /// </summary>
        public override int ContentLength => (int)this.InputStream.Length;

        /// <summary>
        /// Gets the MIME content type of an uploaded file.
        /// </summary>
        public override string ContentType => this.headers.ContentType?.MediaType;

        /// <summary>
        /// Gets the fully qualified name of the file on the client.
        /// </summary>
        public override string FileName => this.headers.ContentDisposition?.FileName?.Trim('"');

        /// <summary>
        /// Gets a <see cref="T:System.IO.Stream" /> object that points to an uploaded file to prepare for reading the contents of the file.
        /// </summary>
        public override Stream InputStream { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.InputStream.Dispose();
            this.content.Dispose();
        }
    }
}