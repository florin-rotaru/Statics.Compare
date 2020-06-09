using Air.Reflection;
using AutoFixture;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static Air.Compare.Members;
using static Test.Models;

namespace Test
{
    public class CompareMembers
    {
        Fixture Fixture { get; }

        readonly ITestOutputHelper Console;

        public CompareMembers(ITestOutputHelper console)
        {
            Fixture = new Fixture();
            Console = console;
        }

        public StructSystemTypeCodes CreateStructSystemTypeCodes()
        {
            return new StructSystemTypeCodes
            {
                BooleanType = true,
                ByteType = Fixture.Create<byte>(),
                CharType = Fixture.Create<char>(),
                DateTimeOffsetType = Fixture.Create<DateTimeOffset>(),
                DateTimeType = Fixture.Create<DateTime>(),
                DecimalType = Fixture.Create<decimal>(),
                DoubleType = Fixture.Create<double>(),
                DtoEnumType = DtoEnumType.B,
                EnumType = EnumType.C,
                GuidType = Guid.NewGuid(),
                Int16SType = Fixture.Create<short>(),
                Int32Type = Fixture.Create<int>(),
                Int64Type = Fixture.Create<long>(),
                SByteType = Fixture.Create<sbyte>(),
                SingleType = Fixture.Create<float>(),
                StringType = Fixture.Create<string>(),
                TimeSpanType = Fixture.Create<TimeSpan>(),
                UInt16Type = Fixture.Create<ushort>(),
                UInt32Type = Fixture.Create<uint>(),
                UInt64Type = Fixture.Create<ulong>()
            };
        }

        [Fact]
        public void LiteralValue()
        {
            MemberInfo memberInfo = TypeInfo.GetMembers(typeof(int)).First(w => w.Name == nameof(int.MaxValue));

            Assert.Equal(int.MaxValue, memberInfo.DefaultValue);
        }

        [Fact]
        public void CompareEqualsList()
        {
            var left = Fixture.Create<Misc>();
            var right = left;

            Assert.True(CompareEquals(left.TypeCodesList, right.TypeCodesList));

            right = Fixture.Create<Misc>();
            Assert.False(CompareEquals(left.TypeCodesList, right.TypeCodesList));
        }

        [Fact]
        public void CompareEqualsArray()
        {
            var left = Fixture.Create<TClass[]>();
            var right = left;

            Assert.True(CompareEquals(left, right));

            right = Fixture.Create<TClass[]>();
            Assert.False(CompareEquals(left, right));
        }

        [Fact]
        public void CompareEqualsClass()
        {
            var left = Fixture.Create<TClass>();
            var right = left;

            Assert.True(CompareEquals(left, right));
        }

        [Fact]
        public void CompareEqualsAbstract()
        {
            var left = typeof(TClass);
            var right = typeof(TClass);

            Assert.True(CompareEquals(left, right));
        }

        [Fact]
        public void CompareEqualsNullableClass()
        {
            var left = Fixture.Create<TNullableClass>();
            var right = left;

            Assert.True(CompareEquals(left, right));
        }

        [Fact]
        public void CompareEqualsNullable()
        {
            var nonNullable = CreateStructSystemTypeCodes();
            var left = new StructSystemTypeCodes?();
            left = nonNullable;
            var right = left;

            Assert.True(CompareEquals(left, right));
        }

        [Fact]
        public void CompareNullableClassEqualsType()
        {
            var left = Fixture.Create<TNullableClass>();
            var right = new TClass
            {
                BooleanType = left.BooleanType.Value,
                CharType = left.CharType.Value,
                SByteType = left.SByteType.Value,
                ByteType = left.ByteType.Value,
                Int16SType = left.Int16SType.Value,
                UInt16Type = left.UInt16Type.Value,
                Int32Type = left.Int32Type.Value,
                UInt32Type = left.UInt32Type.Value,
                Int64Type = left.Int64Type.Value,
                UInt64Type = left.UInt64Type.Value,
                SingleType = left.SingleType.Value,
                DoubleType = left.DoubleType.Value,
                DecimalType = left.DecimalType.Value,
                StringType = left.StringType,
                DateTimeType = left.DateTimeType.Value,
                DateTimeOffsetType = left.DateTimeOffsetType.Value,
                TimeSpanType = left.TimeSpanType.Value,
                GuidType = left.GuidType.Value,
                EnumType = left.EnumType.Value,
                DtoEnumType = left.DtoEnumType.Value,
                ObjectType = left.ObjectType
            };

            Assert.True(CompareEquals(left, right));
        }

        [Fact]
        public void CompareClassEqualsNullable()
        {
            var left = Fixture.Create<TClass>();
            var right = new TNullableClass
            {
                BooleanType = left.BooleanType,
                CharType = left.CharType,
                SByteType = left.SByteType,
                ByteType = left.ByteType,
                Int16SType = left.Int16SType,
                UInt16Type = left.UInt16Type,
                Int32Type = left.Int32Type,
                UInt32Type = left.UInt32Type,
                Int64Type = left.Int64Type,
                UInt64Type = left.UInt64Type,
                SingleType = left.SingleType,
                DoubleType = left.DoubleType,
                DecimalType = left.DecimalType,
                StringType = left.StringType,
                DateTimeType = left.DateTimeType,
                DateTimeOffsetType = left.DateTimeOffsetType,
                TimeSpanType = left.TimeSpanType,
                GuidType = left.GuidType,
                EnumType = left.EnumType,
                DtoEnumType = left.DtoEnumType,
                ObjectType = left.ObjectType
            };

            Assert.True(CompareEquals(left, right));
        }

        [Fact]
        public void CompareClassNotEquals()
        {
            var left = Fixture.Create<TClass>();
            var right = Fixture.Create<TClass>();

            Assert.False(CompareEquals(left, right));
        }

        [Fact]
        public void CompareNullableNotEquals()
        {
            var left = Fixture.Create<TNullableClass>();
            var right = Fixture.Create<TNullableClass>();

            Assert.False(CompareEquals(left, right));
        }

        [Fact]
        public void CompareNotEqualsClass()
        {
            var left = Fixture.Create<TClass[]>();
            var right = Fixture.Create<TClass[]>();

            Assert.False(CompareEquals(left, right));
        }

        [Fact]
        public void CompareClassNotEqualsNullable()
        {
            var left = Fixture.Create<TClass>();
            var right = Fixture.Create<TNullableClass>();

            Assert.False(CompareEquals(left, right));
        }

        [Fact]
        public void CompareNullableNotEqualsClass()
        {
            var left = Fixture.Create<TClass>();
            var right = Fixture.Create<TNullableClass>();

            Assert.False(CompareEquals(left, right));
        }

        [Fact]
        public void CompareNoneEqualsClass()
        {
            var left = Fixture.Create<TClass>();
            var right = Fixture.Create<TClass>();
            right.BooleanType = !left.BooleanType;
            Assert.True(CompareNoneEquals(left, right));

            right.BooleanType = left.BooleanType;
            Assert.False(CompareNoneEquals(left, right));
        }
    }
}
