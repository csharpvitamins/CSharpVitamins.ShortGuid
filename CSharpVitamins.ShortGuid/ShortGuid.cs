using System;
using System.Diagnostics;

namespace CSharpVitamins
{
    /// <summary>
    /// A convenience wrapper struct for dealing with URL-safe base64 encoded globally unique identifier (GUID),
    /// making a shorter string value (22 vs 36 characters long).
    /// </summary>
    /// <remarks>
    /// URL-safe base64? That's just a base64 string with well known special characters replaced (/, +) or removed (==)
    /// </remarks>
    [DebuggerDisplay("{Value}")]
    public struct ShortGuid
    {
        /// <summary>
        /// A read-only instance of the ShortGuid class whose value is guaranteed to be all zeroes.
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        Guid underlyingGuid;
        string encodedString;

        /// <summary>
        /// Creates a new instance with the given base64 encoded string.
        /// (See also <seealso cref="ShortGuid.TryParse(string, out ShortGuid)"/>)
        /// </summary>
        /// <param name="value">A ShortGuid encoded string.</param>
        public ShortGuid(string value)
        {
            encodedString = value;
            underlyingGuid = Decode(value);
        }

        /// <summary>
        /// Creates a new instance with the given Guid.
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        public ShortGuid(Guid guid)
        {
            encodedString = Encode(guid);
            underlyingGuid = guid;
        }

        /// <summary>
        /// Gets the underlying Guid for the encoded ShortGuid
        /// </summary>
        public Guid Guid => underlyingGuid;

        /// <summary>
        /// Gets the encoded string value of the Guid
        /// </summary>
        public string Value => encodedString;

        /// <summary>
        /// Returns the encoded string value
        /// </summary>
        /// <returns></returns>
        public override string ToString() => encodedString;

        /// <summary>
        /// Returns a value indicating whether this instance and a specified object represent the same type and value.
        /// Compares for equality against other string, Guid and ShortGuid types
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortGuid shortGuid)
            {
                return underlyingGuid.Equals(shortGuid.underlyingGuid);
            }

            if (obj is Guid guid)
            {
                return underlyingGuid.Equals(guid);
            }

            if (obj is string str)
            {
                // try a ShortGuid string
                if (TryDecode(str, out guid))
                    return underlyingGuid.Equals(guid);

                // try a guid string
                if (Guid.TryParse(str, out guid))
                    return underlyingGuid.Equals(guid);
            }

            return false;
        }

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => underlyingGuid.GetHashCode();

        /// <summary>
        /// Initialises a new instance of the ShortGuid using <see cref="Guid.NewGuid()"/>.
        /// </summary>
        /// <returns></returns>
        public static ShortGuid NewGuid() => new ShortGuid(Guid.NewGuid());

        /// <summary>
        /// Encodes the given value as an encoded ShortGuid string. The encoding is similar to
        /// Base64, with some non-URL safe characters replaced, and padding removed.
        /// </summary>
        /// <param name="value">Any valid <see cref="Guid"/> string</param>
        /// <returns>A 22 character ShortGuid string</returns>
        public static string Encode(string value)
        {
            var guid = new Guid(value);
            return Encode(guid);
        }

        /// <summary>
        /// Encodes the given <see cref="Guid"/> as an encoded ShortGuid string. The encoding is similar to
        /// Base64, with some non-URL safe characters replaced, and padding removed.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to encode</param>
        /// <returns>A 22 character ShortGuid string</returns>
        public static string Encode(Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());

            encoded = encoded
                .Replace("/", "_")
                .Replace("+", "-");

            return encoded.Substring(0, 22);
        }

        /// <summary>
        /// Decodes the given value to a <see cref="Guid"/>.
        /// See also <seealso cref="TryDecode(string, out Guid)"/> or <seealso cref="TryParse(string, out Guid)"/>.
        /// </summary>
        /// <param name="value">The ShortGuid encoded string to decode.</param>
        /// <returns>A new <see cref="Guid"/> instance from the parsed string.</returns>
        public static Guid Decode(string value)
        {
            value = value
                .Replace("_", "/")
                .Replace("-", "+");

            byte[] blob = Convert.FromBase64String(value + "==");

            return new Guid(blob);
        }

        /// <summary>
        /// Decodes the given value to a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The ShortGuid encoded string to decode.</param>
        /// <param name="guid">A new <see cref="Guid"/> instance from the parsed string.</param>
        /// <returns>A boolean indicating if the decode was successful.</returns>
        public static bool TryDecode(string value, out Guid guid)
        {
            try
            {
                guid = Decode(value);
                return true;
            }
            catch
            {
                guid = Guid.Empty;
                return false;
            }
        }

        #region Operators

        /// <summary>
        /// Determines if both ShortGuids have the same underlying Guid value.
        /// </summary>
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            if (ReferenceEquals(x, null))
            {
                return ReferenceEquals(y, null);
            }

            return x.underlyingGuid == y.underlyingGuid;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the same underlying Guid value.
        /// </summary>
        public static bool operator !=(ShortGuid x, ShortGuid y) => !(x == y);

        /// <summary>
        /// Implicitly converts the ShortGuid to its string equivalent
        /// </summary>
        public static implicit operator string(ShortGuid shortGuid) => shortGuid.encodedString;

        /// <summary>
        /// Implicitly converts the ShortGuid to its Guid equivalent
        /// </summary>
        public static implicit operator Guid(ShortGuid shortGuid) => shortGuid.underlyingGuid;

        /// <summary>
        /// Implicitly converts the string to a ShortGuid
        /// </summary>
        public static implicit operator ShortGuid(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ShortGuid.Empty;

            if (TryParse(value, out ShortGuid shortGuid))
                return shortGuid;

            throw new FormatException("ShortGuid should contain 22 base64 characters or Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).");
        }

        /// <summary>
        /// Implicitly converts the Guid to a ShortGuid
        /// </summary>
        public static implicit operator ShortGuid(Guid guid)
        {
            if (guid == Guid.Empty)
                return ShortGuid.Empty;

            return new ShortGuid(guid);
        }

        #endregion

        /// <summary>
        /// Tries to parse the value as a ShortGuid or Guid string.
        /// </summary>
        /// <param name="value">The ShortGuid encoded string or string representation of a Guid.</param>
        /// <param name="shortGuid">A new <see cref="ShortGuid"/> instance from the parsed string.</param>
        /// <returns>A boolean indicating if the parse was successful.</returns>
        public static bool TryParse(string value, out ShortGuid shortGuid)
        {
            // try a ShortGuid string
            if (ShortGuid.TryDecode(value, out var guid))
            {
                shortGuid = guid;
                return true;
            }

            // try a Guid string
            if (Guid.TryParse(value, out guid))
            {
                shortGuid = guid;
                return true;
            }

            shortGuid = ShortGuid.Empty;
            return false;
        }

        /// <summary>
        /// Tries to parse the value as a ShortGuid or Guid string, and outputs the underlying Guid value.
        /// </summary>
        /// <param name="value">The ShortGuid encoded string or string representation of a Guid.</param>
        /// <param name="guid">A new <see cref="Guid"/> instance from the parsed string.</param>
        /// <returns>A boolean indicating if the parse was successful.</returns>
        public static bool TryParse(string value, out Guid guid)
        {
            // try a ShortGuid string
            if (ShortGuid.TryDecode(value, out guid))
                return true;

            // try a Guid string
            if (Guid.TryParse(value, out guid))
                return true;

            guid = Guid.Empty;
            return false;
        }
    }
}
