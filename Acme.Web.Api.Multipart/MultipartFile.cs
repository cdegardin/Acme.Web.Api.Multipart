// <copyright file="MultipartFile.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Web;

    /// <summary>
    ///   <see cref="MultipartFile"/>.
    /// </summary>
    /// <seealso cref="System.Web.HttpPostedFileBase" />
    /// <seealso cref="System.IDisposable" />
    public sealed class MultipartFile : HttpPostedFileBase, IDisposable
    {
        /// <summary>
        /// The content
        /// </summary>
        private readonly HttpContent content;

        /// <summary>
        /// The stream
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFile"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="stream">The stream.</param>
        public MultipartFile(HttpContent content, Stream stream)
        {
            this.content = content;
            this.stream = stream;
        }

        /// <summary>
        /// Gets the size of an uploaded file, in bytes.
        /// </summary>
        public override int ContentLength => (int)this.stream.Length;

        /// <summary>
        /// Gets the MIME content type of an uploaded file.
        /// </summary>
        public override string ContentType => this.content.Headers.ContentType?.MediaType;

        /// <summary>
        /// Gets the fully qualified name of the file on the client.
        /// </summary>
        public override string FileName => this.content.Headers.ContentDisposition?.FileName?.Trim('"');

        /// <summary>
        /// Gets a <see cref="T:System.IO.Stream" /> object that points to an uploaded file to prepare for reading the contents of the file.
        /// </summary>
        public override Stream InputStream => this.stream;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.stream.Dispose();
            this.content.Dispose();
        }
    }
}