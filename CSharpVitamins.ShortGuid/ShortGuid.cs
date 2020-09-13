using System;
using System.Diagnostics;

namespace CSharpVitamins
{
    /// <summary>
    /// A convenience wrapper struct for dealing with URL-safe Base64 encoded globally unique identifiers (GUID),
    /// making a shorter string value (22 vs 36 characters long).
    /// </summary>
    /// <remarks>
    /// What is URL-safe Base64? That's just a Base64 string with well known special characters replaced (/, +)
    /// or removed (==).
    /// </remarks>
    [DebuggerDisplay("{Value}")]
    public struct ShortGuid
    {
        /// <summary>
        /// A read-only instance of the ShortGuid struct whose value is guaranteed to be all zeroes i.e. equivilent
        /// to <see cref="Guid.Empty"/>.
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        private Guid underlyingGuid;
        private string encodedString;

        /// <summary>
        /// Creates a new instance with the given URL-safe Base64 encoded string.
        /// See also <seealso cref="ShortGuid.TryParse(string, out ShortGuid)"/> which will try to coerce the
        /// the value from URL-safe Base64 or normal Guid string
        /// </summary>
        /// <param name="value">A ShortGuid encoded string e.g. URL-safe Base64.</param>
        public ShortGuid(string value)
        {
            encodedString = value;
            underlyingGuid = Decode(value);
        }

        /// <summary>
        /// Creates a new instance with the given <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="guid">The <see cref="System.Guid"/> to encode.</param>
        public ShortGuid(Guid guid)
        {
            encodedString = Encode(guid);
            underlyingGuid = guid;
        }

        /// <summary>
        /// Gets the underlying <see cref="System.Guid"/> for the encoded ShortGuid.
        /// </summary>
        public Guid Guid => underlyingGuid;

        /// <summary>
        /// Gets the encoded string value of the <see cref="Guid"/> i.e. a URL-safe Base64 string.
        /// </summary>
        public string Value => encodedString;

        /// <summary>
        /// Returns the encoded URL-safe Base64 string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => encodedString;

        /// <summary>
        /// Returns a value indicating whether this instance and a specified object represent the same type and value.
        /// Compares for equality against other string, Guid and ShortGuid types.
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
                // Try a ShortGuid string.
                if (TryDecode(str, out guid))
                    return underlyingGuid.Equals(guid);

                // Try a guid string.
                if (Guid.TryParse(str, out guid))
                    return underlyingGuid.Equals(guid);
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for the underlying <see cref="System.Guid"/>.
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
        /// <param name="value">Any valid <see cref="System.Guid"/> string.</param>
        /// <returns>A 22 character ShortGuid URL-safe Base64 string.</returns>
        public static string Encode(string value)
        {
            var guid = new Guid(value);
            return Encode(guid);
        }

        /// <summary>
        /// Encodes the given <see cref="System.Guid"/> as an encoded ShortGuid string. The encoding is similar to
        /// Base64, with some non-URL safe characters replaced, and padding removed.
        /// </summary>
        /// <param name="guid">The <see cref="System.Guid"/> to encode.</param>
        /// <returns>A 22 character ShortGuid URL-safe Base64 string.</returns>
        public static string Encode(Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());

            encoded = encoded
                .Replace("/", "_")
                .Replace("+", "-");

            return encoded.Substring(0, 22);
        }

        /// <summary>
        /// Decodes the given value to a <see cref="System.Guid"/>.
        /// <para>See also <seealso cref="TryDecode(string, out Guid, bool)"/> or <seealso cref="TryParse(string, out Guid)"/>.</para>
        /// </summary>
        /// <param name="value">The ShortGuid encoded string to decode.</param>
        /// <param name="strict">If true the re-encoded result has to exactly match the input <paramref name="value"/>; if false any valid base64 string will be accepted.</param>
        /// <returns>A new <see cref="System.Guid"/> instance from the parsed string.</returns>
        /// <exception cref="FormatException">If <paramref name="value"/> is no valid base64 string (<seealso cref="Convert.FromBase64String(string)"/>) or if the <paramref name="strict"/> flag is set and the re-encoded output doesn't match <paramref name="value"/>.</exception>
        public static Guid Decode(string value, bool strict = false)
        {
            value = value
                .Replace("_", "/")
                .Replace("-", "+");

            byte[] blob = Convert.FromBase64String(value + "==");
            var guid = new Guid(blob);
            if (!strict)
            {
                return guid;
            }
            var reencodedOutput = new ShortGuid(guid).Value;
            if (reencodedOutput == value)
            {
                return guid;
            }
            throw new FormatException($"The string '{value}' is a valid base64 string but doesn't match the re-encoded {nameof(ShortGuid)} '{reencodedOutput}'.");
        }

        /// <summary>
        /// Attempts to decode the given value to a <see cref="System.Guid"/>.
        ///
        /// <para>The difference between TryParse and TryDecode:</para>
        /// <list type="number">
        ///     <item>
        ///         <term><see cref="TryParse(string, out ShortGuid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> before attempting parsing as a <see cref="System.Guid"/>, outputs the actual <see cref="ShortGuid"/> instance.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="TryParse(string, out Guid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> before attempting parsing as a <see cref="System.Guid"/>, outputs the underlying <see cref="System.Guid"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="TryDecode(string, out Guid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> only, but outputs the result as a <see cref="System.Guid"/> - this method.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="value">The ShortGuid encoded string to decode.</param>
        /// <param name="guid">A new <see cref="System.Guid"/> instance from the parsed string.</param>
        /// <param name="strict">If true the re-encoded result has to exactly match the input <paramref name="value"/>; if false any valid base64 string will be accepted.</param>
        /// <returns>A boolean indicating if the decode was successful.</returns>
        public static bool TryDecode(string value, out Guid guid, bool strict = false)
        {
            try
            {
                guid = Decode(value, strict);
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
        /// Determines if both ShortGuid instances have the same underlying <see cref="System.Guid"/> value.
        /// </summary>
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);

            return x.underlyingGuid == y.underlyingGuid;
        }

        /// <summary>
        /// Determines if both instances have the same underlying <see cref="System.Guid"/> value.
        /// </summary>
        public static bool operator ==(ShortGuid x, Guid y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);

            return x.underlyingGuid == y;
        }

        /// <summary>
        /// Determines if both instances have the same underlying <see cref="System.Guid"/> value.
        /// </summary>
        public static bool operator ==(Guid x, ShortGuid y) => y == x; // NB: order of arguments

        /// <summary>
        /// Determines if both ShortGuid instances do not have the same underlying <see cref="System.Guid"/> value.
        /// </summary>
        public static bool operator !=(ShortGuid x, ShortGuid y) => !(x == y);

        /// <summary>
        /// Determines if both instances do not have the same underlying <see cref="System.Guid"/> value.
        /// </summary>
        public static bool operator !=(ShortGuid x, Guid y) => !(x == y);

        /// <summary>
        /// Determines if both instances do not have the same underlying <see cref="System.Guid"/> value.
        /// </summary>
        public static bool operator !=(Guid x, ShortGuid y) => !(x == y);

        /// <summary>
        /// Implicitly converts the ShortGuid to its string equivalent.
        /// </summary>
        public static implicit operator string(ShortGuid shortGuid) => shortGuid.encodedString;

        /// <summary>
        /// Implicitly converts the ShortGuid to its <see cref="System.Guid"/> equivalent.
        /// </summary>
        public static implicit operator Guid(ShortGuid shortGuid) => shortGuid.underlyingGuid;

        /// <summary>
        /// Implicitly converts the string to a ShortGuid.
        /// </summary>
        public static implicit operator ShortGuid(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ShortGuid.Empty;

            if (TryParse(value, out ShortGuid shortGuid))
                return shortGuid;

            throw new FormatException("ShortGuid should contain 22 Base64 characters or "
                + "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).");
        }

        /// <summary>
        /// Implicitly converts the <see cref="System.Guid"/> to a ShortGuid.
        /// </summary>
        public static implicit operator ShortGuid(Guid guid)
        {
            if (guid == Guid.Empty)
                return ShortGuid.Empty;

            return new ShortGuid(guid);
        }

        #endregion Operators

        /// <summary>
        /// Tries to parse the value as a <see cref="ShortGuid"/> or <see cref="System.Guid"/> string, and outputs an actual <see cref="ShortGuid"/> instance.
        ///
        /// <para>The difference between TryParse and TryDecode:</para>
        /// <list type="number">
        ///     <item>
        ///         <term><see cref="TryParse(string, out ShortGuid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> before attempting parsing as a <see cref="System.Guid"/>, outputs the actual <see cref="ShortGuid"/> instance - this method.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="TryParse(string, out Guid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> before attempting parsing as a <see cref="System.Guid"/>, outputs the underlying <see cref="System.Guid"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="TryDecode(string, out Guid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> only, but outputs the result as a <see cref="System.Guid"/>.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="value">The ShortGuid encoded string or string representation of a Guid.</param>
        /// <param name="shortGuid">A new <see cref="ShortGuid"/> instance from the parsed string.</param>
        /// <param name="strict">If true the re-encoded result has to exactly match the input <paramref name="value"/>; if false any valid base64 string will be accepted.</param>
        /// <returns>A boolean indicating if the parse was successful.</returns>
        public static bool TryParse(string value, out ShortGuid shortGuid, bool strict = false)
        {
            // Try a ShortGuid string.
            if (ShortGuid.TryDecode(value, out var guid, strict))
            {
                shortGuid = guid;
                return true;
            }

            // Try a Guid string.
            if (Guid.TryParse(value, out guid))
            {
                shortGuid = guid;
                return true;
            }
            shortGuid = ShortGuid.Empty;
            return false;
        }

        /// <summary>
        /// Tries to parse the value as a <see cref="ShortGuid"/> or <see cref="System.Guid"/> string, and outputs the underlying <see cref="Guid"/> value.
        ///
        /// <para>The difference between TryParse and TryDecode:</para>
        /// <list type="number">
        ///     <item>
        ///         <term><see cref="TryParse(string, out ShortGuid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> before attempting parsing as a <see cref="System.Guid"/>, outputs the actual <see cref="ShortGuid"/> instance.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="TryParse(string, out Guid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> before attempting parsing as a <see cref="System.Guid"/>, outputs the underlying <see cref="System.Guid"/> - this method.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="TryDecode(string, out Guid, bool)"/></term>
        ///         <description>Tries to parse as a <see cref="ShortGuid"/> only, but outputs the result as a <see cref="System.Guid"/>.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="value">The ShortGuid encoded string or string representation of a Guid.</param>
        /// <param name="guid">A new <see cref="System.Guid"/> instance from the parsed string.</param>
        /// <param name="strict">If true the re-encoded result has to exactly match the input <paramref name="value"/>; if false any valid base64 string will be accepted.</param>
        /// <returns>A boolean indicating if the parse was successful.</returns>
        public static bool TryParse(string value, out Guid guid, bool strict = false)
        {
            // Try a ShortGuid string.
            if (ShortGuid.TryDecode(value, out guid, strict))
                return true;

            // Try a Guid string.
            if (Guid.TryParse(value, out guid))
                return true;

            guid = Guid.Empty;
            return false;
        }
    }
}
