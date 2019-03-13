// <copyright file="InternalExtensions.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System.Net.Http.Headers;

    /// <summary>
    ///   <see cref="InternalExtensions"/>.
    /// </summary>
    internal static class InternalExtensions
    {
        /// <summary>
        /// Adds the specified source.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        public static void Add(this HttpContentHeaders destination, HttpContentHeaders source)
        {
            foreach (var header in source)
            {
                destination.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
    }
}