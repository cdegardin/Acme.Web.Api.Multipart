// <copyright file="MultipartFileJsonConverter.cs" company="ACME">
// Copyright (c) ACME. All rights reserved.
// </copyright>

namespace Acme.Web.Api.Multipart
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Newtonsoft.Json;

    /// <summary>
    ///   <see cref="MultipartFileJsonConverter"/>.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class MultipartFileJsonConverter : JsonConverter
    {
        /// <summary>
        /// The files
        /// </summary>
        private readonly IDictionary<string, HttpPostedFileBase> files;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFileJsonConverter"/> class.
        /// </summary>
        /// <param name="files">The files.</param>
        public MultipartFileJsonConverter(IDictionary<string, HttpPostedFileBase> files)
        {
            this.files = files;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
        /// </summary>
        /// <value>
        ///   <c>false</c> this <see cref="T:Newtonsoft.Json.JsonConverter" /> can not write JSON.
        /// </value>
        public override bool CanWrite => false;

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) => objectType == typeof(HttpPostedFileBase);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (reader.Value is string fileReference && this.files.TryGetValue(fileReference, out var file)) ?
                    file :
                    null;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}