using CSharpVitamins;
using System;
using Xunit;

namespace Tests
{
    public class ShortGuidFacts
    {
        const string SampleGuidString = "c9a646d3-9c61-4cb7-bfcd-ee2522c8f633";
        static readonly Guid SampleGuid = new Guid(SampleGuidString);
        const string SampleShortGuidString = "00amyWGct0y_ze4lIsj2Mw";

        /// <summary>
        /// Literal: c9a646d3-9c61-4cb7-bfcd-ee2522c8f633 and some extra chars.
        /// </summary>
        const string LongerBase64String = "YzlhNjQ2ZDMtOWM2MS00Y2I3LWJmY2QtZWUyNTIyYzhmNjMzIGFuZCBzb21lIGV4dHJhIGNoYXJzLg";

        /// <summary>
        /// "bullshitmustnotbevalid" in this case does produce a valid Guid, which when output encodes as correctly
        /// as "bullshitmustnotbevaliQ".
        /// </summary>
        const string InvalidSampleShortGuidString = "bullshitmustnotbevalid";

        void assert_instance_equals_samples(ShortGuid instance)
        {
            Assert.Equal(SampleShortGuidString, instance.Value);
            Assert.Equal(SampleGuid, instance.Guid);
        }

        [Fact]
        void ctor_decodes_shortguid_string()
        {
            var actual = new ShortGuid(SampleShortGuidString);

            assert_instance_equals_samples(actual);
        }

        [Fact]
        void StrictDecode_parses_valid_shortGuid_strict_off()
        {
            ShortGuid.Decode(SampleShortGuidString);
        }

        [Fact]
        void StrictDecode_parses_valid_shortGuid_strict_on()
        {
            ShortGuid.Decode(SampleShortGuidString);
        }

        [Fact]
        void Decode_does_not_parse_longer_base64_string()
        {
            Assert.Throws<ArgumentException>(
                () => ShortGuid.Decode(LongerBase64String)
                );
        }

        [Fact]
        void StrictDecode_does_not_parse_longer_base64_string()
        {
            Assert.Throws<ArgumentException>(
                () => ShortGuid.Decode(LongerBase64String)
                );
        }

        [Fact]
        void invalid_strings_must_not_return_true_on_try_parse_with_strict_true()
        {
            // try parse should return false
            Assert.False(ShortGuid.TryParse(InvalidSampleShortGuidString, out ShortGuid strictSguid));

            // decode should throw
            Assert.Throws<FormatException>(
                () => ShortGuid.Decode(InvalidSampleShortGuidString)
                );

            // .ctor should throw
            Assert.Throws<FormatException>(
                () => new ShortGuid(InvalidSampleShortGuidString)
                );
        }

        [Fact]
        void ctor_throws_when_trying_to_decode_guid_string()
        {
            Assert.Throws<ArgumentException>(
                () => new ShortGuid(SampleGuidString)
                );
        }

        [Fact]
        void TryParse_decodes_shortguid_string()
        {
            ShortGuid.TryParse(SampleShortGuidString, out ShortGuid actual);

            assert_instance_equals_samples(actual);
        }

        [Fact]
        void TryParse_decodes_guid_string()
        {
            ShortGuid.TryParse(SampleGuidString, out ShortGuid actual);

            assert_instance_equals_samples(actual);
        }

        [Fact]
        void TryParse_decodes_empty_guid_literal_as_empty()
        {
            bool result = ShortGuid.TryParse(Guid.Empty.ToString(), out ShortGuid actual);

            Assert.True(result);
            Assert.Equal(Guid.Empty, actual.Guid);
        }

        [Fact]
        void TryParse_decodes_empty_string_as_empty()
        {
            bool result = ShortGuid.TryParse(string.Empty, out ShortGuid actual);

            Assert.False(result);
            Assert.Equal(Guid.Empty, actual.Guid);
        }

        [Fact]
        void TryParse_decodes_bad_string_as_empty()
        {
            bool result = ShortGuid.TryParse("Nothing to see here...", out ShortGuid actual);

            Assert.False(result);
            Assert.Equal(Guid.Empty, actual.Guid);
        }

        [Fact]
        void Encode_creates_expected_string()
        {
            string actual = ShortGuid.Encode(SampleGuid);

            Assert.Equal(SampleShortGuidString, actual);
        }

        [Fact]
        void Decode_takes_expected_string()
        {
            Guid actual = ShortGuid.Decode(SampleShortGuidString);

            Assert.Equal(SampleGuid, actual);
        }

        [Fact]
        void Decode_fails_on_unexpected_string()
        {
            Assert.Throws<ArgumentException>(
                () => ShortGuid.Decode("Am I valid?")
                );

            Assert.Throws<FormatException>(
                () => ShortGuid.Decode("I am 22characters long")
                );
        }

        [Fact]
        void instance_equality_equals()
        {
            var actual = new ShortGuid(SampleShortGuidString);

            Assert.True(actual.Equals(actual));
            Assert.False(actual.Equals(null));
            Assert.True(actual.Equals(SampleGuid));
            Assert.True(actual.Equals(SampleGuidString));
            Assert.True(actual.Equals(SampleShortGuidString));
        }

        [Fact]
        void operator_eqaulity_equals()
        {
            ShortGuid actual = new ShortGuid(SampleShortGuidString);

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(actual == actual);
#pragma warning restore CS1718 // Comparison made to same variable

            Assert.False(actual == null);

            Assert.True(actual == SampleGuid);
            Assert.True(SampleGuid == actual);

            Assert.True(actual == SampleGuidString);
            Assert.True(actual == SampleShortGuidString);

            Assert.True(null == ShortGuid.Empty);
        }

        [Fact]
        void ShortGuid_Emtpy_equals_Guid_Empty()
        {
            Assert.True(Guid.Empty.Equals(ShortGuid.Empty));
        }
    }
}
