// <copyright file="MyHttpPostedFileBase.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
#if NET452
    using System.Web;
#else
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
#endif

#if NET452
    /// <summary>
    /// The HttpPostedFileBase.
    /// </summary>
    /// <seealso cref="HttpPostedFileBase" />
    public abstract class MyHttpPostedFileBase : HttpPostedFileBase
    {
    }
#else
    /// <summary>
    /// The HttpPostedFileBase.
    /// </summary>
    /// <seealso cref="IFormFile" />
    public abstract class MyHttpPostedFileBase : IFormFile
    {
        /// <inheritdoc/>
        public string ContentDisposition => throw new NotImplementedException();

        /// <summary>
        /// When overridden in a derived class, gets the size of an uploaded file, in bytes.
        /// </summary>
        public virtual int ContentLength => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual string ContentType => throw new NotImplementedException();
        
        /// <inheritdoc/>
        public virtual string FileName => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual IHeaderDictionary Headers => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public long Length => this.ContentLength;

        /// <summary>
        /// Gets a <see cref="T:System.IO.Stream" /> object that points to an uploaded file to prepare for reading the contents of the file.
        /// </summary>
        public virtual Stream InputStream => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Stream OpenReadStream()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// When overridden in a derived class, saves the contents of an uploaded file.
        /// </summary>
        /// <param name="filename">The name of the file to save.</param>
        public virtual void SaveAs(string filename)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
