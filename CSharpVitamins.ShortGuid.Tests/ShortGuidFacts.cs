using CSharpVitamins;

using System;

using Xunit;

namespace Tests
{
    public class ShortGuidFacts
    {
        private const string SampleGuidString = "c9a646d3-9c61-4cb7-bfcd-ee2522c8f633";
        private static readonly Guid SampleGuid = new Guid(SampleGuidString);
        private const string SampleShortGuidString = "00amyWGct0y_ze4lIsj2Mw";

        private void assert_instance_equals_samples(ShortGuid instance)
        {
            Assert.Equal(SampleShortGuidString, instance.Value);
            Assert.Equal(SampleGuid, instance.Guid);
        }

        [Fact]
        private void ctor_decodes_shortguid_string()
        {
            var actual = new ShortGuid(SampleShortGuidString);

            assert_instance_equals_samples(actual);
        }

        [Fact]
        private void invalid_strings_must_not_return_true_on_try_parse()
        {
            Assert.True(ShortGuid.TryParse("bullshitmustnotbevalid", out ShortGuid _, strict: false));
            Assert.False(ShortGuid.TryParse("bullshitmustnotbevalid", out ShortGuid _, strict: true));
        }

        [Fact]
        private void ctor_throws_when_trying_to_decode_guid_string()
        {
            Assert.Throws<FormatException>(
                () => new ShortGuid(SampleGuidString)
                );
        }

        [Fact]
        private void TryParse_decodes_shortguid_string()
        {
            ShortGuid.TryParse(SampleShortGuidString, out ShortGuid actual);

            assert_instance_equals_samples(actual);
        }

        [Fact]
        private void TryParse_decodes_guid_string()
        {
            ShortGuid.TryParse(SampleGuidString, out ShortGuid actual);

            assert_instance_equals_samples(actual);
        }

        [Fact]
        private void TryParse_decodes_empty_guid_literal_as_empty()
        {
            bool result = ShortGuid.TryParse(Guid.Empty.ToString(), out ShortGuid actual);

            Assert.True(result);
            Assert.Equal(Guid.Empty, actual.Guid);
        }

        [Fact]
        private void TryParse_decodes_empty_string_as_empty()
        {
            bool result = ShortGuid.TryParse(string.Empty, out ShortGuid actual);

            Assert.False(result);
            Assert.Equal(Guid.Empty, actual.Guid);
        }

        [Fact]
        private void TryParse_decodes_bad_string_as_empty()
        {
            bool result = ShortGuid.TryParse("Nothing to see here...", out ShortGuid actual);

            Assert.False(result);
            Assert.Equal(Guid.Empty, actual.Guid);
        }

        [Fact]
        private void Encode_creates_expected_string()
        {
            string actual = ShortGuid.Encode(SampleGuid);

            Assert.Equal(SampleShortGuidString, actual);
        }

        [Fact]
        private void Decode_takes_expected_string()
        {
            Guid actual = ShortGuid.Decode(SampleShortGuidString);

            Assert.Equal(SampleGuid, actual);
        }

        [Fact]
        private void Decode_fails_on_unexpected_string()
        {
            Assert.Throws<FormatException>(
                () => ShortGuid.Decode("Am I valid?")
                );
        }

        [Fact]
        private void instance_equality_equals()
        {
            var actual = new ShortGuid(SampleShortGuidString);

            Assert.True(actual.Equals(actual));
            Assert.False(actual.Equals(null));
            Assert.True(actual.Equals(SampleGuid));
            Assert.True(actual.Equals(SampleGuidString));
            Assert.True(actual.Equals(SampleShortGuidString));
        }

        [Fact]
        private void operator_eqaulity_equals()
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
        private void ShortGuid_Emtpy_equals_Guid_Empty()
        {
            Assert.True(Guid.Empty.Equals(ShortGuid.Empty));
        }
    }
}
