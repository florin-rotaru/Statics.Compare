using System;
using System.Collections.Generic;

namespace Test
{
    public class Models
    {

        public class RecursiveNode
        {
            public string Name { get; set; }

            public RecursiveNode ParentNode { get; set; }
            public List<RecursiveNode> ChildNodes { get; set; }
        }

        public enum EnumType
        {
            Undefined,
            A,
            B,
            C
        }

        public enum DtoEnumType
        {
            A,
            B,
            C
        }

        public class Misc
        {
            public TClass TypeCodes { get; set; }
            public List<TClass> TypeCodesList { get; set; }
        }

        public class Node
        {
            public string Name { get; set; }
            public Segment Segment { get; set; }
        }

        public class StaticNode
        {
            public static string Name { get; set; }
            public static Segment Segment { get; set; }
        }

        public class Segment
        {
            public string Name { get; set; }
            public TClass SystemTypeCodes { get; set; }
            public TNullableClass NullableSystemTypeCodes { get; set; }
        }

        public class TClass
        {
            public bool BooleanType { get; set; }
            public char CharType { get; set; }
            public sbyte SByteType { get; set; }
            public byte ByteType { get; set; }
            public short Int16SType { get; set; }
            public ushort UInt16Type { get; set; }
            public int Int32Type { get; set; }
            public uint UInt32Type { get; set; }
            public long Int64Type { get; set; }
            public ulong UInt64Type { get; set; }
            public float SingleType { get; set; }
            public double DoubleType { get; set; }
            public decimal DecimalType { get; set; }
            public string StringType { get; set; }

            public DateTime DateTimeType { get; set; }
            public DateTimeOffset DateTimeOffsetType { get; set; }
            public TimeSpan TimeSpanType { get; set; }

            public Guid GuidType { get; set; }

            public EnumType EnumType { get; set; }
            public DtoEnumType DtoEnumType { get; set; }

            public object ObjectType { get; set; }
        }

        public class TLiteralClass
        {
            public const bool BooleanType = true;
            public const char CharType = '0';
            public const sbyte SByteType = 1;
            public const byte ByteType = 2;
            public const short Int16SType = 3;
            public const ushort UInt16Type = 4;
            public const int Int32Type = 5;
            public const uint UInt32Type = 6;
            public const long Int64Type = 7;
            public const ulong UInt64Type = 8;
            public const float SingleType = 9;
            public const double DoubleType = 10;
            public const decimal DecimalType = 11;
            public const string StringType = "12";
        }

        public class TReadonlyClass
        {
            public bool BooleanType { get; }
            public char CharType { get; }
            public sbyte SByteType { get; }
            public byte ByteType { get; }
            public short Int16SType { get; }
            public ushort UInt16Type { get; }
            public int Int32Type { get; }
            public uint UInt32Type { get; }
            public long Int64Type { get; }
            public ulong UInt64Type { get; }
            public float SingleType { get; }
            public double DoubleType { get; }
            public decimal DecimalType { get; }
            public string StringType { get; }

            public DateTime DateTimeType { get; }
            public DateTimeOffset DateTimeOffsetType { get; }
            public TimeSpan TimeSpanType { get; }

            public Guid GuidType { get; }

            public EnumType EnumType { get; }
            public DtoEnumType DtoEnumType { get; }

            public object ObjectType { get; }
        }

        public class TNullableClass
        {
            public bool? BooleanType { get; set; }
            public char? CharType { get; set; }
            public sbyte? SByteType { get; set; }
            public byte? ByteType { get; set; }
            public short? Int16SType { get; set; }
            public ushort? UInt16Type { get; set; }
            public int? Int32Type { get; set; }
            public uint? UInt32Type { get; set; }
            public long? Int64Type { get; set; }
            public ulong? UInt64Type { get; set; }
            public float? SingleType { get; set; }
            public double? DoubleType { get; set; }
            public decimal? DecimalType { get; set; }
            public string StringType { get; set; }

            public DateTime? DateTimeType { get; set; }
            public DateTimeOffset? DateTimeOffsetType { get; set; }
            public TimeSpan? TimeSpanType { get; set; }

            public Guid? GuidType { get; set; }

            public EnumType? EnumType { get; set; }
            public DtoEnumType? DtoEnumType { get; set; }

            public object ObjectType { get; set; }
        }

        public class TStaticClass
        {
            public static bool BooleanType { get; set; }
            public static char CharType { get; set; }
            public static sbyte SByteType { get; set; }
            public static byte ByteType { get; set; }
            public static short Int16SType { get; set; }
            public static ushort UInt16Type { get; set; }
            public static int Int32Type { get; set; }
            public static uint UInt32Type { get; set; }
            public static long Int64Type { get; set; }
            public static ulong UInt64Type { get; set; }
            public static float SingleType { get; set; }
            public static double DoubleType { get; set; }
            public static decimal DecimalType { get; set; }
            public static string StringType { get; set; }

            public static DateTime DateTimeType { get; set; }
            public static DateTimeOffset DateTimeOffsetType { get; set; }
            public static TimeSpan TimeSpanType { get; set; }

            public Guid GuidType { get; set; }

            public static EnumType EnumType { get; set; }
            public static DtoEnumType DtoEnumType { get; set; }

            public static object ObjectType { get; set; }
        }

        public class TNullableStaticClass
        {
            public static bool? BooleanType { get; set; }
            public static char? CharType { get; set; }
            public static sbyte? SByteType { get; set; }
            public static byte? ByteType { get; set; }
            public static short? Int16SType { get; set; }
            public static ushort? UInt16Type { get; set; }
            public static int? Int32Type { get; set; }
            public static uint? UInt32Type { get; set; }
            public static long? Int64Type { get; set; }
            public static ulong? UInt64Type { get; set; }
            public static float? SingleType { get; set; }
            public static double? DoubleType { get; set; }
            public static decimal? DecimalType { get; set; }
            public static string StringType { get; set; }

            public static DateTime? DateTimeType { get; set; }
            public static DateTimeOffset? DateTimeOffsetType { get; set; }
            public static TimeSpan? TimeSpanType { get; set; }

            public static Guid? GuidType { get; set; }

            public static EnumType? EnumType { get; set; }
            public static DtoEnumType? DtoEnumType { get; set; }

            public static object ObjectType { get; set; }
        }

        public struct StructSegment
        {
            public StructSystemTypeCodes SystemTypeCodes { get; set; }
            public StructNullableSystemTypeCodes NullableSystemTypeCodes { get; set; }
        }

        public struct StructSystemTypeCodes
        {
            public bool BooleanType { get; set; }
            public char CharType { get; set; }
            public sbyte SByteType { get; set; }
            public byte ByteType { get; set; }
            public short Int16SType { get; set; }
            public ushort UInt16Type { get; set; }
            public int Int32Type { get; set; }
            public uint UInt32Type { get; set; }
            public long Int64Type { get; set; }
            public ulong UInt64Type { get; set; }
            public float SingleType { get; set; }
            public double DoubleType { get; set; }
            public decimal DecimalType { get; set; }
            public string StringType { get; set; }

            public DateTime DateTimeType { get; set; }
            public DateTimeOffset DateTimeOffsetType { get; set; }
            public TimeSpan TimeSpanType { get; set; }

            public Guid GuidType { get; set; }

            public EnumType EnumType { get; set; }
            public DtoEnumType DtoEnumType { get; set; }

            public object ObjectType { get; set; }
        }

        public struct StructNullableSystemTypeCodes
        {
            public bool? BooleanType { get; set; }
            public char? CharType { get; set; }
            public sbyte? SByteType { get; set; }
            public byte? ByteType { get; set; }
            public short? Int16SType { get; set; }
            public ushort? UInt16Type { get; set; }
            public int? Int32Type { get; set; }
            public uint? UInt32Type { get; set; }
            public long? Int64Type { get; set; }
            public ulong? UInt64Type { get; set; }
            public float? SingleType { get; set; }
            public double? DoubleType { get; set; }
            public decimal? DecimalType { get; set; }
            public string StringType { get; set; }

            public DateTime? DateTimeType { get; set; }
            public DateTimeOffset? DateTimeOffsetType { get; set; }
            public TimeSpan? TimeSpanType { get; set; }

            public Guid? GuidType { get; set; }

            public EnumType? EnumType { get; set; }
            public DtoEnumType? DtoEnumType { get; set; }

            public object ObjectType { get; set; }
        }
    }
}
